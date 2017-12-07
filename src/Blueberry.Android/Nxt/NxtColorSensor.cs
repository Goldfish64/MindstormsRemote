/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: NxtColorSensor.cs
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
    /// Represents an NXT color sensor.
    /// </summary>
    public class NxtColorSensor : NxtAnalogSensor
    {
        #region Private variables

        private NxtColorSensorModes detectionMode;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NxtColorSensor"/> class.
        /// </summary>
        /// <param name="generateLight">Whether or not the sensor's LED is on or not</param>
        public NxtColorSensor() :
            base(NxtSensorTypes.ColorFull, NxtSensorModes.Raw)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the detection mode of the sensor.
        /// </summary>
        public NxtColorSensorModes DetectionMode
        {
            get { return detectionMode; }
            set
            {
                if (value == detectionMode)
                    return;
                detectionMode = value;
                switch (detectionMode)
                {
                    case NxtColorSensorModes.LightRed:
                        Type = NxtSensorTypes.ColorRed;
                        break;

                    case NxtColorSensorModes.LightGreen:
                        Type = NxtSensorTypes.ColorGreen;
                        break;

                    case NxtColorSensorModes.LightBlue:
                        Type = NxtSensorTypes.ColorBlue;
                        break;

                    case NxtColorSensorModes.LightPassive:
                        Type = NxtSensorTypes.ColorNone;
                        break;

                    default:
                        detectionMode = NxtColorSensorModes.Color;
                        Type = NxtSensorTypes.ColorFull;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the light intensity since the last poll of the sensor. A null value indicates the sensor has either not
        /// yet been polled or is not in a light sensing mode.
        /// </summary>
        public short? Intensity => detectionMode != NxtColorSensorModes.Color ? PollingData?.ScaledValue : null;

        /// <summary>
        /// Gets the color detected by the sensor.
        /// </summary>
        public NxtColorSensorValues? Color
        {
            get
            {
                if (detectionMode == NxtColorSensorModes.Color && Enum.IsDefined(typeof(NxtColorSensorValues), PollingData?.ScaledValue))
                    return (NxtColorSensorValues)PollingData?.ScaledValue;
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the sensor's value as a string.
        /// </summary>
        public override string Value
        {
            get
            {
                if (detectionMode == NxtColorSensorModes.Color)
                    return Enum.GetName(typeof(NxtColorSensorValues), Color);
                else
                    return Intensity != null ? (Intensity / 10).ToString() + "%" : null;
            }
        }

        /// <summary>
        /// Gets the sensor's friendly name.
        /// </summary>
        public override string FriendlyName
        {
            get
            {
                switch (detectionMode)
                {
                    case NxtColorSensorModes.LightRed:
                        return "Color (red light)";

                    case NxtColorSensorModes.LightGreen:
                        return "Color (green light)";

                    case NxtColorSensorModes.LightBlue:
                        return "Color (blue light)";

                    case NxtColorSensorModes.LightPassive:
                        return "Color (inactive light)";

                    default:
                        return "Color";
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Specifies the detection mode for the NXT color sensor.
    /// </summary>
    public enum NxtColorSensorModes
    {
        /// <summary>
        /// Sensor is in full color mode.
        /// </summary>
        Color,

        /// <summary>
        /// Sensor is in active light detection mode using the red LED.
        /// </summary>
        LightRed,

        /// <summary>
        /// Sensor is in active light detection mode using the green LED.
        /// </summary>
        LightGreen,

        /// <summary>
        /// Sensor is in active light detection mode using the blue LED.
        /// </summary>
        LightBlue,

        /// <summary>
        /// Sensor is in passive light detection mode.
        /// </summary>
        LightPassive
    }

    public enum NxtColorSensorValues : short
    {
        Black = 1,
        Blue = 2,
        Green = 3,
        Yellow = 4,
        Red = 5,
        White = 6
    }
}