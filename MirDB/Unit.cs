using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MirAI.AI {
    public enum UnitType { }

    [Table( "Units" )]
    public class Unit {
        public int Id { get; set; }
        public UnitType Type { get; set; }
        public Program UnitProgram { get; set; }

        [NotMapped]
        public int X { get; set; }
        [NotMapped]
        public int Y { get; set; }
        [NotMapped]
        public int HP { get; set; }                         // Очки здоровья
        public static int MeleeRange { get; set; } = 1;     // Расстояние ближней атаки/взаимодействия
        public static int LongRange { get; set; } = 10;     // Расстояние дальней атаки

        public Unit() {
            //Program UnitProgram = new Program();
        }
        public override string ToString() {
            return $"│{Id,-5}│{Type,-15}│{UnitProgram.Id,-30}│";
        }
    }
}
