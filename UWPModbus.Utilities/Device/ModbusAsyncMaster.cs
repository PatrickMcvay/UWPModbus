

namespace Modbus.Device
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using Data;
    using IO;
    using Message;

    public abstract class ModbusAsyncMaster : ModbusAsyncDevice, IModbusAsyncMaster
    {
        internal ModbusAsyncMaster(ModbusAsyncTransport transport)
            : base(transport)
        {

        }

        /// <summary>
        ///    Asynchronously reads from 1 to 2000 contiguous coils status.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of coils to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public bool[] ReadCoilsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 2000);

            var request = new ReadCoilsInputsRequest(
                Modbus.ReadCoils,
                slaveAddress,
                startAddress,
                numberOfPoints);

            return PerformReadDiscretesAsync(request);
        }

        /// <summary>
        ///    Asynchronously reads from 1 to 2000 contiguous discrete input status.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of discrete inputs to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public bool[] ReadInputsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 2000);

            var request = new ReadCoilsInputsRequest(
                Modbus.ReadInputs,
                slaveAddress,
                startAddress,
                numberOfPoints);

            return PerformReadDiscretesAsync(request);
        }

        /// <summary>
        ///    Asynchronously reads contiguous block of holding registers.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public ushort[] ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 125);

            var request = new ReadHoldingInputRegistersRequest(
                Modbus.ReadHoldingRegisters,
                slaveAddress,
                startAddress,
                numberOfPoints);

            return PerformReadRegistersAsync(request);
        }

        /// <summary>
        ///    Asynchronously reads contiguous block of input registers.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public ushort[] ReadInputRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 125);

            var request = new ReadHoldingInputRegistersRequest(
                Modbus.ReadInputRegisters,
                slaveAddress,
                startAddress,
                numberOfPoints);

            return PerformReadRegistersAsync(request);
        }

        /// <summary>
        ///    Asynchronously writes a single coil value.
        /// </summary>
        /// <param name="slaveAddress">Address of the device to write to.</param>
        /// <param name="coilAddress">Address to write value to.</param>
        /// <param name="value">Value to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public Task WriteSingleCoilAsync(byte slaveAddress, ushort coilAddress, bool value)
        {
            var request = new WriteSingleCoilRequestResponse(slaveAddress, coilAddress, value);
            return PerformWriteRequestAsync<WriteSingleCoilRequestResponse>(request);
        }

        /// <summary>
        ///    Asynchronously writes a single holding register.
        /// </summary>
        /// <param name="slaveAddress">Address of the device to write to.</param>
        /// <param name="registerAddress">Address to write.</param>
        /// <param name="value">Value to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress, ushort value)
        {
            var request = new WriteSingleRegisterRequestResponse(
                slaveAddress,
                registerAddress,
                value);

            return PerformWriteRequestAsync<WriteSingleRegisterRequestResponse>(request);
        }

        /// <summary>
        ///    Asynchronously writes a block of 1 to 123 contiguous registers.
        /// </summary>
        /// <param name="slaveAddress">Address of the device to write to.</param>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public Task WriteMultipleRegistersAsync(byte slaveAddress, ushort startAddress, ushort[] data)
        {
            ValidateData("data", data, 123);

            var request = new WriteMultipleRegistersRequest(
                slaveAddress,
                startAddress,
                new RegisterCollection(data));

            return PerformWriteRequestAsync<WriteMultipleRegistersResponse>(request);
        }

        /// <summary>
        ///    Asynchronously writes a sequence of coils.
        /// </summary>
        /// <param name="slaveAddress">Address of the device to write to.</param>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public Task WriteMultipleCoilsAsync(byte slaveAddress, ushort startAddress, bool[] data)
        {
            ValidateData("data", data, 1968);

            var request = new WriteMultipleCoilsRequest(
                slaveAddress,
                startAddress,
                new DiscreteCollection(data));

            return PerformWriteRequestAsync<WriteMultipleCoilsResponse>(request);
        }

        /// <summary>
        ///    Asynchronously performs a combination of one read operation and one write operation in a single Modbus transaction.
        ///    The write operation is performed before the read.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startReadAddress">Address to begin reading (Holding registers are addressed starting at 0).</param>
        /// <param name="numberOfPointsToRead">Number of registers to read.</param>
        /// <param name="startWriteAddress">Address to begin writing (Holding registers are addressed starting at 0).</param>
        /// <param name="writeData">Register values to write.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public ushort[] ReadWriteMultipleRegistersAsync(
            byte slaveAddress,
            ushort startReadAddress,
            ushort numberOfPointsToRead,
            ushort startWriteAddress,
            ushort[] writeData)
        {
            ValidateNumberOfPoints("numberOfPointsToRead", numberOfPointsToRead, 125);
            ValidateData("writeData", writeData, 121);

            var request = new ReadWriteMultipleRegistersRequest(
                slaveAddress,
                startReadAddress,
                numberOfPointsToRead,
                startWriteAddress,
                new RegisterCollection(writeData));

            return PerformReadRegistersAsync(request);
        }

        /// <summary>
        ///    Executes the custom message.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The request.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        [SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")]
        public TResponse ExecuteCustomMessageAsync<TResponse>(IModbusMessage request)
            where TResponse : IModbusMessage, new()
        {
            return Transport.UnicastMessageAsync<TResponse>(request);
        }

        private static void ValidateData<T>(string argumentName, T[] data, int maxDataLength)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length == 0 || data.Length > maxDataLength)
            {
                string msg = $"The length of argument {argumentName} must be between 1 and {maxDataLength} inclusive.";
                throw new ArgumentException(msg);
            }
        }

        private static void ValidateNumberOfPoints(string argumentName, ushort numberOfPoints, ushort maxNumberOfPoints)
        {
            if (numberOfPoints < 1 || numberOfPoints > maxNumberOfPoints)
            {
                string msg = $"Argument {argumentName} must be between 1 and {maxNumberOfPoints} inclusive.";
                throw new ArgumentException(msg);
            }
        }

        private bool[] PerformReadDiscretesAsync(ReadCoilsInputsRequest request)
        {
            var response = Transport.UnicastMessageAsync<ReadCoilsInputsResponse>(request);
            return response.Data.Take(request.NumberOfPoints).ToArray();
        }

        private ushort[] PerformReadRegistersAsync(ReadHoldingInputRegistersRequest request)
        {
            ReadHoldingInputRegistersResponse response =
                Transport.UnicastMessageAsync<ReadHoldingInputRegistersResponse>(request);

            return response.Data.Take(request.NumberOfPoints).ToArray();
        }

        private ushort[] PerformReadRegistersAsync(ReadWriteMultipleRegistersRequest request)
        {
            ReadHoldingInputRegistersResponse response =
            Transport.UnicastMessageAsync<ReadHoldingInputRegistersResponse>(request);

            return response.Data.Take(request.ReadRequest.NumberOfPoints).ToArray();
        }

        private Task PerformWriteRequestAsync<T>(IModbusMessage request)
            where T : IModbusMessage, new()
        {
            return Task.Factory.StartNew(() => Transport.UnicastMessageAsync<T>(request));
        }
    }
}
