/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: NxtSoundSensor.cs
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
    public class NxtSoundSensor : NxtAnalogSensor
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NxtSoundSensor"/> class.
        /// </summary>
        /// <param name="active">Whether or not the sensor should use dBA mode.</param>
        public NxtSoundSensor(bool dBA) :
            base(dBA ? NxtSensorTypes.SoundDBA : NxtSensorTypes.SoundDB, NxtSensorModes.PercentFullScale)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether to run the sensor in dBA mode or dB mode.
        /// </summary>
        public bool dBA
        {
            get { return Type == NxtSensorTypes.SoundDBA; }
            set { Type = value ? NxtSensorTypes.SoundDBA : NxtSensorTypes.SoundDB; }
        }

        public short? SoundLevel => PollingData?.ScaledValue;

        /// <summary>
        /// Gets the sensor's value as a string.
        /// </summary>
        public override string Value => SoundLevel?.ToString();

        /// <summary>
        /// Gets the sensor's friendly name.
        /// </summary>
        public override string FriendlyName => dBA ? "Sound (dBA)" : "Sound (dB)";

        #endregion
    }
}