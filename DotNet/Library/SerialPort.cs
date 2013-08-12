using System;
using System.IO.Ports;
using System.Linq;

namespace LeskerThermocoupleGauge
{
    public interface IPort : IDisposable
    {
        void Open(Action<string> dataHandler, string name = null);
        void Close();
    }

    public class SerialPort : IPort
    {
        private System.IO.Ports.SerialPort _port;

        public void Open(Action<string> dataHandler, string name = null)
        {
            if (name == null)
            {
                var ports = System.IO.Ports.SerialPort.GetPortNames();
                if (!ports.Any()) throw new Exception("No serial ports found.");
                name = ports[0];
            }
            _port = new System.IO.Ports.SerialPort(name, 9600, Parity.None, 8)
            {
                StopBits = StopBits.One,
                Handshake = Handshake.None
            };

            _port.DataReceived += (s, e) => dataHandler(((System.IO.Ports.SerialPort)s).ReadExisting());
            _port.Open();
        }

        public void Close()
        {
            if (_port != null) _port.Close();
        }

        void IDisposable.Dispose()
        {
            Close();
        }
    }
}