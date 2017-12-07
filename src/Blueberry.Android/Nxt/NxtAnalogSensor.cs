/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: NxtAnalogSensor.cs
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

namespace Blueberry.Nxt
{
    /// <summary>
    /// Represents the base class for analog NXT sensors.
    /// </summary>
    public abstract class NxtAnalogSensor : NxtSensor
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NxtAnalogSensor"/> class with the specified type and mode.
        /// </summary>
        /// <param name="type">The type of sensor.</param>
        /// <param name="mode">The sensor mode.</param>
        public NxtAnalogSensor(NxtSensorTypes type, NxtSensorModes mode) : base(type, mode)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the polling data.
        /// </summary>
        protected NxtGetInputValuesResponse? PollingData { get; set; }

        /// <summary>
        /// Gets the raw sensor value from the last poll. A null value indicates the sensor has not yet been polled.
        /// </summary>
        public ushort? RawValue => PollingData?.RawValue;

        #endregion

        #region Methods

        /// <summary>
        /// Polls the sensor.
        /// </summary>
        public override void Poll()
        {
            // Get sensor information.
            PollingData = brick.GetInputValues(Port);
            base.Poll();
        }

        #endregion
    }
}
