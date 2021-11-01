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
        public static int MaxLenght { get; set; } = 1000;
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
            if (!(owner is null) && !(child is null) && Nodes.Contains(owner) &&    // проверка нодов на 'null' и принадлежность 'owner' списку 'Nodes'
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
            UnDiscover();
            Node curnode = Nodes.Find(n => n.Type == NodeType.Root);
            if (!(curnode is null))
            {
                curnode.discovered = true;
                Console.WriteLine($"{curnode.Id}.{curnode.Type} в Run({this.Name})"); //TODO временно
                curnode = DFC(curnode, ref programs);
            }
            return curnode;
        }

        /// <summary>
        /// Рекурсивная реализация DFS (Depth-first search).
        /// Просматривает все связанные ноды программы в поисках валидной
        /// ноды действия.
        /// </summary>
        /// <param name="node">Нода от которой делать просмотр</param>
        /// <param name="programs">Ссылка на список всех программ для возможности
        /// переходов на подпрограммы</param>
        /// <returns>Первая валидная нода действия или 'null' если такой не существует</returns>
        private Node DFC(Node node, ref List<Program> programs)
        {
            Node ret = null;
            foreach (var n in node.Next)          // Ноды в 'node.Next' должны быть уже упорядочены в порядке их просмотра
            {                                     // согласно правилам игры.
                if (ret != null)
                    break;
                if (!n.discovered)
                {
                    n.discovered = true;
                    Console.WriteLine($"{n.Id}.{n.Type} в DFC({node.Id})"); //TODO временно
                    switch (n.Type)
                    {
                        case NodeType.Action:
                            {
                                if (n.IsValid())
                                    ret = n;
                                break;
                            }
                        case NodeType.Root:
                        case NodeType.Connector:
                            {
                                ret = DFC(n, ref programs);
                                break;
                            }
                        case NodeType.Condition:
                            {
                                if (n.IsValid())
                                    ret = DFC(n, ref programs);
                                break;
                            }
                        case NodeType.SubAI:
                            {
                                if (n.Next.Count > 0)
                                {
                                    Node nextrootnode = n.Next[0];
                                    Program nextprog = programs.Find(p => p.Id == nextrootnode.ProgramId);
                                    ret = nextprog.Run(ref programs);
                                }
                                break;
                            }
                        default:            // неизвестный тип ноды просто пропускается
                            break;
                    }
                }
            }
            return ret;
        }

        public bool CheckProgram(ref int lenght, ref List<Program> programs)
        {
            UnDiscover();
            Node curnode = Nodes.Find(n => n.Type == NodeType.Root);
            if (!(curnode is null))
            {
                curnode.discovered = true;
                CheckDFC(curnode, ref lenght, ref programs);
            }
            return (lenght <= MaxLenght);
        }

        private void CheckDFC(Node node, ref int lenght, ref List<Program> programs)
        {
            foreach (var n in node.Next)
            {
                if (lenght > MaxLenght)
                    return;
                if (!n.discovered)
                {
                    n.discovered = true;
                    switch (n.Type)
                    {
                        case NodeType.Action:
                            {
                                lenght++;
                                break;
                            }
                        case NodeType.Root:
                        case NodeType.Connector:
                            {
                                CheckDFC(n, ref lenght, ref programs);
                                break;
                            }
                        case NodeType.Condition:
                            {
                                lenght++;
                                CheckDFC(n, ref lenght, ref programs);
                                break;
                            }
                        case NodeType.SubAI:
                            {
                                if (n.Next.Count > 0)
                                {
                                    Node nextrootnode = n.Next[0];
                                    Program nextprog = programs.Find(p => p.Id == nextrootnode.ProgramId);
                                    nextprog.CheckProgram(ref lenght, ref programs);
                                }
                                break;
                            }
                        default:            // неизвестный тип ноды просто пропускается
                            break;
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
