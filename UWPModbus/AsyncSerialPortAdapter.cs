using System;
using System.Diagnostics;
using Windows.Devices.SerialCommunication;

using Modbus.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using System.IO;

namespace Modbus.Serial
{
    public class AsyncSerialPortAdapter : IAsyncStreamResource
    {
        private const string NewLine = "\r\n";
        private SerialDevice _serialPort;

        public AsyncSerialPortAdapter(SerialDevice serialPort)
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            uint readBufferLength = (uint)count;
            uint myInt;
            try
            {
                Task.Delay(10).Wait();
                DataReader dr = new DataReader(_serialPort.InputStream);
                dr.InputStreamOptions = InputStreamOptions.Partial;
                myInt = (await dr.LoadAsync((uint)count));

                dr.ReadBytes(buffer);

                dr.DetachStream();
                dr.DetachBuffer();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + e.InnerException + e.Source);
                throw;
            }

            return (int)myInt;
        }

        public async void WriteAsync(byte[] buffer)
        {
            await _serialPort.OutputStream.WriteAsync(buffer.AsBuffer());
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            var dw = new DataWriter(_serialPort.OutputStream);

            dw.WriteBuffer(buffer.AsBuffer(), Convert.ToUInt32(offset), Convert.ToUInt32(count));
            
            dw.DetachBuffer();
            dw.DetachStream();

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
