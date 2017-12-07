/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: NxtI2cSensor.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Blueberry.Nxt
{
    /// <summary>
    /// Represents the base class for NXT I2C sensors.
    /// </summary>
    public abstract class NxtI2cSensor : NxtSensor
    {
        #region Private variables


        #endregion

        #region Constructor

        public NxtI2cSensor(byte address) : base(NxtSensorTypes.LowSpeed9V, NxtSensorModes.Raw)
        {
            // Save address.
            Address = address;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the address of the sensor.
        /// </summary>
        public byte Address { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Attaches the sensor to an <see cref="NxtBrick"/>.
        /// </summary>
        /// <param name="brick">The <see cref="NxtBrick"/> to use.</param>
        /// <param name="port">The port the sensor is on.</param>
        internal override void Attach(NxtBrick brick, NxtInputPorts port)
        {
            // Call base method.
            base.Attach(brick, port);

            // Clear out sensor.
            byte? bytesReady;
            try
            {
                bytesReady = brick.LsGetStatus(Port);
            }
            catch (Exception)
            {

                throw;
            }

            byte[] test = (bytesReady?.CompareTo(0) > 0) ? brick.LsRead(Port) : null;
        }

        internal byte[] SendBytes(byte[] data, byte resultLength)
        {
            // Send request to sensor.
            brick.LsWrite(Port, data, resultLength);

            // If result length is nothing, return null.
            if (resultLength == 0)
                return null;

            // Wait until the result is ready.
            byte? bytesReady = 0;
            do
            {
                try
                {
                    // Get how many bytes are ready.
                    Thread.Sleep(10);
                    bytesReady = brick.LsGetStatus(Port);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            while (bytesReady < resultLength);

            return brick.LsRead(Port);
        }

        /// <summary>
        /// Reads the version from the sensor.
        /// </summary>
        public string ReadVersion()
        {
            // Get version.
            var result = SendBytes(new byte[] { Address, 0x00 }, 8);
            return (result != null) ? Encoding.ASCII.GetString(result, 0, result.Length).TrimEnd('\0', '?', ' ') : string.Empty;
        }

        /// <summary>
        /// Reads the product ID from the sensor.
        /// </summary>
        public string ReadProductId()
        {
            // Get product ID.
            var result = SendBytes(new byte[] { Address, 0x08}, 8);
            return (result != null) ? Encoding.ASCII.GetString(result, 0, result.Length).TrimEnd('\0', '?', ' ') : string.Empty;
        }

        /// <summary>
        /// Reads the sensor type from the sensor.
        /// </summary>
        public string ReadSensorType()
        {
            // Get type.
            var result = SendBytes(new byte[] { Address, 0x10 }, 8);
            return (result != null) ? Encoding.ASCII.GetString(result, 0, result.Length).TrimEnd('\0', '?', ' ') : string.Empty;
        }

        #endregion
    }
}