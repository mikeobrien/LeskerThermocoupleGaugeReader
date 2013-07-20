using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LeskerThermocoupleGauge;
using NUnit.Framework;
using Should;

namespace Tests
{
    [TestFixture]
    public class Tests
    {
        [Test, Ignore("This test has to be run manually with the TC attached.")]
        public void should_read_device()
        {
            var data = new List<string>();
            using (var port = new SerialPort())
            {
                port.Open(data.Add);
                Thread.Sleep(1000);
            }
            data.Count.ShouldBeGreaterThan(0);
            data.All(x => x.Length > 0).ShouldBeTrue();
        }

        [Test]
        public void should_read_data()
        {
            //"1999. \r\n"
            var port = new MemoryPort();
            var data = new List<Reading>();
            using (var reader = new Reader(port))
            {
                reader.Open(data.Add);
                port.Write("1999. \r\n", "1", "2", "3", "4", ".", " ", "\r", "\n", "11", "11", ". ",  
                    "\r\n", "0999. \r\n08", "88. \r\n7777. \r\n6666. \r\n5555. \r", "\n");
            }

            data[0].Pressure.ShouldEqual(1999);
            data[1].Pressure.ShouldEqual(1234);
            data[2].Pressure.ShouldEqual(1111);
            data[3].Pressure.ShouldEqual(0999);
            data[4].Pressure.ShouldEqual(0888);
            data[5].Pressure.ShouldEqual(7777);
            data[6].Pressure.ShouldEqual(6666);
            data[7].Pressure.ShouldEqual(5555);

            data.All(x => x.Timestamp > DateTime.Now.AddSeconds(-5) &&
                x.Timestamp < DateTime.Now.AddSeconds(1)).ShouldBeTrue();
        }
    }

    public class MemoryPort : IPort
    {
        private Action<string> _dataHandler;

        public void Open(Action<string> dataHandler, string name = null)
        {
            _dataHandler = dataHandler;
        }

        public void Write(params string[] data)
        {
            data.ToList().ForEach(x => _dataHandler(x));
        }

        public void Close() { }
        public void Dispose() { }
    }
}
