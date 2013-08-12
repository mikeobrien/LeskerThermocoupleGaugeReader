using System;
using System.IO;
using System.Linq;

namespace LeskerThermocoupleGauge.CLI
{
    class Program
    {
        private static decimal _lastPressure;

        static void Main(string[] args)
        {
            var path = args.Any() ? string.Join(" ", args) : null;

            Console.WriteLine("KJ Lesker Thermocouple");
            Console.WriteLine();
            using (var reader = new Reader())
            {
                reader.Open(x =>
                {
                    if (x.Pressure == _lastPressure) return;
                    _lastPressure = x.Pressure;
                    Console.WriteLine("{0:hh:mm:ss}: {1}", x.Timestamp, x.Pressure);
                    if (path != null) File.AppendAllText(path, string.Format("{0:yyyy-MM-dd hh:mm:ss}\t{1}\r\n", x.Timestamp, x.Pressure));
                });
                Console.ReadKey();
            }
        }
    }
}
