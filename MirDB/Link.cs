using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MirAI.AI {
    [Table("Links")]
    public class NodeLink : IEquatable<NodeLink> {
        public int FromId { get; set; }
        public Node From { get; set; }
        public int ToId { get; set; }
        public Node To { get; set; }

        public NodeLink() { }
        public NodeLink(Node from, Node to) {
            From = from;
            FromId = (From is null) ? 0 : from.Id;
            To = to;
            ToId = (To is null) ? 0 : to.Id;
        }

        public bool Equals(NodeLink other) {
            if (other is null)
                return false;
            if (this.From == other.From && this.To == other.To)
                return true;
            else
                return false;
        }

        public override bool Equals(Object obj) {
            if (obj is null)
                return false;
            NodeLink NodeLinkObj = obj as NodeLink;
            if (NodeLinkObj == null)
                return false;
            else
                return Equals(NodeLinkObj);
        }

        public override int GetHashCode() {
            return this.To.GetHashCode() + this.From.GetHashCode();
        }
        public static bool operator ==(NodeLink NodeLink1, NodeLink NodeLink2) {
            if (((object)NodeLink1) == null || ((object)NodeLink2) == null)
                return Object.Equals(NodeLink1, NodeLink2);
            return NodeLink1.Equals(NodeLink2);
        }

        public static bool operator !=(NodeLink NodeLink1, NodeLink NodeLink2) {
            if (((object)NodeLink1) == null || ((object)NodeLink2) == null)
                return !Object.Equals(NodeLink1, NodeLink2);
            return !(NodeLink1.Equals(NodeLink2));
        }
    }
}
