using System.ComponentModel.DataAnnotations.Schema;

namespace MirAI.AI
{
    [Table("Links")]
    public class NodeLink
    {
        //public int Id { get; set; }
        public int FromId { get; set; }
        public Node From { get; set; }
        public int ToId { get; set; }
        public Node To { get; set; }

        public NodeLink() { }
        public NodeLink(Node from, Node to)
        {
            From = from;
            FromId = from.Id;
            To = to;
            ToId = to.Id;
        }
    }
}
