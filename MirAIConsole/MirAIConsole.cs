using System;
using System.Collections.Generic;
using MirAI.AI;
using MirAI.DB;


namespace MirAI
{
    class MirAIConsole
    {
        static void Main(string[] args)
        {
            Node.RegisterValidator(CheckNode);
            CreateSomeDB();
            PrintColor(MirDBRoutines.ToString(), ConsoleColor.Blue);

            Console.WriteLine("Загрузка данных из БД в 'List<Program> programs'.");
            List<Program> programs = MirDBRoutines.GetPrograms(true);
            Console.WriteLine($"Количество записей в 'programs': {programs.Count}");
            foreach (var p in programs)
            {
                //Program prog = p.Value;
                Console.WriteLine($"{p.Id} - '{p.Name}':");
                Console.WriteLine($"\tКоличество нодов в '{p.Name}': {p.Nodes.Count}");
                foreach (var nn in p.Nodes)
                {
                    Console.WriteLine($"\t" + nn.ToString());
                }
            }

            Program prog = programs[0];
            int len = 0;
            prog.CheckProgram(ref len, ref programs);
            Console.WriteLine($"\nДлина программы '{prog.Name}' (только Action и Condition с учетом подпрограмм): {len}");

            Console.WriteLine($"Запуск программы '{prog.Name}'.");
            Node n = prog.Run(ref programs);
            Console.WriteLine($"Результат программы '{prog.Name}': {n.Id}.{n.Type} из '{programs.Find(p=>p.Id== n.ProgramId).Name}'");

            Console.WriteLine("\n=> OK");
            Console.Read();
        }

        public static void CreateSomeDB()
        {
            Console.WriteLine("Очистка БД."); MirDBRoutines.Clear();
            Console.WriteLine("Генерация новой БД.");

            Program prog = new Program("ProgAI");
            Program prog2 = new Program("Sub AI");

            Unit unit = new Unit { UnitProgram = prog };
            MirDBRoutines.SaveUnit(unit);

            // Добавление Нодов
            prog.AddNode(NodeType.Root);
            prog.AddNode(NodeType.Condition);
            prog.AddNode(NodeType.Action);
            prog.AddNode(NodeType.SubAI);
            prog.Nodes[0].Command = 0;
            prog.Nodes[1].Command = 777;
            prog.Nodes[2].Command = 7;
            prog.Nodes[3].Command = 423;
            prog.Nodes[0].X = 100;
            prog.Nodes[0].Y = 10;
            prog.Nodes[1].X = 100;
            prog.Nodes[1].Y = 100;
            prog.Nodes[2].X = 10;
            prog.Nodes[2].Y = 200;
            prog.Nodes[3].X = 1200;
            prog.Nodes[3].Y = 1300;
            // Установка связей между Нодами
            prog.AddLink(prog.Nodes[0], prog.Nodes[3]);
            prog.AddLink(prog.Nodes[0], prog.Nodes[1]);
            prog.AddLink(prog.Nodes[1], prog.Nodes[3]);
            prog.AddLink(prog.Nodes[1], prog.Nodes[2]);

            prog2.AddNode(NodeType.Root);
            prog2.AddNode(NodeType.Connector);
            prog2.AddNode(NodeType.Condition);
            prog2.AddNode(NodeType.Action);
            prog2.Nodes[2].Command = 777;
            prog2.Nodes[3].Command = 777;
            prog2.AddLink(prog2.Nodes[0], prog2.Nodes[1]);
            prog2.AddLink(prog2.Nodes[0], prog2.Nodes[2]);
            prog2.AddLink(prog2.Nodes[1], prog2.Nodes[3]);
            prog2.AddLink(prog2.Nodes[2], prog2.Nodes[3]);


            try
            {
                prog.Save();
                prog2.Save();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                PrintColor($"\nИсключение2: {ex.Message}\nInnerException: {ex.InnerException.Message}", ConsoleColor.Red);
            }

            prog.AddLink(prog.Nodes[3], prog2.Nodes[0]);
            prog.Save();

        }

        public static bool CheckNode( Node node )
        {
            bool ret = (node.Command == 777);
            Console.WriteLine($"\tCheckNode({node.Id}.{node.Type}) = {ret}");
            return ret;
        }

        public static void PrintColor(string message, ConsoleColor color)
        {
            var clr = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = clr;
        }
    }
}
