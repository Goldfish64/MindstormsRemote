/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: NxtSensor.cs
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

namespace Blueberry.Nxt
{
    /// <summary>
    /// Represents the base class for NXT sensors.
    /// </summary>
    public abstract class NxtSensor : NxtDevice
    {
        #region Protected variables

        private NxtSensorTypes type;
        private NxtSensorModes mode;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NxtSensor"/> class with the specified type and mode.
        /// </summary>
        /// <param name="type">The type of sensor.</param>
        /// <param name="mode">The sensor mode.</param>
        public NxtSensor(NxtSensorTypes type, NxtSensorModes mode)
        {
            this.type = type;
            this.mode = mode;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        internal NxtInputPorts Port { get; set; }

        /// <summary>
        /// Gets or sets the sensor type.
        /// </summary>
        protected NxtSensorTypes Type
        {
            get { return type; }
            set
            {
                // Update sensor.
                type = value;
                brick?.SetInputMode(Port, type, mode);
            }
        }

        /// <summary>
        /// Gets or sets the sensor mode.
        /// </summary>
        protected NxtSensorModes Mode
        {
            get { return mode; }
            set
            {
                // Update sensor.
                mode = value;
                brick?.SetInputMode(Port, Type, mode);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attaches the sensor to an <see cref="NxtBrick"/>.
        /// </summary>
        /// <param name="brick">The <see cref="NxtBrick"/> to use.</param>
        /// <param name="port">The port the sensor is on.</param>
        internal virtual void Attach(NxtBrick brick, NxtInputPorts port)
        {
            // Save brick and port.
            this.brick = brick ?? throw new ArgumentNullException(nameof(brick));
            Port = port;

            // Update sensor.
            brick.SetInputMode(port, Type, mode);
        }

        /// <summary>
        /// Detaches the sensor.
        /// </summary>
        internal virtual void Detach()
        {
            PollingInterval = 0;
            brick = null;
            Port = 0;
        }

        /// <summary>
        /// Disconnects the sensor. Used when the brick is being disconnected.
        /// </summary>
        internal virtual void Disconnect()
        {
            // Stop polling and set sensor to none.
            PollingInterval = 0;
            brick.SetInputMode(Port, NxtSensorTypes.None, NxtSensorModes.Raw);
        }

        #endregion
    }
}
