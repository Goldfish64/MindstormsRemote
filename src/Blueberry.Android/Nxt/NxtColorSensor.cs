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
    /// Represents an NXT color sensor.
    /// </summary>
    public class NxtColorSensor : NxtSensor
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

    public enum NxtColorSensorValues
    {
        Black = 1,
        Blue = 2,
        Green = 3,
        Yellow = 4,
        Red = 5,
        White = 6
    }
}