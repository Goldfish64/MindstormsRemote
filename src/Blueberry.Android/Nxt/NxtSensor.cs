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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Blueberry.Nxt
{
    /// <summary>
    /// Represents the base class for NXT sensors.
    /// </summary>
    public abstract class NxtSensor : NxtDevice
    {
        #region Protected variables

        private NxtInputPorts port;
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

        protected NxtSensorTypes Type
        {
            get { return type; }
            set
            {
                // Update sensor.
                type = value;
                brick.SetInputMode(port, type, mode);
            }
        }

        protected NxtSensorModes Mode
        {
            get { return mode; }
            set
            {
                // Update sensor.
                mode = value;
                brick.SetInputMode(port, Type, mode);
            }
        }

        protected NxtGetInputValuesResponse? PollingData { get; set; }

        /// <summary>
        /// Gets the raw sensor value from the last poll. A null value indicates the sensor has not yet been polled.
        /// </summary>
        public ushort? RawValue => PollingData?.RawValue;

        #endregion

        #region Methods

        /// <summary>
        /// Attaches the sensor to an <see cref="NxtBrick"/>.
        /// </summary>
        /// <param name="brick">The <see cref="NxtBrick"/> to use.</param>
        /// <param name="port">The port the sensor is on.</param>
        internal void Attach(NxtBrick brick, NxtInputPorts port)
        {
            // Save brick and port.
            this.brick = brick;
            this.port = port;

            // Update sensor.
            brick.SetInputMode(port, Type, mode);
        }

        /// <summary>
        /// Polls the sensor.
        /// </summary>
        public override void Poll()
        {
            // Get sensor information.
            PollingData = brick.GetInputValues(port);
            base.Poll();
        }

        #endregion
    }
}