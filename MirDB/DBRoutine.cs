using Microsoft.EntityFrameworkCore;
using MirAI.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MirAI.DB
{
    /// <summary>
    /// Класс методов взаимодействия с БД
    /// </summary>
    public static class MirDBRoutines
    {
        /// <summary>
        /// Очистка БД путём её удаления и создания новой пустой БД
        /// </summary>
        public static void Clear()
        {
            using MirDBContext db = new MirDBContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        /// <summary>
        /// Загрузка списка программ из БД.
        /// </summary>
        /// <param name="loadNodes">Загружать содержимое программ (true) или только названия (false)</param>
        /// <returns>Коллекция программ</returns>
        public static List<Program> GetPrograms(bool loadNodes)
        {
            List<Program> programs;
            using (MirDBContext db = new MirDBContext())
            {
                programs = db.Programs.Include(p => p.Nodes).ThenInclude(n => n.LinkTo).ToList();
            }
            return programs;
        }

        /// <summary>
        /// Загрузка содержимого программы.
        /// </summary>
        /// <param name="programId">Id программы в БД</param>
        /// <returns>Список всех нодов программы с их связями</returns>
        public static List<Node> GetNodes(int programId)
        {
            List<Node> RetNodes;
            using (MirDBContext db = new MirDBContext())
            {
                RetNodes = db.Nodes.Where(p => p.ProgramId == programId).OrderBy(p => p.Type).Include(n => n.LinkTo).ToList();
            }
            return RetNodes;
        }

        /// <summary>
        /// Сохранение или обновление юнита
        /// </summary>
        /// <param name="unit">Юнит</param>
        /// <returns>Сохраненный юнит</returns>
        public static Unit SaveUnit(Unit unit)
        {
            using var db = new MirDBContext();
            if (db.Units.Contains(unit))
                db.Units.Update(unit);
            else
                db.Units.Add(unit);
            db.SaveChanges();
            return unit;
        }

        /// <summary>
        /// Удаление юнита из БД
        /// </summary>
        /// <param name="unit">Удаляемый юнит</param>
        public static void RemoveUnit(Unit unit)
        {
            using MirDBContext db = new MirDBContext();
            if (db.Units.Contains(unit))
            {
                db.Units.Remove(unit);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Сохранение программы (без содержимого) в БД
        /// </summary>
        /// <param name="program">Программа</param>
        /// <returns>Программа после сохранения в БД (получает Id если первое сохранение)</returns>
        public static Program SaveProgramm(Program program)
        {
            using MirDBContext db = new MirDBContext();
            var fromdb = db.Programs.SingleOrDefault(p => p.Id == program.Id);
            fromdb.Nodes = program.Nodes;
            db.Programs.Update(fromdb);
            db.SaveChanges();
            return program;
        }

        /// <summary>
        /// Каскадное удаление программы из БД
        /// </summary>
        /// <param name="programm">Программа</param>
        public static void RemoveProgramm(Program programm)
        {
            using MirDBContext db = new MirDBContext();
            if (db.Programs.Contains(programm))
            {
                db.Programs.Remove(programm);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Сохранение (или обновление) Нода с его связями в БД
        /// </summary>
        /// <param name="node">Нода программы</param>
        public static void SaveNode(Node node)
        {
            using MirDBContext db = new MirDBContext();
            var nodes = db.Nodes.Where(n => n.Id == node.Id).Include(n => n.LinkTo);
            foreach (var n in nodes)
            {
                if (n.Id == node.Id)
                {
                    n.LinkTo = node.LinkTo;
                    if (db.Nodes.Contains(n))
                        db.Nodes.Update(n);
                    else
                        db.Nodes.Add(n);
                    db.SaveChanges();
                    return;
                }
            }
        }

        /// <summary>
        /// Удаление Ноды из БД
        /// </summary>
        /// <param name="node">Нода программы</param>
        public static void RemoveNode(Node node)
        {
            using MirDBContext db = new MirDBContext();
            if (db.Nodes.Contains(node))
            {
                db.Nodes.Remove(node);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Печать таблиц БД в текстовом виде.
        /// </summary>
        /// <returns>Строка с содержимым БД</returns>
        public static new string ToString()
        {
            StringBuilder str = new StringBuilder();
            using MirDBContext db = new MirDBContext();
            // получаем объекты из бд и выводим на консоль
            var units = db.Units.ToList();
            var programms = db.Programs.Include(p => p.Nodes).ThenInclude(n => n.LinkTo).ToList();
            var nodes = db.Nodes.ToList();
            str.Append("\nДанные из MirDB:\n");
            str.Append(" [Units]\n");
            str.Append($"┌─────┬───────────────┬──────────────────────────────┐\n");
            str.Append($"│{"Id",-5}│{"Type",-15}│{"Prog",-30}│\n");
            str.Append($"├─────┼───────────────┼──────────────────────────────┤\n");
            if (units.Count > 0)
            {
                foreach (Unit u in units)
                {
                    str.Append(u.ToString());
                    str.Append('\n');
                }
            }
            str.Append($"└─────┴───────────────┴──────────────────────────────┘\n\n");
            str.Append(" [Programms]\n");
            str.Append($"┌─────┬──────────────────────────────┐\n");
            str.Append($"│{"Id",-5}│{"Name",-30}│\n");
            str.Append($"├─────┼──────────────────────────────┤\n");
            if (programms.Count > 0)
            {
                foreach (Program p in programms)
                {
                    str.Append(p.ToString());
                    str.Append('\n');
                }
            }
            str.Append($"└─────┴──────────────────────────────┘\n\n");
            str.Append(" [Nodes & Connections]\n");
            str.Append($"┌─────┬─────┬───────────────┬──────────┬───────────┬────────────────────────────┐\n");
            str.Append($"│{"Id",-5}│{"Prog",-5}│{"Type",-15}│{"Command",-10}│({"X",4},{"Y",4})│{"Connections",-28}│\n");
            str.Append($"├─────┼─────┼───────────────┼──────────┼───────────┼────────────────────────────┤\n");
            if (nodes.Count > 0)
            {
                foreach (Node u in nodes)
                {
                    str.Append(u.ToString());
                    str.Append('\n');
                }
            }
            str.Append($"└─────┴─────┴───────────────┴──────────┴───────────┴────────────────────────────┘\n");
            return str.ToString();
        }

        public static void CreateSomeDB()
        {
            //Console.WriteLine("Очистка БД.");
            MirDBRoutines.Clear();
            //Console.WriteLine("Генерация новой БД.");

            Program prog = new Program("ProgAI");
            Program prog2 = new Program("Sub AI");

            //Unit unit = new Unit { UnitProgram = prog };
            //MirDBRoutines.SaveUnit(unit);

            // Добавление Нодов
            //prog.AddNode(NodeType.Root);
            prog.AddNode(prog.Nodes[0], NodeType.Condition);
            prog.AddNode(prog.Nodes[1], NodeType.Action);
            prog.AddNode(prog.Nodes[1], NodeType.SubAI);
            prog.Nodes[0].Command = 0;
            prog.Nodes[1].Command = 777;
            prog.Nodes[2].Command = 7;
            prog.Nodes[3].Command = 423;
            prog.Nodes[0].X = 100;
            prog.Nodes[0].Y = 10;
            prog.Nodes[1].X = 100;
            prog.Nodes[1].Y = 100;
            prog.Nodes[2].X = 10;
            prog.Nodes[2].Y = 300;
            prog.Nodes[3].X = 200;
            prog.Nodes[3].Y = 300;
            prog.Nodes[0].Save();
            prog.Nodes[1].Save();
            prog.Nodes[2].Save();
            prog.Nodes[3].Save();
            // Установка связей между Нодами
            prog.AddLink(prog.Nodes[0], prog.Nodes[3]);

            prog2.AddNode(prog2.Nodes[0], NodeType.Connector);
            prog2.AddNode(prog2.Nodes[0], NodeType.Condition);
            prog2.AddNode(prog2.Nodes[1], NodeType.Action);
            prog2.Nodes[2].Command = 777;
            prog2.Nodes[3].Command = 777;
            prog2.Nodes[0].X = 100;
            prog2.Nodes[0].Y = 10;
            prog2.Nodes[1].X = 10;
            prog2.Nodes[1].Y = 100;
            prog2.Nodes[2].X = 200;
            prog2.Nodes[2].Y = 100;
            prog2.Nodes[3].X = 200;
            prog2.Nodes[3].Y = 300;
            prog2.Nodes[0].Save();
            prog2.Nodes[1].Save();
            prog2.Nodes[2].Save();
            prog2.Nodes[3].Save();
            prog2.AddLink(prog2.Nodes[2], prog2.Nodes[3]);

            prog.Save();
            prog2.Save();

            prog.AddLink(prog.Nodes[3], prog2.Nodes[0]);
            //prog.Save();
            //prog2.Save();

            //prog2.AddLink(prog2.Nodes[3], prog.Nodes[0]);
            //prog2.Save();
        }
    }
}
