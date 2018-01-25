using Modbus.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modbus.Device
{
    /// <summary>
    ///     Modbus Serial Master device.
    /// </summary>
    public interface IModbusAsyncSerialMaster : IModbusAsyncMaster
    {
        /// <summary>
        ///     Transport for used by this master.
        /// </summary>
        new ModbusAsyncSerialTransport Transport { get; }

        /// <summary>
        ///     Serial Line only.
        ///     Diagnostic function which loops back the original data.
        ///     NModbus only supports looping back one ushort value, this is a
        ///     limitation of the "Best Effort" implementation of the RTU protocol.
        /// </summary>
        /// <param name="slaveAddress">Address of device to test.</param>
        /// <param name="data">Data to return.</param>
        /// <returns>Return true if slave device echoed data.</returns>
        bool ReturnQueryData(byte slaveAddress, ushort data);
    }
}
