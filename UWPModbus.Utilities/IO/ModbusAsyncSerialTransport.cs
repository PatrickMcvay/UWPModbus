using Modbus.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modbus.IO
{
    public abstract class ModbusAsyncSerialTransport : ModbusAsyncTransport
    {
        private bool _checkFrame = true;

        internal ModbusAsyncSerialTransport(IAsyncStreamResource streamResource)
            : base(streamResource)
        {
            Debug.Assert(streamResource != null, "Argument streamResource cannot be null.");
        }

        /// <summary>
        ///     Gets or sets a value indicating whether LRC/CRC frame checking is performed on messages.
        /// </summary>
        public bool CheckFrame
        {
            get { return _checkFrame; }
            set { _checkFrame = value; }
        }

        internal void DiscardInBuffer()
        {
            StreamResource.DiscardInBuffer();
        }

        internal override void Write(IModbusMessage message)
        {
            

            byte[] frame = BuildMessageFrame(message);
            Debug.WriteLine($"TX: {string.Join(", ", frame)}");
            StreamResource.Write(frame, 0, frame.Length);
        }

        internal override IModbusMessage CreateResponse<T>(byte[] frame)
        {
            IModbusMessage response = base.CreateResponse<T>(frame);
            Debug.WriteLine(string.Format("Response:: " + string.Join(", ", frame), response.TransactionId, response.SlaveAddress, response.ProtocolDataUnit));

            // compare checksum
            if (CheckFrame && !ChecksumsMatch(response, frame))
            {
                string msg = $"Checksums failed to match {string.Join(", ", response.MessageFrame)} != {string.Join(", ", frame)}";
                Debug.WriteLine(msg);
                //throw new IOException(msg);
            }

            return response;
        }

        internal abstract bool ChecksumsMatch(IModbusMessage message, byte[] messageFrame);

        internal override void OnValidateResponse(IModbusMessage request, IModbusMessage response)
        {
            // no-op
        }
    }
}
//Debug.WriteLine(string.Format("Request:: Transaction Id: {0} , Slave Address: {1} , Message: {2}", request.TransactionId, request.SlaveAddress, request.ProtocolDataUnit));
