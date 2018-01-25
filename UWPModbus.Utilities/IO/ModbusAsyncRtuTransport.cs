using Modbus.Message;
using Modbus.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modbus.IO
{
    class ModbusAsyncRtuTransport : ModbusAsyncSerialTransport
    {
        public const int RequestFrameStartLength = 7;

        public const int ResponseFrameStartLength = 4;

        public ModbusAsyncRtuTransport(IAsyncStreamResource streamResource) 
            : base(streamResource)
        {
            Debug.Assert(streamResource != null, "Argument streamResource cannot be null.");
        }

        public static int RequestBytesToRead(byte[] frameStart)
        {
            byte functionCode = frameStart[1];
            int numBytes;

            switch (functionCode)
            {
                case Modbus.ReadCoils:
                case Modbus.ReadInputs:
                case Modbus.ReadHoldingRegisters:
                case Modbus.ReadInputRegisters:
                case Modbus.WriteSingleCoil:
                case Modbus.WriteSingleRegister:
                case Modbus.Diagnostics:
                    numBytes = 1;
                    break;
                case Modbus.WriteMultipleCoils:
                case Modbus.WriteMultipleRegisters:
                    byte byteCount = frameStart[6];
                    numBytes = byteCount + 2;
                    break;
                default:
                    string msg = $"Function code {functionCode} not supported.";
                    Debug.WriteLine(msg);
                    throw new NotImplementedException(msg);
            }

            return numBytes;
        }

        public static int ResponseBytesToRead(byte[] frameStart)
        {
            byte functionCode = frameStart[1];

            // exception response
            if (functionCode > Modbus.ExceptionOffset)
            {
                return 1;
            }

            int numBytes;
            switch (functionCode)
            {
                case Modbus.ReadCoils:
                case Modbus.ReadInputs:
                case Modbus.ReadHoldingRegisters:
                case Modbus.ReadInputRegisters:
                    numBytes = frameStart[2] + 1;
                    break;
                case Modbus.WriteSingleCoil:
                case Modbus.WriteSingleRegister:
                case Modbus.WriteMultipleCoils:
                case Modbus.WriteMultipleRegisters:
                case Modbus.Diagnostics:
                    numBytes = 4;
                    break;
                default:
                    numBytes = frameStart[2] + 1;
                    string msg = $"Function code {functionCode} not supported.";
                    Debug.WriteLine(msg);
                    break;
                    //throw new NotImplementedException(msg);
            }

            return numBytes;
        }

        public virtual async Task<byte[]> ReadAsync(int count)
        {
            byte[] frameBytes = new byte[count];

            int numBytesRead = 0;

            while (numBytesRead != count)
            {
                numBytesRead += await StreamResource.ReadAsync(frameBytes, numBytesRead, count - numBytesRead);
            }

            return frameBytes;
        }

        internal override byte[] BuildMessageFrame(IModbusMessage message)
        {
            var messageFrame = message.MessageFrame;
            var crc = ModbusUtility.CalculateCrc(messageFrame);
            var messageBody = new MemoryStream(messageFrame.Length + crc.Length);

            messageBody.Write(messageFrame, 0, messageFrame.Length);
            messageBody.Write(crc, 0, crc.Length);

            return messageBody.ToArray();
        }

        internal override bool ChecksumsMatch(IModbusMessage message, byte[] messageFrame)
        {
            var messInt = BitConverter.ToUInt16(messageFrame, messageFrame.Length - 2);

            var frameInt = BitConverter.ToUInt16(ModbusUtility.CalculateCrc(message.MessageFrame), 0);

            return messInt == frameInt;
        }

        internal override async Task<IModbusMessage> ReadResponseAsync<T>()
        {
            byte[] frameStart = await ReadAsync(ResponseFrameStartLength);
            byte[] frameEnd = await ReadAsync(ResponseBytesToRead(frameStart));
            byte[] frame = Enumerable.Concat(frameStart, frameEnd).ToArray();
            Debug.WriteLine($"RX: {string.Join(", ", frame)}");

            return CreateResponse<T>(frame.Take(3).ToArray());
        }

        internal override async Task<byte[]> ReadRequestAsync()
        {
            byte[] frameStart = await ReadAsync(RequestFrameStartLength);
            byte[] frameEnd = await ReadAsync(RequestBytesToRead(frameStart));
            byte[] frame = Enumerable.Concat(frameStart, frameEnd).ToArray();
            Debug.WriteLine($"RX: {string.Join(", ", frame)}");

            return frame;
        }
    }
}
