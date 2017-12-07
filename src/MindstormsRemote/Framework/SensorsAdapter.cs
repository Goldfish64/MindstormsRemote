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
            var view = (convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.TextViewItem, parent, false)) as TextView;

            // Get name.
            if (Enum.IsDefined(typeof(Sensors), value))
                view.Text = GetFriendlyName((Sensors)value);

            // Return the view.
            return view;
        }

        #endregion
    }
}