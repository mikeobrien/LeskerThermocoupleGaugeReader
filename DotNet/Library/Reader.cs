using System;
using System.Linq;

namespace LeskerThermocoupleGauge
{
    public class Reading
    {
        public Reading(decimal pressure)
        {
            Timestamp = DateTime.Now;
            Pressure = pressure;
        }

        public DateTime Timestamp { get; private set; }
        public decimal Pressure { get; private set; }
    }

    public class Reader : IDisposable
    {
        private readonly IPort _port;
        private string _buffer;

        public Reader()
        {
            _port = new SerialPort();
        }

        public Reader(IPort port)
        {
            _port = port;
        }

        public void Open(Action<Reading> dataHandler, string name = null)
        {
            _buffer = "";
            _port.Open(x => ReadStream(x, dataHandler), name);
        }

        private void ReadStream(string data, Action<Reading> dataHandler)
        {
            string results = null;
            lock (_buffer)
            {
                _buffer += data;
                var index = _buffer.LastIndexOf("\r\n");
                if (index >= 0)
                {
                    index += 2;
                    results = _buffer.Substring(0, index);
                    _buffer = index == _buffer.Length ? "" : _buffer.Substring(index);
                }
            }
            if (results != null) results.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x != "_---. ").ToList().ForEach(x => dataHandler(new Reading(decimal.Parse(x.Trim()))));
        }

        public void Close()
        {
            _port.Close();
        }

        void IDisposable.Dispose()
        {
            Close();
        } 
    }
}
