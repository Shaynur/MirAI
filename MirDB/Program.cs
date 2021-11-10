using Microsoft.EntityFrameworkCore;
using MirAI.DB;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        //[NotMapped] //???
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
            Node node = new Node(this.Id, NodeType.Root);
            node.X = 350;
            Nodes.Add(node);
            Save();
        }

        public static List<Program> GetListPrograms()
        {
            using var db = new MirDBContext();
            List<Program> programs = db.Programs
                        .Include(p => p.Nodes)
                        .ThenInclude(n => n.LinkTo)
                        .ThenInclude(l => l.To)
                        .ToList();
            //foreach( var p in programs)   // Сортировка нодов по типу (Node.Type)
            //{
            //    p.Nodes.Sort();
            //}
            return programs;
        }

        public static string GetName(int id)
        {
            using var db = new MirDBContext();
            var fromdb = db.Programs.SingleOrDefault(p => p.Id == id);
            if (fromdb != null)
                return fromdb.Name;
            return null;
        }

        public Node GetRootNode()
        {
            return Nodes.Find(n => n.Type == NodeType.Root);
        }

        /// <summary>
        /// Добавление нового нода в программу
        /// Если указан 'ownerNode', новый нод привязывается к нему
        /// </summary>
        /// <param name="ownerNode">Родительский нод</param>
        /// <param name="type">Тип добавляемого нового нода</param>
        /// <returns>Добавленный в БД нод</returns>
        public Node AddNode(Node ownerNode, NodeType type)
        {
            Node node = new Node(this.Id, type);
            Nodes.Add(node);
            node.Save();
            AddLink(ownerNode, node);
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
            if (!(owner is null) && !(child is null) && Nodes.Contains(owner) &&    // Если родитель есть в программе
                owner.Type != NodeType.Action &&                                    // и родитель не действие
                ((owner.Type != NodeType.SubAI && Nodes.Contains(child)) ||         // и родитель не ПП и при этом чайлд есть в программе
                 (owner.Type == NodeType.SubAI && !Nodes.Contains(child) && child.Type == NodeType.Root && owner.LinkTo.Count == 0))) // еще проверки если родитель ПП
            {
                return owner.AddChildNode(child);
            }
            return false;
        }

        public void Reload()
        {
            using var db = new MirDBContext();
            var fromdb = db.Programs.Include(p => p.Nodes).ThenInclude(n => n.LinkTo).ThenInclude(l => l.To).SingleOrDefault(p => p.Id == this.Id);
            if (fromdb != null)
            {
                this.Name = fromdb.Name;
                this.Nodes = fromdb.Nodes;
            }
        }

        public Program Save()
        {
            using var db = new MirDBContext();
            Save(db);
            db.SaveChanges();
            return this;
        }
        public Program Save(MirDBContext db)
        {
            if (!db.Programs.Contains(this))         // Сначала сохраняем 'Program'
            {
                db.Programs.Add(this);
            }
            else
            {
                var fromdb = db.Programs.SingleOrDefault(p => p.Id == this.Id);
                fromdb.Name = this.Name;
                db.Update(fromdb);
            }
            foreach (var n in Nodes)                // Потом сохраняем все ноды этой программы
            {
                n.Save(db);
            }
            return this;
        }

        public static void RemoveProgramm(Program programm)
        {
            using var db = new MirDBContext();
            if (db.Programs.Contains(programm))
            {
                db.Programs.Remove(programm);
                db.SaveChanges();
            }
        }

        public bool RemoveNode(Node node)
        {
            if (node.Type == NodeType.Root)
                return false;
            using var db = new MirDBContext();
            var fromdb = db.Nodes.Include(p => p.LinkTo).ThenInclude(u => u.To).SingleOrDefault(p => p.Id == node.Id);
            if (fromdb != null)             // Удаляем ноду из БД
            {
                db.Nodes.Remove(fromdb);
                db.SaveChanges();
                Nodes.Remove(node);             // Удаляем из локальной 'Program'
                return true;
            }
            return false;
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
            Console.WriteLine($"Run({this.Name})"); ///TODO временно (Run trace)
            Node curnode = this.GetRootNode();
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
                                if (node.LinkTo.Count > 0)
                                {
                                    Node rn = programs.Find(p => p.Id == node.LinkTo[0].To.ProgramId).Run(ref programs);
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
            Node curnode = this.GetRootNode();
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
                                if (node.LinkTo.Count > 0)
                                {
                                    Program nextprog = programs.Find(p => p.Id == node.LinkTo[0].To.ProgramId);
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
        public static IEnumerable<Node> DFC(Node fromNode)
        {
            if (!fromNode.discovered)
                yield return fromNode;
            if (!fromNode.discovered)
            {
                fromNode.discovered = true;
                foreach (var nodeLink in fromNode.LinkTo)
                {
                    foreach (var n in DFC(nodeLink.To))
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
