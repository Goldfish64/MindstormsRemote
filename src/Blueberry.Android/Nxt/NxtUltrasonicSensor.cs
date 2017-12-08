/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: NxtUltrasonicSensor.cs
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

using System;
using System.Text;

namespace Blueberry.Nxt
{
    /// <summary>
    /// Represents an NXT ultrasonic sensor.
    /// </summary>
    public class NxtUltrasonicSensor : NxtI2cSensor
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NxtUltrasonicSensor"/> class.
        /// </summary>
        public NxtUltrasonicSensor() : base(0x02)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the polling data.
        /// </summary>
        protected byte? PollingData { get; set; }

        /// <summary>
        /// Gets the measured distance in centimeters since the last poll of the sensor.
        /// </summary>
        public byte? DistanceCm => PollingData;

        /// <summary>
        /// Gets the friendly name of the sensor.
        /// </summary>
        public override string FriendlyName => "Ultrasonic";

        /// <summary>
        /// Gets the value of the sensor as a string.
        /// </summary>
        public override string Value => (DistanceCm != null) ? $"{DistanceCm} cm" : null;

        #endregion

        #region Methods

        #region I2C stuff

        /// <summary>
        /// Reads the factory zero from the sensor.
        /// </summary>
        public byte? ReadFactoryZero()
        {
            // Get factory zero.
            var result = SendBytes(new byte[] { Address, 0x11 }, 1);
            if (result != null)
                return result[0];
            else
                return null;
        }

        /// <summary>
        /// Reads the factory scale factor from the sensor.
        /// </summary>
        public byte? ReadFactoryScaleFactor()
        {
            // Get factory scale factor.
            var result = SendBytes(new byte[] { Address, 0x12 }, 1);
            if (result != null)
                return result[0];
            else
                return null;
        }

        /// <summary>
        /// Reads the factory scale divisor from the sensor.
        /// </summary>
        public byte? ReadFactoryScaleDivisor()
        {
            // Get factory scale divisor.
            var result = SendBytes(new byte[] { Address, 0x13 }, 1);
            if (result != null)
                return result[0];
            else
                return null;
        }

        public string ReadMeasurementUnits()
        {
            // Get measurement units string.
            var result = SendBytes(new byte[] { Address, 0x14 }, 7);
            return (result != null) ? Encoding.ASCII.GetString(result, 0, result.Length).TrimEnd('\0', '?', ' ') : string.Empty;
        }

        public byte? ReadContinousMeasurementInterval()
        {
            // Get continuous measurement interval.
            var result = SendBytes(new byte[] { Address, 0x40 }, 1);
            if (result != null)
                return result[0];
            else
                return null;
        }

        /// <summary>
        /// Reads the command state from the sensor.
        /// </summary>
        public byte? ReadCommandState()
        {
            // Get command state.
            var result = SendBytes(new byte[] { Address, 0x41 }, 1);
            if (result != null)
                return result[0];
            else
                return null;
        }

        /// <summary>
        /// Reads the specified measurement byte from the sensor.
        /// </summary>
        /// <param name="index">The byte to read.</param>
        public byte? ReadMeasurementByte(byte index)
        {
            // Check input.
            if (index < 0 || index > 8)
                throw new ArgumentException("The index must be between 0 and 7.", nameof(index));

            // Get byte.
            var result = SendBytes(new byte[] { Address, (byte)(0x42 + index) }, 1);
            if (result != null)
                return result[0];
            else
                return null;
        }

        /// <summary>
        /// Reads the actual zero from the sensor.
        /// </summary>
        public byte? ReadActualZero()
        {
            // Get actual zero.
            var result = SendBytes(new byte[] { Address, 0x50 }, 1);
            if (result != null)
                return result[0];
            else
                return null;
        }

        /// <summary>
        /// Reads the actual scale factor from the sensor.
        /// </summary>
        public byte? ReadActualScaleFactor()
        {
            // Get actual scale factor.
            var result = SendBytes(new byte[] { Address, 0x51 }, 1);
            if (result != null)
                return result[0];
            else
                return null;
        }

        /// <summary>
        /// Reads the actual scale divisor from the sensor.
        /// </summary>
        public byte? ReadActualScaleDivisor()
        {
            // Get actual scale divisor.
            var result = SendBytes(new byte[] { Address, 0x52 }, 1);
            if (result != null)
                return result[0];
            else
                return null;
        }

        /// <summary>
        /// Turns the sensor off.
        /// </summary>
        public void CommandOff()
        {
            // Send off command.
            SendBytes(new byte[] { Address, 0x41, 0x00 }, 0);
        }

        /// <summary>
        /// Enables single-shot mode. In this mode the ultrasonic sensor will
        /// only make a new measurement every time the command byte is sent to the sensor.
        /// </summary>
        public void CommandSingleShot()
        {
            // Send single-shot command.
            SendBytes(new byte[] { Address, 0x41, 0x01 }, 0);
        }

        /// <summary>
        /// Enables continuous measurement mode. This is the default mode, where the sensor continuously makes
        /// new measurement with the specified interval.
        /// </summary>
        public void CommandContinuousMeasurement()
        {
            // Send continuous measurement command.
            SendBytes(new byte[] { Address, 0x41, 0x02 }, 0);
        }

        /// <summary>
        /// Enables event capture mode. Within this mode the sensor will measure whether any other
        /// ultrasonic sensors are within the vicinity.
        /// </summary>
        public void CommandEventCapture()
        {
            // Send event capture command.
            SendBytes(new byte[] { Address, 0x41, 0x03 }, 0);
        }

        /// <summary>
        /// Resets the sensor.
        /// </summary>
        public void CommandRequestWarmReset()
        {
            // Send warm reset command.
            SendBytes(new byte[] { Address, 0x41, 0x04 }, 0);
        }

        /// <summary>
        /// Sets the continuous measurement interval.
        /// </summary>
        /// <param name="interval">The interval to set.</param>
        public void CommandSetContinuousMeasurementInterval(byte interval)
        {
            // Send set continuous measurement interval command.
            SendBytes(new byte[] { Address, 0x40, interval }, 0);
        }

        /// <summary>
        /// Sets the actual zero.
        /// </summary>
        /// <param name="value">The number to set as zero.</param>
        public void CommandSetActualZero(byte value)
        {
            // Send set actual zero command.
            SendBytes(new byte[] { Address, 0x50, value }, 0);
        }

        /// <summary>
        /// Sets the actual scale factor.
        /// </summary>
        /// <param name="value">The number to set as the scale factor.</param>
        public void CommandSetActualScaleFactor(byte value)
        {
            // Send set actual scale factor command.
            SendBytes(new byte[] { Address, 0x51, value }, 0);
        }

        /// <summary>
        /// Sets the actual scale divisor.
        /// </summary>
        /// <param name="value">The number to set as the scale divisor.</param>
        public void CommandSetActualScaleDivisor(byte value)
        {
            // Send set actual scale divisor command.
            SendBytes(new byte[] { Address, 0x52, value }, 0);
        }

        #endregion

        /// <summary>
        /// Attaches the sensor to an <see cref="NxtBrick"/>.
        /// </summary>
        /// <param name="brick">The <see cref="NxtBrick"/> to use.</param>
        /// <param name="port">The port the sensor is on.</param>
        internal override void Attach(NxtBrick brick, NxtInputPorts port)
        {
            // Call base method.
            base.Attach(brick, port);

            // Wake up sensor.
            CommandRequestWarmReset();
        }

        /// <summary>
        /// Detaches the sensor.
        /// </summary>
        internal override void Detach()
        {
            // Turn off sensor.
            CommandOff();

            // Call base method.
            base.Detach();
        }

        /// <summary>
        /// Polls the sensor.
        /// </summary>
        public override void Poll()
        {
            try
            {
                // Get measurement from sensor.
                PollingData = ReadMeasurementByte(0);
            }
            catch (NxtCommunicationException)
            {
                PollingData = null;
            }

            // Call base method.
            base.Poll();
        }

        #endregion
    }
}
