namespace Modbus.Serial
{
    using System;
    using System.Diagnostics;
    using Windows.Devices.SerialCommunication;

    using Modbus.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Windows.Storage.Streams;
    using System.IO;

    /// <summary>
    ///     Concrete Implementor - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    public class SerialPortAdapter : IStreamResource
    {
        private const string NewLine = "\r\n";
        private SerialDevice _serialPort;

        public SerialPortAdapter(SerialDevice serialPort)
        {
            
            Debug.Assert(serialPort != null, "Argument serialPort cannot be null.");

            _serialPort = serialPort;
            
        }

        public int InfiniteTimeout
        {
            get { return Timeout.Infinite; }
        }

        public int ReadTimeout
        {
            get { return Convert.ToInt32(_serialPort.ReadTimeout); }
            set { _serialPort.ReadTimeout = TimeSpan.FromMilliseconds(value); }
        }

        public int WriteTimeout
        {
            get { return Convert.ToInt32(_serialPort.WriteTimeout); }
            set { _serialPort.WriteTimeout = TimeSpan.FromMilliseconds(value); }
        }

        public void DiscardInBuffer()
        {
            
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            Task.Delay(10).Wait();

            _serialPort.InputStream.ReadAsync(buffer.AsBuffer(), (uint)count, InputStreamOptions.Partial).AsTask().Wait();

            return (int)_serialPort.BytesReceived;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _serialPort.OutputStream.WriteAsync(buffer.AsBuffer()).AsTask().Wait();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _serialPort?.Dispose();
                _serialPort = null;
            }
        }
    }
}
