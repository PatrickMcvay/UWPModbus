

namespace Modbus.Device
{
    using System;
    using System.Diagnostics.CodeAnalysis;
#if SERIAL
    using System.IO.Ports;
#endif
    using System.Net.Sockets;

    using Data;
    using IO;
    using Message;
    using System.Threading.Tasks;

    public class ModbusSerialAsyncMaster : ModbusAsyncMaster, IModbusAsyncSerialMaster
    {
        private ModbusSerialAsyncMaster(ModbusAsyncTransport transport)
            : base(transport)
        {
        }

        /// <summary>
        ///     Gets the Modbus Transport.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        ModbusAsyncSerialTransport IModbusAsyncSerialMaster.Transport
        {
            get { return (ModbusAsyncSerialTransport)Transport; }
        }

#if SERIAL
        /// <summary>
        ///     Modbus ASCII master factory method.
        /// </summary>
        public static ModbusSerialAsyncMaster CreateAscii(SerialPort serialPort)
        {
            if (serialPort == null)
            {
                throw new ArgumentNullException(nameof(serialPort));
            }

            return CreateAscii(new SerialPortAdapter(serialPort));
        }
#endif

        /// <summary>
        ///     Modbus ASCII master factory method.
        /// </summary>
        public static ModbusSerialAsyncMaster CreateAscii(TcpClient tcpClient)
        {
            if (tcpClient == null)
            {
                throw new ArgumentNullException(nameof(tcpClient));
            }

            return CreateAscii(new TcpClientAdapter(tcpClient));
        }

        /// <summary>
        ///     Modbus ASCII master factory method.
        /// </summary>
        public static ModbusSerialAsyncMaster CreateAscii(UdpClient udpClient)
        {
            if (udpClient == null)
            {
                throw new ArgumentNullException(nameof(udpClient));
            }

            if (!udpClient.Client.Connected)
            {
                throw new InvalidOperationException(Resources.UdpClientNotConnected);
            }

            return CreateAscii(new UdpClientAdapter(udpClient));
        }

        /// <summary>
        ///     Modbus ASCII master factory method.
        /// </summary>
        public static ModbusSerialAsyncMaster CreateAscii(IStreamResource streamResource)
        {
            if (streamResource == null)
            {
                throw new ArgumentNullException(nameof(streamResource));
            }

            throw new NotImplementedException();
            //return new ModbusSerialAsyncMaster(new ModbusAsciiTransport(streamResource));
        }

#if SERIAL
        /// <summary>
        ///     Modbus RTU master factory method.
        /// </summary>
        public static ModbusSerialAsyncMaster CreateRtu(SerialPort serialPort)
        {
            if (serialPort == null)
            {
                throw new ArgumentNullException(nameof(serialPort));
            }

            return CreateRtu(new SerialPortAdapter(serialPort));
        }
#endif

        /// <summary>
        ///     Modbus RTU master factory method.
        /// </summary>
        public static ModbusSerialAsyncMaster CreateRtu(TcpClient tcpClient)
        {
            if (tcpClient == null)
            {
                throw new ArgumentNullException(nameof(tcpClient));
            }
            throw new NotImplementedException();
            //return CreateRtu(new TcpClientAdapter(tcpClient));
        }

        /// <summary>
        ///     Modbus RTU master factory method.
        /// </summary>
        public static ModbusSerialAsyncMaster CreateRtu(UdpClient udpClient)
        {
            if (udpClient == null)
            {
                throw new ArgumentNullException(nameof(udpClient));
            }

            if (!udpClient.Client.Connected)
            {
                throw new InvalidOperationException(Resources.UdpClientNotConnected);
            }
            throw new NotImplementedException();
            //return CreateRtu(new UdpClientAdapter(udpClient));
        }

        /// <summary>
        ///     Modbus RTU master factory method.
        /// </summary>
        public static ModbusSerialAsyncMaster CreateRtu(IAsyncStreamResource streamResource)
        {
            if (streamResource == null)
            {
                throw new ArgumentNullException(nameof(streamResource));
            }

            return new ModbusSerialAsyncMaster(new ModbusAsyncRtuTransport(streamResource));
        }

        /// <summary>
        ///     Serial Line only.
        ///     Diagnostic function which loops back the original data.
        ///     NModbus only supports looping back one ushort value, this is a limitation of the "Best Effort" implementation of
        ///     the RTU protocol.
        /// </summary>
        /// <param name="slaveAddress">Address of device to test.</param>
        /// <param name="data">Data to return.</param>
        /// <returns>Return true if slave device echoed data.</returns>
        public bool ReturnQueryData(byte slaveAddress, ushort data)
        {
            DiagnosticsRequestResponse request;
            DiagnosticsRequestResponse response;

            request = new DiagnosticsRequestResponse(
                Modbus.DiagnosticsReturnQueryData,
                slaveAddress,
                new RegisterCollection(data));

            response = Transport.UnicastMessageAsync<DiagnosticsRequestResponse>(request);

            return response.Data[0] == data;
        }
    }
}
