﻿/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: SensorsAdapter.cs
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

using Android.App;
using Android.Views;
using Android.Widget;
using System;

namespace MindstormsRemote.Framework
{
    /// <summary>
    /// Represents the adapter used for sensors.
    /// </summary>
    public class SensorsAdapter : BaseAdapter
    {
        #region Private variables

        private Array sensors;
        private Activity activity;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorsAdapter"/> class.
        /// </summary>
        public SensorsAdapter(Activity activity)
        {
            // Initialize variables.
            this.activity = activity;
            sensors = Enum.GetValues(typeof(Sensors));
        }

        #endregion

        #region Properties

        public override int Count
        {
            get { return sensors.Length; }
        }

        #endregion

        #region Methods

        private string GetFriendlyName(Sensors sensor)
        {
            switch (sensor)
            {
                case Sensors.None:
                    return "None";

                case Sensors.Touch:
                    return "Touch";

                case Sensors.SoundDB:
                    return "Sound (dB)";

                case Sensors.SoundDBA:
                    return "Sound (dBA)";

                case Sensors.LightActive:
                    return "Light (active)";

                case Sensors.LightInactive:
                    return "Light (inactive)";

                case Sensors.ColorFull:
                    return "Color";

                case Sensors.ColorRed:
                    return "Color (red light)";

                case Sensors.ColorGreen:
                    return "Color (green light)";

                case Sensors.ColorBlue:
                    return "Color (blue light)";

                case Sensors.ColorInactive:
                    return "Color (inactive light)";

                case Sensors.Ultrasonic:
                    return "Ultrasonic";

                default:
                    return null;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return sensors.GetValue(position) as Java.Lang.Object;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (position >= sensors.Length)
                position = 0;

            // Get item and view.
            var value = sensors.GetValue(position);
            var view = (convertView ?? activity.LayoutInflater.Inflate(Android.Resource.Layout.SimpleSpinnerDropDownItem, parent, false)) as TextView;

            // Get name.
            if (Enum.IsDefined(typeof(Sensors), value))
                view.Text = GetFriendlyName((Sensors)value);

            // Return the view.
            return view;
        }

        #endregion
    }
}
