using MirAI.AI;
using MirAI.DB;
using System;
using System.Collections.Generic;


namespace MirAI
{
    class MirAIConsole
    {
        static void Main(string[] args)
        {
            Node.RegisterValidator(CheckNode);
            MirDBRoutines.CreateSomeDB();
            PrintColor(MirDBRoutines.ToString(), ConsoleColor.Blue);

            Console.WriteLine("Загрузка данных из БД в 'List<Program> programs'.");
            List<Program> programs = Program.GetListPrograms();
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
            Console.WriteLine($"\nКорректная длина программы? {prog.CheckProgram(ref len, ref programs)}");
            Console.WriteLine($"Длина программы '{prog.Name}' (только Action и Condition с учетом подпрограмм): {len}");

            Console.WriteLine($"Запуск программы '{prog.Name}'.");
            Node n = prog.Run(ref programs);
            //Node n = null;
            Console.Write($"Результат программы '{prog.Name}': ");
            if (n is null)
                Console.WriteLine($"null");
            else
                Console.WriteLine($"{n.Id}.{n.Type} из '{programs.Find(p => p.Id == n.ProgramId).Name}'");

            Console.WriteLine("\n=> OK");
            Console.Read();
        }

        public static bool CheckNode(Node node)
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
