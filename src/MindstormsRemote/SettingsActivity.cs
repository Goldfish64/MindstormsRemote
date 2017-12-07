/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: SettingsActivity.cs
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
using Android.Support.V7.App;
using Android.Preferences;
using Blueberry.Nxt;
using MindstormsRemote.Framework;

namespace MindstormsRemote
{
    [Activity(Theme = "@style/Theme.AppCompat")]
    public class SettingsActivity : AppCompatActivity
    {
        #region Private variables

        private ISharedPreferencesEditor prefEditor;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Call base method.
            base.OnCreate(savedInstanceState);

            // Load the layout into view.
            SetContentView(Resource.Layout.SettingsPage);

            // Load up prefernces.
            var preferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            prefEditor = preferences.Edit();

            // Populate spinners.
            var spinSensor1 = FindViewById<Spinner>(Resource.Id.SpinSensor1);
            spinSensor1.Adapter = new SensorsAdapter(this);
            spinSensor1.Tag = Constants.PrefSensor1Type;
            spinSensor1.SetSelection(preferences.GetInt(Constants.PrefSensor1Type, (int)Sensors.None));
            spinSensor1.ItemSelected += OnSensorSelected;

            var spinSensor2 = FindViewById<Spinner>(Resource.Id.SpinSensor2);
            spinSensor2.Adapter = new SensorsAdapter(this);
            spinSensor2.Tag = Constants.PrefSensor2Type;
            spinSensor2.SetSelection(preferences.GetInt(Constants.PrefSensor2Type, (int)Sensors.None));
            spinSensor2.ItemSelected += OnSensorSelected;

            var spinSensor3 = FindViewById<Spinner>(Resource.Id.SpinSensor3);
            spinSensor3.Adapter = new SensorsAdapter(this);
            spinSensor3.Tag = Constants.PrefSensor3Type;
            spinSensor3.SetSelection(preferences.GetInt(Constants.PrefSensor3Type, (int)Sensors.None));
            spinSensor3.ItemSelected += OnSensorSelected;

            var spinSensor4 = FindViewById<Spinner>(Resource.Id.SpinSensor4);
            spinSensor4.Adapter = new SensorsAdapter(this);
            spinSensor4.Tag = Constants.PrefSensor4Type;
            spinSensor4.SetSelection(preferences.GetInt(Constants.PrefSensor4Type, (int)Sensors.None));
            spinSensor4.ItemSelected += OnSensorSelected;
        }

        private void OnSensorSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            // Save sensor type.
            var spinner = sender as Spinner;
            if (spinner != null)
            {
                prefEditor.PutInt(spinner.Tag.ToString(), spinner.SelectedItemPosition);
                prefEditor.Commit();
            }
        }
    }
}