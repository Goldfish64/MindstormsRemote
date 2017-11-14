/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: NxtLightSensor.cs
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
    /// Represents an NXT light sensor.
    /// </summary>
    public class NxtLightSensor : NxtSensor
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NxtLightSensor"/> class.
        /// </summary>
        /// <param name="generateLight">Whether or not the sensor's LED is on or not</param>
        public NxtLightSensor(bool generateLight = false) :
            base(generateLight ? NxtSensorTypes.LightActive : NxtSensorTypes.LightInactive, NxtSensorModes.PercentFullScale)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets if the sensor's LED is on or not.
        /// </summary>
        public bool GenerateLight
        {
            get { return Type == NxtSensorTypes.LightActive; }
            set { Type = value ? NxtSensorTypes.LightActive : NxtSensorTypes.LightInactive; }
        }

        /// <summary>
        /// Gets the scaled value from the sensor since the last poll.
        /// A null value indicates the sensor hasn't been polled yet.
        /// </summary>
        public short? Intensity => PollingData?.ScaledValue;

        #endregion
    }
}
