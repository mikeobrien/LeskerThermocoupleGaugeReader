using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeskerThermocoupleGauge.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = args.Any() ? string.Join(" ", args) : null;

            Console.WriteLine("KJ Lesker Thermocouple");
            Console.WriteLine();
            using (var reader = new Reader())
            {
                reader.Open(x =>
                {
                    Console.WriteLine("{0:hh:mm:ss}: {1}", x.Timestamp, x.Pressure);
                    if (path != null) File.AppendAllText(path, string.Format("{0:yyyy-MM-dd hh:mm:ss}\t{1}\r\n", x.Timestamp, x.Pressure));
                });
                Console.ReadKey();
            }
        }
    }
}
