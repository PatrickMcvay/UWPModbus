

namespace Modbus.Device
{
    using System;
    using IO;
    using Unme.Common;

    public abstract class ModbusAsyncDevice : IDisposable
    {
        private ModbusAsyncTransport _transport;

        internal ModbusAsyncDevice(ModbusAsyncTransport transport)
        {
            _transport = transport;
        }

        /// <summary>
        ///     Gets the Modbus Transport.
        /// </summary>
        public ModbusAsyncTransport Transport
        {
            get { return _transport; }
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources;
        ///     <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposableUtility.Dispose(ref _transport);
            }
        }
    }
}
