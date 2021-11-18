using Microsoft.EntityFrameworkCore;
using MirAI.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MirAI.AI {
    public enum UnitType { Soldier = 0, Medic = 1, Worker = 2 }

    [Table( "Units" )]
    public class Unit {
        public int Id { get; set; }
        public UnitType Type { get; set; }
        public string Name { get; set; }
        public int ProgramId { get; set; }

        [NotMapped]
        public int X { get; set; }
        [NotMapped]
        public int Y { get; set; }
        [NotMapped]
        public int HP { get; set; }                         // Очки здоровья
        public static int MeleeRange { get; set; } = 1;     // Расстояние ближней атаки/взаимодействия
        public static int LongRange { get; set; } = 10;     // Расстояние дальней атаки

        public Unit() {
        }
        public static List<Unit> GetListUnits() {
            using var db = new MirDBContext();
            List<Unit> units = db.Units.ToList();
            return units;
        }

        public Unit Save() {
            using var db = new MirDBContext();
            if (!db.Units.Contains( this )) {       // Сначала сохраняем 'Unit'
                db.Units.Add( this );
            } else {
                var fromdb = db.Units.SingleOrDefault(p => p.Id == this.Id);
                fromdb.Type = this.Type;
                fromdb.Name = this.Name;
                fromdb.ProgramId = this.ProgramId;
                db.Update( fromdb );
            }
            db.SaveChanges();
            return this;
        }

        public override string ToString() {
            return $"│{Id,-5}│{Type,-15}│{ProgramId,-30}│";
        }
    }
}
