using Microsoft.EntityFrameworkCore;
using MirAI.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace MirAI.AI
{
    public enum NodeType { None = -1, Root = 0, Action = 1, Condition = 2, Connector = 3, SubAI = 4 }
    // Объявляем делегат для обработчика внешней проверки условия в ноде на валидность
    public delegate bool NodeAIValidator(Node node);

    [Table("Nodes")]
    public class Node : IEquatable<Node>, IComparable<Node>
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public NodeType Type { get; set; }
        public int Command { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<NodeLink> LinkFrom { get; set; } = new List<NodeLink>();
        public List<NodeLink> LinkTo { get; set; } = new List<NodeLink>();

        [NotMapped]
        public bool discovered;

        public Node() { }
        public Node(int programId, NodeType type)
        {
            ProgramId = programId;
            Type = type;
        }

        public Node Save()
        {
            using var db = new MirDBContext();
            Save(db);
            db.SaveChanges();
            return this;
        }
        public Node Save(MirDBContext db)
        {
            if (!db.Nodes.Contains(this))       // Если ноды нет в БД - добавляем
                db.Nodes.Add(this);
            else                                // иначе - обновляем содержимое
            {
                var fromdb = db.Nodes.Include(p => p.LinkTo).ThenInclude(u => u.To).SingleOrDefault(p => p.Id == this.Id);
                fromdb.X = this.X;
                fromdb.Y = this.Y;
                fromdb.Command = this.Command;
                foreach (var l in this.LinkTo)  // добавляем отсутствующие линки
                {
                    if (!fromdb.LinkTo.Contains(l))
                        fromdb.LinkTo.Add(l);
                }
                foreach (var l in fromdb.LinkTo)  // и убираем лишние
                {
                    if (!this.LinkTo.Contains(l))
                        db.Remove(l);
                }
                db.Nodes.Update(fromdb);
                return fromdb;
            }
            return this;
        }

        public bool AddChildNode(Node node)
        {
            if (node is null)
                return false;
            using var db = new MirDBContext();
            var dbfrom = db.Nodes.Include(p => p.LinkTo).ThenInclude(u => u.To).SingleOrDefault(p => p.Id == this.Id);
            var dbto = db.Nodes.Include(p => p.LinkTo).ThenInclude(u => u.To).SingleOrDefault(p => p.Id == node.Id);
            //if (dbto == null)
            //    dbto = node.Save(db);
            NodeLink nl = new NodeLink(dbfrom, dbto);
            if (!dbfrom.LinkTo.Contains(nl))
            {
                dbfrom.LinkTo.Add(nl);
                LinkTo.Add(nl);
                node.LinkFrom.Add(nl);
            }
            else
                return false;
            db.SaveChanges();
            return true;
        }

        public void RemoveChildNode(Node node)
        {
            NodeLink nl = new NodeLink { From = this, To = node };
            if (LinkTo.Contains(nl))
                LinkTo.Remove(nl);
            if (node.LinkFrom.Contains(nl))
                node.LinkFrom.Remove(nl);
            using var db = new MirDBContext();
            var dbfrom = db.Nodes.Include(p => p.LinkTo).ThenInclude(u => u.To).SingleOrDefault(p => p.Id == this.Id);
            var linktoremove = dbfrom.LinkTo.Find(u => u.To.Id == node.Id);
            db.Remove(linktoremove);
            db.SaveChanges();
        }

        //-------------------------------------------------------------------------
        // Блок членов для проверки Node на выполняемость условия:
        //
        // Создаем делегат и методы для привязки к нему внешней функции валидации
        // а также метод IsValid() вызываемый для проверки валидности Node,
        // который в свою очередь будет вызывать делегат
        //-------------------------------------------------------------------------
        // Создаем переменную делегата
        static NodeAIValidator Validator;
        // Регистрируем делегат
        public static void RegisterValidator(NodeAIValidator validator)
        {
            Validator = validator;
        }
        // Метод вызываемый для проверки экземпляра Node на валидность
        // Если обработчик не привязан, возвращает false
        public bool IsValid()
        {
            return (Validator?.Invoke(this) ?? false);
        }
        //-------------------------------------------------------------------------
        /// <summary>
        /// Переопределение метода ToString для класса Node
        /// </summary>
        public override string ToString()
        {
            StringBuilder ret = new StringBuilder($"│{Id,5}│{ProgramId,5}│{Type,-15}│{Command,10}│({X,4},{Y,4})│");
            if (LinkTo != null && LinkTo.Count > 0)
            {
                foreach (NodeLink node in LinkTo)
                {
                    ret.Append($"[{node.ToId}]");
                }
            }
            ret.Append(' ', 80 - ret.Length).Append('│');
            return ret.ToString();
        }

        public bool Equals(Node other)
        {
            if (other is null)
                return false;
            if (this.Id == other.Id)
                return true;
            else
                return false;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;
            Node NodeObj = obj as Node;
            if (NodeObj == null)
                return false;
            else
                return Equals(NodeObj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public int CompareTo([AllowNull] Node other)
        {
            if (other is null)
                return 1;
            else
                return this.Type.CompareTo(other.Type);
        }

        public static bool operator ==(Node node1, Node node2)
        {
            if (((object)node1) == null || ((object)node2) == null)
                return Object.Equals(node1, node2);
            return node1.Equals(node2);
        }

        public static bool operator !=(Node node1, Node node2)
        {
            if (((object)node1) == null || ((object)node2) == null)
                return !Object.Equals(node1, node2);
            return !(node1.Equals(node2));
        }
    }
}
