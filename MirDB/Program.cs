using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using MirAI.DB;
using ServiceStack.DataAnnotations;

namespace MirAI.AI
{
    ///====================================================================================
    /// <summary>
    /// Класс набора узлов (Node) которые (не обязательно) представляют некий исполняемый
    /// код для отдельного юнита (или именованную подпрограмму).
    /// </summary>
    ///====================================================================================
    [Table("Programs")]
    public class Program
    {
        public int Id { get; set; }
        [Unique, Column(TypeName = "varchar(30)")]
        public string Name { get; set; }
        public List<Node> Nodes { get; set; } = new List<Node>();
        /// <summary>
        /// Максимальная длинна программы ( учитываются только ноды действия и ноды условий.
        /// Длинну можно проверить функцией 'CheckProgram()'
        /// </summary>
        public static int MaxLenght { get; set; } = 10;
        public Program() { }
        public Program(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Добавить новый пустой нод в список нодов
        /// </summary>
        /// <param name="type">Тип добавляемого нода</param>
        /// <returns>Добавленный нод</returns>
        public Node AddNode(NodeType type)
        {
            Node node = new Node(this.Id, type);
            Nodes.Add(node);
            return node;
        }

        /// <summary>
        /// Установка связи между двумя нодами программы
        /// </summary>
        /// <param name="owner">Родительский нод</param>
        /// <param name="child">Присоединяемый нод</param>
        /// <returns>true если связь была установленна, false если связь невозможна</returns>
        public bool AddLink(Node owner, Node child)
        {
            if (!(owner is null) && !(child is null) && Nodes.Contains(owner) &&
                (((owner.Type != NodeType.SubAI) && Nodes.Contains(child)) ||
                 ((owner.Type == NodeType.SubAI) && !Nodes.Contains(child))))
            {
                return owner.AddChildNode(child);
            }
            return false;
        }

        public void Save()
        {
            MirDBRoutines.SaveProgramm(this);
            foreach (var n in Nodes)
                MirDBRoutines.SaveNode(n);
        }

        /// <summary>
        /// Выполнение программы для получения первого валидного нода действия.
        ///     Программа должна быть уже проверенна на допустимый размер
        ///     с помощью ф-ии 'CheckProgram()',
        ///     Ноды в списках 'Node.Next' должны быть уже упорядоченны по
        ///     порядку для просмотра.
        /// </summary>
        /// <param name="programs">Ссылка на список всех программ для возможности
        /// переходов на подпрограммы</param>
        /// <returns>Первая валидная нода действия или 'null' если такой не существует</returns>
        public Node Run(ref List<Program> programs)
        {
            Node curnode = Nodes.Find(n => n.Type == NodeType.Root);
            Console.WriteLine($"Run({this.Name})"); ///TODO временно (Run trace)
            if (!(curnode is null))
            {
                UnDiscover();
                foreach (var node in DFC(curnode))
                {
                    Console.WriteLine($"{node.Id}.{node.Type}"); //TODO временно (Run trace)
                    switch (node.Type)
                    {
                        case NodeType.Action:
                            {
                                if (node.IsValid())
                                    return node;
                                break;
                            }
                        case NodeType.Root:
                        case NodeType.Connector:
                            {
                                break;
                            }
                        case NodeType.Condition:
                            {
                                if (!node.IsValid())
                                    node.discovered = true;
                                break;
                            }
                        case NodeType.SubAI:
                            {
                                node.discovered = true;
                                if (node.Next.Count > 0)
                                {
                                    Program nextprog = programs.Find(p => p.Id == node.Next[0].ProgramId);
                                    Node rn = nextprog.Run(ref programs);
                                    if (rn != null)
                                        return rn;
                                }
                                break;
                            }
                        default:            // неизвестный тип ноды просто пропускается
                            break;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Подсчет длины программы с учетом всех подпрограмм и проверка на превышение максимального
        /// размера (Program.MaxLenght)
        /// </summary>
        /// <param name="lenght">Ссылка на счетчик</param>
        /// <param name="programs">Ссылка на список всех программ для возможности
        /// переходов на подпрограммы</param>
        /// <returns>true если размер не превышает Program.MaxLenght, false если превышает</returns>
        public bool CheckProgram(ref int lenght, ref List<Program> programs)
        {
            Node curnode = Nodes.Find(n => n.Type == NodeType.Root);
            if (!(curnode is null))
            {
                UnDiscover();
                foreach (var node in DFC(curnode))
                {
                    if (lenght > MaxLenght)
                        return false;
                    switch (node.Type)
                    {
                        case NodeType.Action:
                            {
                                lenght++;
                                break;
                            }
                        case NodeType.Root:
                        case NodeType.Connector:
                            {
                                break;
                            }
                        case NodeType.Condition:
                            {
                                lenght++;
                                break;
                            }
                        case NodeType.SubAI:
                            {
                                node.discovered = true;
                                if (node.Next.Count > 0)
                                {
                                    Program nextprog = programs.Find(p => p.Id == node.Next[0].ProgramId);
                                    if (!nextprog.CheckProgram(ref lenght, ref programs))
                                        return false;
                                }
                                break;
                            }
                        default:            // неизвестный тип ноды просто пропускается
                            break;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Рекурсивный итератор всех нод вглубь начиная с fromNode.
        /// Перед первым использованием обнулить флаги просмотра вызовом UnDiscover()
        /// </summary>
        /// <param name="fromNode">Нода с которой начинать перечисление вглубь включая ее саму</param>
        /// <returns>Последовательность нод</returns>
        public IEnumerable<Node> DFC(Node fromNode)
        {
            if (!fromNode.discovered)
                yield return fromNode;
            if (!fromNode.discovered)
            {
                fromNode.discovered = true;
                foreach (var node in fromNode.Next)
                {
                    foreach (var n in DFC(node))
                    {
                        yield return n;
                    }
                }
            }
        }

        /// <summary>
        /// Сброс флагов просмотра всех нод программы
        /// </summary>
        public void UnDiscover()
        {
            foreach (Node node in Nodes)
            {
                node.discovered = false;
            }
        }

        /// <summary>
        /// Переопределение ф-ии ToString для 'Program'
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"│{Id,-5}│{Name,-30}│";
        }
    }
}
