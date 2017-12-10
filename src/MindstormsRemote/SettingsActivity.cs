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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Widget;
using MindstormsRemote.Framework;
using System;

namespace MindstormsRemote
{
    /// <summary>
    /// Represents the settings activity.
    /// </summary>
    [Activity(Label = "Settings")]
    public class SettingsActivity : AppCompatActivity
    {
        #region Private variables

        private ISharedPreferencesEditor prefEditor;
        private RadioButton rbnMotorLPortA;
        private RadioButton rbnMotorLPortB;
        private RadioButton rbnMotorLPortC;
        private RadioButton rbnMotorRPortA;
        private RadioButton rbnMotorRPortB;
        private RadioButton rbnMotorRPortC;
        private SeekBar seekDriveMotorPower;
        private TextView txtDriveMotorPower;

        #endregion

        #region Methods

        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Call base method.
            base.OnCreate(savedInstanceState);

            // Load the layout into view.
            SetContentView(Resource.Layout.SettingsPage);

            // Add toolbar to view.
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.ToolbarSettings);
            SetSupportActionBar(toolbar);

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

            // Get motor radio buttons.
            rbnMotorLPortA = FindViewById<RadioButton>(Resource.Id.RbnMotorLPortA);
            rbnMotorLPortA.Click += OnMotorRadioButtonClick;
            rbnMotorLPortB = FindViewById<RadioButton>(Resource.Id.RbnMotorLPortB);
            rbnMotorLPortB.Click += OnMotorRadioButtonClick;
            rbnMotorLPortC = FindViewById<RadioButton>(Resource.Id.RbnMotorLPortC);
            rbnMotorLPortC.Click += OnMotorRadioButtonClick;
            rbnMotorRPortA = FindViewById<RadioButton>(Resource.Id.RbnMotorRPortA);
            rbnMotorRPortA.Click += OnMotorRadioButtonClick;
            rbnMotorRPortB = FindViewById<RadioButton>(Resource.Id.RbnMotorRPortB);
            rbnMotorRPortB.Click += OnMotorRadioButtonClick;
            rbnMotorRPortC = FindViewById<RadioButton>(Resource.Id.RbnMotorRPortC);
            rbnMotorRPortC.Click += OnMotorRadioButtonClick;

            // Get left motor setting.
            var motorLeftPort = preferences.GetInt(Constants.PrefMotorLPort, Constants.PrefValueMotorPortB);
            switch (motorLeftPort)
            {
                case Constants.PrefValueMotorPortA:
                    rbnMotorLPortA.Checked = true;
                    rbnMotorLPortB.Checked = rbnMotorLPortC.Checked = false;
                    break;

                case Constants.PrefValueMotorPortC:
                    rbnMotorLPortC.Checked = true;
                    rbnMotorLPortA.Checked = rbnMotorLPortB.Checked = false;
                    break;

                default:
                    rbnMotorLPortB.Checked = true;
                    rbnMotorLPortA.Checked = rbnMotorLPortC.Checked = false;
                    break;
            }

            // Get right motor and ensure its not the same as left.
            var motorRightPort = preferences.GetInt(Constants.PrefMotorRPort, Constants.PrefValueMotorPortC);
            if (motorRightPort == motorLeftPort)
                motorRightPort = Constants.PrefValueMotorPortC;

            // Set radio buttons for right motor.
            switch (motorRightPort)
            {
                case Constants.PrefValueMotorPortA:
                    rbnMotorRPortA.Checked = true;
                    rbnMotorRPortB.Checked = rbnMotorRPortC.Checked = false;
                    break;

                case Constants.PrefValueMotorPortB:
                    rbnMotorRPortB.Checked = true;
                    rbnMotorRPortA.Checked = rbnMotorRPortC.Checked = false;
                    break;

                default:
                    rbnMotorRPortC.Checked = true;
                    rbnMotorRPortA.Checked = rbnMotorRPortB.Checked = false;
                    break;
            }

            // Save motor settings.
            prefEditor.PutInt(Constants.PrefMotorLPort, motorLeftPort);
            prefEditor.PutInt(Constants.PrefMotorRPort, motorRightPort);
            prefEditor.Commit();

            // Get drive motor power setting.
            var driveMotorPower = preferences.GetInt(Constants.PrefMotorDrivePower, 75);
            if (driveMotorPower > 100)
                driveMotorPower = 100;
            else if (driveMotorPower < 0)
                driveMotorPower = 0;
            prefEditor.PutInt(Constants.PrefMotorDrivePower, driveMotorPower);
            prefEditor.Commit();

            // Get seekbar for drive motor power.
            seekDriveMotorPower = FindViewById<SeekBar>(Resource.Id.SeekMotorDrivePower);
            seekDriveMotorPower.Progress = driveMotorPower;
            seekDriveMotorPower.ProgressChanged += OnDriveMotorPowerProgressChanged;
            txtDriveMotorPower = FindViewById<TextView>(Resource.Id.TxtMotorDrivePower);
            txtDriveMotorPower.Text = string.Format(Constants.MotorDrivePowerFormat, driveMotorPower);

            // Get drive motor brake setting.
            var chkBrakeDriveMotors = FindViewById<CheckBox>(Resource.Id.ChkBrakeDriveMotors);
            chkBrakeDriveMotors.Checked = preferences.GetBoolean(Constants.PrefMotorBrakeDrive, false);
            chkBrakeDriveMotors.CheckedChange += OnBrakeDriveMotorsCheckChanged;
        }

        /// <summary>
        /// Handles the Selected event of the sensor spinners.
        /// </summary>
        private void OnSensorSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            // Save sensor type.
            if (sender is Spinner spinner)
            {
                prefEditor.PutInt(spinner.Tag.ToString(), spinner.SelectedItemPosition);
                prefEditor.Commit();
            }
        }

        /// <summary>
        /// Handles the Click event of the motor radio buttons.
        /// </summary>
        private void OnMotorRadioButtonClick(object sender, EventArgs e)
        {
            // Ensure only one motor is attached to a port.
            if (rbnMotorLPortA.Checked && rbnMotorRPortA.Checked)
            {
                if (sender == rbnMotorLPortA)
                {
                    rbnMotorRPortA.Checked = false;
                    rbnMotorRPortB.Checked = true;
                }
                else if (sender == rbnMotorRPortA)
                {
                    rbnMotorLPortA.Checked = false;
                    rbnMotorLPortB.Checked = true;
                }
            }
            else if (rbnMotorLPortB.Checked && rbnMotorRPortB.Checked)
            {
                if (sender == rbnMotorLPortB)
                {
                    rbnMotorRPortB.Checked = false;
                    rbnMotorRPortC.Checked = true;
                }
                else if (sender == rbnMotorRPortB)
                {
                    rbnMotorLPortB.Checked = false;
                    rbnMotorLPortC.Checked = true;
                }
            }
            else if (rbnMotorLPortC.Checked && rbnMotorRPortC.Checked)
            {
                if (sender == rbnMotorLPortC)
                {
                    rbnMotorRPortC.Checked = false;
                    rbnMotorRPortB.Checked = true;
                }
                else if (sender == rbnMotorRPortC)
                {
                    rbnMotorLPortC.Checked = false;
                    rbnMotorLPortB.Checked = true;
                }
            }

            // Save motor settings.
            if (rbnMotorLPortA.Checked)
                prefEditor.PutInt(Constants.PrefMotorLPort, Constants.PrefValueMotorPortA);
            else if (rbnMotorLPortB.Checked)
                prefEditor.PutInt(Constants.PrefMotorLPort, Constants.PrefValueMotorPortB);
            else if (rbnMotorLPortC.Checked)
                prefEditor.PutInt(Constants.PrefMotorLPort, Constants.PrefValueMotorPortC);

            if (rbnMotorRPortA.Checked)
                prefEditor.PutInt(Constants.PrefMotorRPort, Constants.PrefValueMotorPortA);
            else if (rbnMotorRPortB.Checked)
                prefEditor.PutInt(Constants.PrefMotorRPort, Constants.PrefValueMotorPortB);
            else if (rbnMotorRPortC.Checked)
                prefEditor.PutInt(Constants.PrefMotorRPort, Constants.PrefValueMotorPortC);

            // Commit changes.
            prefEditor.Commit();
        }

        /// <summary>
        /// Handles the ProgressChanged event of the drive motor power seekbar.
        /// </summary>
        private void OnDriveMotorPowerProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            // Save power level.
            txtDriveMotorPower.Text = string.Format(Constants.MotorDrivePowerFormat, seekDriveMotorPower.Progress);
            prefEditor.PutInt(Constants.PrefMotorDrivePower, seekDriveMotorPower.Progress);
            prefEditor.Commit();
        }

        /// <summary>
        /// Handles the CheckChanged event for the brake drive motors checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBrakeDriveMotorsCheckChanged(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            // Save brake setting.
            prefEditor.PutBoolean(Constants.PrefMotorBrakeDrive, e.IsChecked);
            prefEditor.Commit();
        }

        #endregion
    }
}
