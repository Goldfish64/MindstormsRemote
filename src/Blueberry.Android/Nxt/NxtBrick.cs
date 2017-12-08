﻿/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: NxtBrick.cs
* 
* Copyright (c) 2016-2017 John Davis
*
* Permission is hereby granted, free of charge, to any person obtaining a
* copy of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the
* Software is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included
* in all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
* OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
* THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
* IN THE SOFTWARE.
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Android.Bluetooth;
using Java.IO;
using System;
using System.Text;
using System.Threading;

namespace Blueberry.Nxt
{
    /// <summary>
    /// Represents an NXT brick.
    /// </summary>
    public class NxtBrick : NxtDevice, IDisposable
    {
        #region Private variables

        // Bluetooth.
        private BluetoothDevice bluetoothDevice;
        private BluetoothSocket bluetoothSocket;

        // Motors.
        private NxtMotor motorA;
        private NxtMotor motorB;
        private NxtMotor motorC;

        // Sensors.
        private NxtSensor sensor1;
        private NxtSensor sensor2;
        private NxtSensor sensor3;
        private NxtSensor sensor4;

        private object lockObject = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NxtBrick"/> class.
        /// </summary>
        /// <param name="bluetoothDevice">The <see cref="BluetoothDevice"/> to connect with.</param>
        public NxtBrick(BluetoothDevice bluetoothDevice)
        {
            // Save Bluetooth device.
            this.bluetoothDevice = bluetoothDevice ?? throw new ArgumentNullException(nameof(bluetoothDevice), "The Bluetooth device can't be null.");
            brick = this;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether or not the brick is currently connected.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                try
                {
                    return bluetoothSocket?.IsConnected == true;
                }
                catch (ObjectDisposedException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the battery level in mV since the last poll.
        /// </summary>
        public ushort? BatteryLevel { get; private set; }

        /// <summary>
        /// Gets or sets the motor on output port A.
        /// </summary>
        public NxtMotor MotorA
        {
            get { return motorA; }
            set
            {
                motorA?.Detach();
                motorA = value;
                motorA?.Attach(this, NxtOutputPorts.PortA);
            }
        }

        /// <summary>
        /// Gets or sets the motor on output port B.
        /// </summary>
        public NxtMotor MotorB
        {
            get { return motorB; }
            set
            {
                motorB?.Detach();
                motorB = value;
                motorB?.Attach(this, NxtOutputPorts.PortB);
            }
        }

        /// <summary>
        /// Gets or sets the motor on output port C.
        /// </summary>
        public NxtMotor MotorC
        {
            get { return motorC; }
            set
            {
                motorC?.Detach();
                motorC = value;
                motorC?.Attach(this, NxtOutputPorts.PortC);
            }
        }

        /// <summary>
        /// Gets or sets the sensor on input port 1.
        /// </summary>
        public NxtSensor Sensor1
        {
            get { return sensor1; }
            set
            {
                sensor1?.Detach();
                sensor1 = value;
                sensor1?.Attach(this, NxtInputPorts.Port1);
            }
        }

        /// <summary>
        /// Gets or sets the sensor on input port 2.
        /// </summary>
        public NxtSensor Sensor2
        {
            get { return sensor2; }
            set
            {
                sensor2?.Detach();
                sensor2 = value;
                sensor2?.Attach(this, NxtInputPorts.Port2);
            }
        }

        /// <summary>
        /// Gets or sets the sensor on input port 3.
        /// </summary>
        public NxtSensor Sensor3
        {
            get { return sensor3; }
            set
            {
                sensor3?.Detach();
                sensor3 = value;
                sensor3?.Attach(this, NxtInputPorts.Port3);
            }
        }

        /// <summary>
        /// Gets or sets the sensor on input port 4.
        /// </summary>
        public NxtSensor Sensor4
        {
            get { return sensor4; }
            set
            {
                sensor4?.Detach();
                sensor4 = value;
                sensor4?.Attach(this, NxtInputPorts.Port4);
            }
        }

        public override string FriendlyName => "NXT";

        public override string Value => null;

        #endregion

        #region Methods

        /// <summary>
        /// Connects to the NXT over Bluetooth.
        /// </summary>
        public void Connect()
        {
            // Are we connected already?
            if (bluetoothSocket?.IsConnected == true)
                return;

            // Create and open socket.
            try
            {
                // UUID is well-known for Serial Port Protocol (SPP) devices.
                bluetoothSocket = bluetoothDevice.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"));
                bluetoothSocket.Connect();
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Disconnects the Bluetooth connection.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                // Close socket.
                bluetoothSocket?.Close();
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Sends a packet to the NXT brick.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <returns></returns>
        internal byte[] SendPacket(byte[] data)
        {
            // Are we connected?
            if (bluetoothSocket?.IsConnected != true)
                throw new InvalidOperationException();

            // Lock to ensure only a single thread sends/receives at a time.
            lock (lockObject)
            {
                // Create packet.
                var bytes = new byte[data.Length + 2];
                BitConverter.GetBytes((short)data.Length).CopyTo(bytes, 0);
                data.CopyTo(bytes, 2);

                try
                {
                    // Send packet bytes.
                    bluetoothSocket.OutputStream.Write(bytes, 0, bytes.Length);

                    // Read response bytes if response is expected.
                    if (data[0] == (byte)CommandTypes.DirectCommand || data[0] == (byte)CommandTypes.SystemCommand)
                    {
                        // Read length bytes.
                        var lengthBytes = new byte[2];
                        bluetoothSocket.InputStream.Read(lengthBytes, 0, lengthBytes.Length);

                        var response = new byte[BitConverter.ToUInt16(lengthBytes, 0)];
                        bluetoothSocket.InputStream.Read(response, 0, response.Length);
                        return response;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (IOException e)
                {
                    Disconnected?.Invoke(this, EventArgs.Empty);
                    throw new NxtCommunicationException("There was an error communicating with the NXT brick.", e);
                }
            }
        }

        /// <summary>
        /// Polls the NXT.
        /// </summary>
        public override void Poll()
        {
            try
            {
                // Get battery level.
                BatteryLevel = GetBatteryLevel();
            }
            catch (NxtCommunicationException)
            {
                BatteryLevel = null;
            }

            // Call base method.
            base.Poll();
        }

        #endregion

        #region Direct command methods

        /// <summary>
        /// Starts the specified program.
        /// </summary>
        /// <param name="programName"></param>
        public void StartProgram(string programName)
        {
            // Create byte array.
            var bytes = new byte[22];
            bytes[0] = (byte)CommandTypes.DirectCommandNoReply;
            bytes[1] = (byte)RemoteCommands.StartProgram;

            // Get string.
            Encoding.ASCII.GetBytes(programName + Char.MinValue).CopyTo(bytes, 2);

            // Send command to NXT.
            SendPacket(bytes);
        }

        /// <summary>
        /// Stops any current running program.
        /// </summary>
        public void StopProgram()
        {
            // Send command to NXT.
            SendPacket(new byte[] { (byte)CommandTypes.DirectCommandNoReply, (byte)RemoteCommands.StopProgram });
        }

        public void PlaySoundFile(string fileName, bool loop = false)
        {
            // Create byte array.
            var bytes = new byte[22];
            bytes[0] = (byte)CommandTypes.DirectCommandNoReply;
            bytes[1] = (byte)RemoteCommands.PlaySoundFile;
            bytes[2] = Convert.ToByte(loop);

            // Get string.
            Encoding.ASCII.GetBytes(fileName + Char.MinValue).CopyTo(bytes, 3);

            // Send command to NXT.
            SendPacket(bytes);
        }

        /// <summary>
        /// Plays a tone.
        /// </summary>
        /// <param name="frequency">Frequency of the tone in Hz.</param>
        /// <param name="duration">Duration of the tone in milliseconds.</param>
        public void PlayTone(short frequency, short duration)
        {
            // Create byte array.
            var bytes = new byte[6];
            bytes[0] = (byte)CommandTypes.DirectCommandNoReply;
            bytes[1] = (byte)RemoteCommands.PlayTone;
            bytes[2] = (byte)(frequency & 0xFF);
            bytes[3] = (byte)(frequency >> 8);
            bytes[4] = (byte)(duration & 0xFF);
            bytes[5] = (byte)(duration >> 8);

            // Send command to NXT.
            SendPacket(bytes);
        }

        internal void SetOutputState(NxtOutputPorts port, sbyte power, NxtMotorModes mode, NxtMotorRegulationModes regulationMode, sbyte turnRatio,
            NxtMotorRunStates runState, uint tachoLimit = 0)
        {
            // Correct power level.
            if (power > 100) power = 100;
            if (power < -100) power = -100;

            // Correct turn ratio.
            if (turnRatio > 100) turnRatio = 100;
            if (turnRatio < -100) turnRatio = -100;

            // Create array.
            var bytes = new byte[13];
            bytes[0] = (byte)CommandTypes.DirectCommandNoReply;
            bytes[1] = (byte)RemoteCommands.SetOutputState;
            bytes[2] = (byte)port;
            bytes[3] = (byte)power;
            bytes[4] = (byte)mode;
            bytes[5] = (byte)regulationMode;
            bytes[6] = (byte)turnRatio;
            bytes[7] = (byte)runState;
            BitConverter.GetBytes(tachoLimit).CopyTo(bytes, 8);

            // Send command to NXT.
            SendPacket(bytes);
        }

        /// <summary>
        /// Sets the type and mode of the sensor.
        /// </summary>
        internal void SetInputMode(NxtInputPorts port, NxtSensorTypes type, NxtSensorModes mode)
        {
            // Create byte array.
            var bytes = new byte[5];

            // Populate info.
            bytes[0] = (byte)CommandTypes.DirectCommandNoReply;
            bytes[1] = (byte)RemoteCommands.SetInputMode;
            bytes[2] = (byte)port;
            bytes[3] = (byte)type;
            bytes[4] = (byte)mode;

            // Send command to NXT.
            SendPacket(bytes);
        }

        internal NxtGetOutputStateResponse GetOutputState(NxtOutputPorts port)
        {
            // Send command to NXT.
            var response = SendPacket(new byte[] { (byte)CommandTypes.DirectCommand, (byte)RemoteCommands.GetOutputState, (byte)port });

            // Get values.
            var values = new NxtGetOutputStateResponse();
            values.Power = (sbyte)response[4];
            values.Mode = (NxtMotorModes)response[5];
            values.RegulationMode = (NxtMotorRegulationModes)response[6];
            values.TurnRatio = (sbyte)response[7];
            values.RunState = (NxtMotorRunStates)response[8];
            values.TachoLimit = BitConverter.ToUInt32(response, 9);
            values.TachoCount = BitConverter.ToInt32(response, 13);
            values.BlockTachoCount = BitConverter.ToInt32(response, 17);
            values.RotationCount = BitConverter.ToInt32(response, 21);

            // Return the result.
            return values;
        }

        internal NxtGetInputValuesResponse GetInputValues(NxtInputPorts port)
        {
            // Send command to NXT.
            var response = SendPacket(new byte[] { (byte)CommandTypes.DirectCommand, (byte)RemoteCommands.GetInputValues, (byte)port });

            // Get values.
            var values = new NxtGetInputValuesResponse();
            values.Valid = Convert.ToBoolean(response[4]);
            values.Calibrated = Convert.ToBoolean(response[5]);
            values.Type = (NxtSensorTypes)response[6];
            values.Mode = (NxtSensorModes)response[7];
            values.RawValue = BitConverter.ToUInt16(response, 8);
            values.NormalizedValue = BitConverter.ToUInt16(response, 10);
            values.ScaledValue = BitConverter.ToInt16(response, 12);
            values.CalibratedValue = BitConverter.ToInt16(response, 14);

            // Return the result.
            return values;
        }

        internal void ResetInputScaledValue(NxtInputPorts port)
        {
            // Send command to NXT.
            SendPacket(new byte[] { (byte)CommandTypes.DirectCommandNoReply, (byte)RemoteCommands.ResetInputScaledValue, (byte)port });
        }

        public void MessageWrite(byte mailbox, string data)
        {
            // Check mailbox range.
            if (mailbox < 1 || mailbox > 10)
                throw new ArgumentOutOfRangeException(nameof(mailbox), "The mailbox must be between 1 and 10.");

            // Create byte array.
            var bytes = new byte[data.Length + 5];
            bytes[0] = (byte)CommandTypes.DirectCommandNoReply;
            bytes[1] = (byte)RemoteCommands.MessageWrite;
            bytes[2] = (byte)(mailbox - 1); // 0-based.
            bytes[3] = (byte)(data.Length + 1);

            // Get string.
            Encoding.ASCII.GetBytes(data + Char.MinValue).CopyTo(bytes, 4);

            // Send command to NXT.
            SendPacket(bytes);
        }

        internal void ResetMotorPosition(NxtOutputPorts port, bool relative = false)
        {
            // Send command to NXT.
            SendPacket(new byte[] { (byte)CommandTypes.DirectCommandNoReply,
                (byte)RemoteCommands.ResetMotorPosition, (byte)port, Convert.ToByte(relative) });
        }

        /// <summary>
        /// Gets the battery level.
        /// </summary>
        /// <returns></returns>
        internal ushort GetBatteryLevel()
        {
            // Send command.
            var response = SendPacket(new byte[] { (byte)CommandTypes.DirectCommand, (byte)RemoteCommands.GetBatteryLevel });
            return BitConverter.ToUInt16(response, 3);
        }

        /// <summary>
        /// Stops any playing sound files.
        /// </summary>
        public void StopSoundPlayback()
        {
            // Send command to NXT.
            SendPacket(new byte[] { (byte)CommandTypes.DirectCommandNoReply, (byte)RemoteCommands.StopSoundPlayback });
        }

        /// <summary>
        /// Prevent the NXT from idling to sleep.
        /// </summary>
        /// <param name="reply">If true, returns the current time before the NXT goes to sleep.</param>
        /// <returns>The sleep time if requested; otherwise 0.</returns>
        public uint KeepAlive(bool reply = false)
        {
            // Send command to NXT.
            var response = SendPacket(new byte[] { reply ? (byte)CommandTypes.DirectCommand : (byte)CommandTypes.DirectCommandNoReply, (byte)RemoteCommands.KeepAlive });

            // Are we expecting a reply?
            if (reply)
                return BitConverter.ToUInt32(response, 3);
            else
                return 0;
        }

        internal byte? LsGetStatus(NxtInputPorts port)
        {
            // Send command to NXT.
            var response = SendPacket(new byte[] { (byte)CommandTypes.DirectCommand, (byte)RemoteCommands.LsGetStatus, (byte)port });
            return response?[3];
        }

        internal void LsWrite(NxtInputPorts port, byte[] data, byte resultLength)
        {
            // Create byte array.
            var bytes = new byte[data.Length + 5];
            bytes[0] = (byte)CommandTypes.DirectCommandNoReply;
            bytes[1] = (byte)RemoteCommands.LsWrite;
            bytes[2] = (byte)port;
            bytes[3] = (byte)(data.Length);
            bytes[4] = resultLength;

            // Copy data to array for sending.
            data.CopyTo(bytes, 5);

            // Send command to NXT.
            SendPacket(bytes);
        }

        internal byte[] LsRead(NxtInputPorts port)
        {
            // Send command and get up to 16 bytes from specified sensor.
            var result = SendPacket(new byte[] { (byte)CommandTypes.DirectCommand, (byte)RemoteCommands.LsRead, (byte)port });

            // Return bytes.
            var bytes = new byte[result[3]];
            Array.Copy(result, 4, bytes, 0, bytes.Length);
            return bytes;
        }

        #endregion

        #region System commend methods

        /// <summary>
        /// Stops any current running program.
        /// </summary>
        public Version GetFirmwareVersion()
        {
            // Send command to NXT.
            var response = SendPacket(new byte[] { (byte)CommandTypes.SystemCommand, (byte)SystemCommands.GetFirmwareVersion });

            // TODO: Check status.

            // Return firmware version.
            return new Version(response[5], response[6]);
        }

        #endregion

        #region Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Disconnected;

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Disconnect();
            bluetoothSocket.Dispose();
            bluetoothDevice.Dispose();
        }

        #endregion
    }
}