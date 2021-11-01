using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MirAI.AI
{
    public enum UnitType { }

    [Table("Units")]
    public class Unit
    {
        public int Id { get; set; }
        public UnitType Type { get; set; }
        public Program UnitProgram { get; set; }

        public Unit()
        {
            //Program UnitProgram = new Program();
        }
        public override string ToString()
        {
            return $"│{Id,-5}│{Type,-15}│{UnitProgram.Id,-30}│";
        }
    }
}
