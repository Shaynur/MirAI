using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MirAI.AI
{
    public enum NodeType { Root = 0, Action = 1, Condition = 2, Connector = 3, SubAI = 4 }
    // Объявляем делегат
    public delegate bool NodeAIValidator(Node node);

    [Table("Nodes")]
    public class Node //: IEquatable<Node>
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public NodeType Type { get; set; }
        public int Command { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        [NotMapped]
        public bool discovered;

        [NotMapped]
        public List<Node> Next { get; set; } = new List<Node>();

        public Node()
        {
        }
        public Node(int program, NodeType type)
        {
            ProgramId = program;
            Type = type;
        }

        public bool AddChildNode(Node node)
        {
            if (this.Type == NodeType.Action ||
                node is null ||
                ((this.Type == NodeType.SubAI) && (node.Type != NodeType.Root)) ||
                this.Next.Contains(node))
                return false;
            Next.Add(node);
            return true;
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
        public bool IsValid()
        {
            if (Validator != null)
                return Validator.Invoke(this);
            return false;
        }
        /*
                //-------------------------------------------------------------------------
                // Блок методов для реализации интерфейса IEquatable<Node>
                //-------------------------------------------------------------------------
                public override bool Equals(object obj)
                {
                    if (obj == null) return false;
                    Node objAsNodeAI = obj as Node;
                    if (objAsNodeAI == null) return false;
                    else return Equals(objAsNodeAI);
                }
                public override int GetHashCode()
                {
                    return Id.GetHashCode();
                }
                public bool Equals(Node other)
                {
                    if (other is null) return false;
                    return (this.Id.Equals(other.Id));
                }
                public static bool operator ==(Node node1, Node node2)
                {
                    return node1.Equals(node2);
                }
                public static bool operator !=(Node node1, Node node2)
                {
                    return !node1.Equals(node2);
                }
        */
        //-------------------------------------------------------------------------
        /// <summary>
        /// Переопределение метода ToString для класса Node
        /// </summary>
        public override string ToString()
        {
            StringBuilder ret = new StringBuilder($"│{Id,5}│{ProgramId,5}│{Type,-15}│{Command,10}│({X,4},{Y,4})│");
            if (Next != null && Next.Count > 0)
            {
                foreach (Node node in Next)
                {
                    ret.Append($"[{node.Id}]");
                }
            }
            ret.Append(' ', 80 - ret.Length).Append('│');
            return ret.ToString();
        }
    }
}
