/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: ControllerActivity.cs
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
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Blueberry.Nxt;
using MindstormsRemote.Framework;
using System;
using System.Threading.Tasks;

namespace MindstormsRemote
{
    /// <summary>
    /// Represents the controller activity.
    /// </summary>
    [Activity]
    public class ControllerActivity : AppCompatActivity
    {
        #region Private variables

        private BluetoothDevice device;
        private NxtBrick brick;
        private NxtMotor motorL;
        private NxtMotor motorR;

        private TextView txtSensor1;
        private TextView txtSensor2;
        private TextView txtSensor3;
        private TextView txtSensor4;
        private TextView txtMotors;

        private byte powerLevel = 75;
        private bool brakeDriveMotors = false;

        #endregion

        #region Methods

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            // Call base method.
            base.OnCreate(savedInstanceState);

            // Load the layout into view.
            SetContentView(Resource.Layout.ControllerPage);

            // Add toolbar to view.
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.ToolbarController);
            toolbar.Title = ""; // https://stackoverflow.com/a/35430590.
            SetSupportActionBar(toolbar);

            // Wire up controller button events.
            FindViewById<Button>(Resource.Id.BtnDriveNW).Touch += OnControllerButtonTouch;
            FindViewById<Button>(Resource.Id.BtnDriveN).Touch += OnControllerButtonTouch;
            FindViewById<Button>(Resource.Id.BtnDriveNE).Touch += OnControllerButtonTouch;
            FindViewById<Button>(Resource.Id.BtnDriveW).Touch += OnControllerButtonTouch;
            FindViewById<Button>(Resource.Id.BtnDriveE).Touch += OnControllerButtonTouch;
            FindViewById<Button>(Resource.Id.BtnDriveSW).Touch += OnControllerButtonTouch;
            FindViewById<Button>(Resource.Id.BtnDriveS).Touch += OnControllerButtonTouch;
            FindViewById<Button>(Resource.Id.BtnDriveSE).Touch += OnControllerButtonTouch;

            // Get widget references..
            txtSensor1 = FindViewById<TextView>(Resource.Id.TxtSensor1);
            txtSensor2 = FindViewById<TextView>(Resource.Id.TxtSensor2);
            txtSensor3 = FindViewById<TextView>(Resource.Id.TxtSensor3);
            txtSensor4 = FindViewById<TextView>(Resource.Id.TxtSensor4);
            txtMotors = FindViewById<TextView>(Resource.Id.TxtMotors);

            // Clear text fields.
            txtSensor1.Text = "1: None";
            txtSensor2.Text = "2: None";
            txtSensor3.Text = "3: None";
            txtSensor4.Text = "4: None";

            await Task.Run(() =>
            {
                // Get MAC address passed.
                var address = Intent.GetStringExtra(Constants.BluetoothAddressExtra);
                device = BluetoothAdapter.DefaultAdapter.GetRemoteDevice(address);

                // Change title to device name.
                toolbar.Title = device.Name;

                // Initialize NXT brick.
                brick = new NxtBrick(device);
                brick.Polled += OnBrickPolled;
                brick.Disconnected += OnBrickDisconnected;

                brick.Connect();
                brick.PlayTone(1000, 100);
                motorL = new NxtMotor();
                motorR = new NxtMotor();
                brick.PollingInterval = 1000;

                // Load up prefernces.
                var preferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);

                // Configure sensor on port 1.
                var sensor1 = preferences.GetInt(Constants.PrefSensor1Type, (int)Sensors.None);
                brick.Sensor1 = CreateSensor((Sensors)sensor1);
                if (brick.Sensor1 != null)
                {
                    brick.Sensor1.Polled += OnSensor1Polled;
                    brick.Sensor1.PollingInterval = 500;
                }

                // Configure sensor on port 2.
                var sensor2 = preferences.GetInt(Constants.PrefSensor2Type, (int)Sensors.None);
                brick.Sensor2 = CreateSensor((Sensors)sensor2);
                if (brick.Sensor2 != null)
                {
                    brick.Sensor2.Polled += OnSensor2Polled;
                    brick.Sensor2.PollingInterval = 500;
                }

                // Configure sensor on port 3.
                var sensor3 = preferences.GetInt(Constants.PrefSensor3Type, (int)Sensors.None);
                brick.Sensor3 = CreateSensor((Sensors)sensor3);
                if (brick.Sensor3 != null)
                {
                    brick.Sensor3.Polled += OnSensor3Polled;
                    brick.Sensor3.PollingInterval = 500;
                }

                // Configure sensor on port 4.
                var sensor4 = preferences.GetInt(Constants.PrefSensor4Type, (int)Sensors.None);
                brick.Sensor4 = CreateSensor((Sensors)sensor4);
                if (brick.Sensor4 != null)
                {
                    brick.Sensor4.Polled += OnSensor4Polled;
                    brick.Sensor4.PollingInterval = 500;
                }

                // Configure motors.
                // Get left motor setting.
                var motorLeftPort = preferences.GetInt(Constants.PrefMotorLPort, Constants.PrefValueMotorPortB);
                switch (motorLeftPort)
                {
                    case Constants.PrefValueMotorPortA:
                        brick.MotorA = motorL;
                        break;

                    case Constants.PrefValueMotorPortC:
                        brick.MotorC = motorL;
                        break;

                    default:
                        brick.MotorB = motorL;
                        break;
                }

                // Get right motor and ensure its not the same as left.
                var motorRightPort = preferences.GetInt(Constants.PrefMotorRPort, Constants.PrefValueMotorPortC);
                if (motorRightPort == motorLeftPort)
                    motorRightPort = Constants.PrefValueMotorPortC;

                // Assign right motor.
                switch (motorRightPort)
                {
                    case Constants.PrefValueMotorPortA:
                        brick.MotorA = motorR;
                        break;

                    case Constants.PrefValueMotorPortB:
                        brick.MotorB = motorR;
                        break;

                    default:
                        brick.MotorC = motorR;
                        break;
                }

                // Get drive motors brake setting.
                brakeDriveMotors = preferences.GetBoolean(Constants.PrefMotorBrakeDrive, false);
                if (brakeDriveMotors)
                {
                    motorL.Off();
                    motorR.Off();
                }

                // Update motor TextView.
                RunOnUiThread(() => txtMotors.Text = $"Left motor: {GetMotorPortString(motorLeftPort)}\nRight motor: {GetMotorPortString(motorRightPort)}");
            });

            // Show UI.
            FindViewById<ProgressBar>(Resource.Id.ControllerProgressBar).Visibility = ViewStates.Gone;
            FindViewById<RelativeLayout>(Resource.Id.RootControllerLayout).Visibility = ViewStates.Visible;
        }

        /// <summary>
        /// Called after Android.App.Activity.OnRestoreInstanceState(Android.OS.Bundle),
        /// Android.App.Activity.OnRestart, or Android.App.Activity.OnPause, for your activity
        /// to start interacting with the user.
        /// </summary>
        protected override void OnResume()
        {
            // Call base method.
            base.OnResume();

            // If brick is null, return.
            if (brick == null)
                return;

            // Load up prefernces.
            var preferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);

            // Configure sensor on port 1.
            var sensor1 = preferences.GetInt(Constants.PrefSensor1Type, (int)Sensors.None);
            brick.Sensor1 = CreateSensor((Sensors)sensor1);
            if (brick.Sensor1 != null)
            {
                brick.Sensor1.Polled += OnSensor1Polled;
                brick.Sensor1.PollingInterval = 500;
            }

            // Configure sensor on port 2.
            var sensor2 = preferences.GetInt(Constants.PrefSensor2Type, (int)Sensors.None);
            brick.Sensor2 = CreateSensor((Sensors)sensor2);
            if (brick.Sensor2 != null)
            {
                brick.Sensor2.Polled += OnSensor2Polled;
                brick.Sensor2.PollingInterval = 500;
            }

            // Configure sensor on port 3.
            var sensor3 = preferences.GetInt(Constants.PrefSensor3Type, (int)Sensors.None);
            brick.Sensor3 = CreateSensor((Sensors)sensor3);
            if (brick.Sensor3 != null)
            {
                brick.Sensor3.Polled += OnSensor3Polled;
                brick.Sensor3.PollingInterval = 500;
            }

            // Configure sensor on port 4.
            var sensor4 = preferences.GetInt(Constants.PrefSensor4Type, (int)Sensors.None);
            brick.Sensor4 = CreateSensor((Sensors)sensor4);
            if (brick.Sensor4 != null)
            {
                brick.Sensor4.Polled += OnSensor4Polled;
                brick.Sensor4.PollingInterval = 500;
            }

            // Clear text fields.
            if (brick.Sensor1 == null)
                txtSensor1.Text = "1: None";
            if (brick.Sensor2 == null)
                txtSensor2.Text = "2: None";
            if (brick.Sensor3 == null)
                txtSensor3.Text = "3: None";
            if (brick.Sensor4 == null)
                txtSensor4.Text = "4: None";

            // Clear motors.
            brick.MotorA = null;
            brick.MotorB = null;
            brick.MotorC = null;

            // Configure motors.
            // Get left motor setting.
            var motorLeftPort = preferences.GetInt(Constants.PrefMotorLPort, Constants.PrefValueMotorPortB);
            switch (motorLeftPort)
            {
                case Constants.PrefValueMotorPortA:
                    brick.MotorA = motorL;
                    break;

                case Constants.PrefValueMotorPortC:
                    brick.MotorC = motorL;
                    break;

                default:
                    brick.MotorB = motorL;
                    break;
            }

            // Get right motor and ensure its not the same as left.
            var motorRightPort = preferences.GetInt(Constants.PrefMotorRPort, Constants.PrefValueMotorPortC);
            if (motorRightPort == motorLeftPort)
                motorRightPort = Constants.PrefValueMotorPortC;

            // Assign right motor.
            switch (motorRightPort)
            {
                case Constants.PrefValueMotorPortA:
                    brick.MotorA = motorR;
                    break;

                case Constants.PrefValueMotorPortB:
                    brick.MotorB = motorR;
                    break;

                default:
                    brick.MotorC = motorR;
                    break;
            }

            // Get drive motors brake setting.
            brakeDriveMotors = preferences.GetBoolean(Constants.PrefMotorBrakeDrive, false);
            if (brakeDriveMotors)
            {
                motorL.Off();
                motorR.Off();
            }
            else
            {
                motorL.Coast();
                motorR.Coast();
            }
        }

        /// <summary>
        /// Creates an <see cref="NxtSensor"/> object.
        /// </summary>
        /// <param name="sensor"></param>
        private NxtSensor CreateSensor(Sensors sensor)
        {
            switch (sensor)
            {
                case Sensors.Touch:
                    return new NxtTouchSensor();

                case Sensors.ColorFull:
                    return new NxtColorSensor() { DetectionMode = NxtColorSensorModes.Color };

                case Sensors.ColorRed:
                    return new NxtColorSensor() { DetectionMode = NxtColorSensorModes.LightRed };

                case Sensors.ColorGreen:
                    return new NxtColorSensor() { DetectionMode = NxtColorSensorModes.LightGreen };

                case Sensors.ColorBlue:
                    return new NxtColorSensor() { DetectionMode = NxtColorSensorModes.LightBlue };

                case Sensors.ColorInactive:
                    return new NxtColorSensor() { DetectionMode = NxtColorSensorModes.LightPassive };

                case Sensors.Ultrasonic:
                    return new NxtUltrasonicSensor();

                default:
                    return null;
            }
        }

        private string GetMotorPortString(int port)
        {
            switch (port)
            {
                case Constants.PrefValueMotorPortA:
                    return "Port A";

                case Constants.PrefValueMotorPortB:
                    return "Port B";

                case Constants.PrefValueMotorPortC:
                    return "Port C";

                default:
                    return null;
            }
        }

        protected override void OnDestroy()
        {
            // Clean up brick.
            brick.Dispose();

            // Call base method.
            base.OnDestroy();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //change main_compat_menu
            MenuInflater.Inflate(Resource.Menu.ControllerMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.settings)
            {
                StartActivity(new Intent(this, typeof(SettingsActivity)));
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        /// <summary>
        /// Handles the Touch event of the controller buttons.
        /// </summary>
        private void OnControllerButtonTouch(object sender, View.TouchEventArgs e)
        {
            // Get button.
            var button = sender as Button;
            if (button == null)
                return;

            // http://www.robotappstore.com/Knowledge-Base/-How-to-Control-Lego-NXT-Motors/81.html
            if (e.Event.Action == MotionEventActions.Down)
            {
                // Button was pressed.
                button.Pressed = true;
                switch (button.Id)
                {
                    case Resource.Id.BtnDriveN:
                        motorL.OnForward(powerLevel);
                        motorR.OnForward(powerLevel);
                        break;

                    case Resource.Id.BtnDriveNW:
                        motorL.OnForward((byte)(powerLevel / 3));
                        motorR.OnForward(powerLevel);
                        break;

                    case Resource.Id.BtnDriveNE:
                        motorL.OnForward(powerLevel);
                        motorR.OnForward((byte)(powerLevel / 3));
                        break;

                    case Resource.Id.BtnDriveW:
                        motorL.OnBackward(powerLevel);
                        motorR.OnForward(powerLevel);
                        break;

                    case Resource.Id.BtnDriveE:
                        motorL.OnForward(powerLevel);
                        motorR.OnBackward(powerLevel);
                        break;

                    case Resource.Id.BtnDriveS:
                        motorL.OnBackward(powerLevel);
                        motorR.OnBackward(powerLevel);
                        break;

                    case Resource.Id.BtnDriveSW:
                        motorL.OnBackward((byte)(powerLevel / 3));
                        motorR.OnBackward(powerLevel);
                        break;

                    case Resource.Id.BtnDriveSE:
                        motorL.OnBackward(powerLevel);
                        motorR.OnBackward((byte)(powerLevel / 3));
                        break;

                }
            }
            else if (e.Event.Action == MotionEventActions.Up)
            {
                // Button is no longer pressed.
                button.Pressed = false;
            }

            // If all buttons are released, stop motors.
            if (!(FindViewById<Button>(Resource.Id.BtnDriveNW).Pressed || FindViewById<Button>(Resource.Id.BtnDriveN).Pressed ||
                FindViewById<Button>(Resource.Id.BtnDriveNE).Pressed || FindViewById<Button>(Resource.Id.BtnDriveW).Pressed ||
                FindViewById<Button>(Resource.Id.BtnDriveE).Pressed || FindViewById<Button>(Resource.Id.BtnDriveSW).Pressed ||
                FindViewById<Button>(Resource.Id.BtnDriveS).Pressed || FindViewById<Button>(Resource.Id.BtnDriveSE).Pressed))
            {
                if (brakeDriveMotors)
                {
                    motorL.Off();
                    motorR.Off();
                }
                else
                {
                    motorL.Coast();
                    motorR.Coast();
                }
            }
        }

        /// <summary>
        /// Handles the Polled event of the NxtBrick.
        /// </summary>
        private void OnBrickPolled(NxtDevice sender)
        {
            RunOnUiThread(() =>
            {
                // Update battery status.
                var batteryStatus = FindViewById<TextView>(Resource.Id.TxtBattery);
                batteryStatus.Text = string.Format("Battery:\n{0:F}%", (brick.BatteryLevel / (double)9000) * 100);
            });
        }

        /// <summary>
        /// Handles the Disconnected event of the NxtBrick.
        /// </summary>
        private void OnBrickDisconnected(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                // Brick disconnected, show error and navigate back.
                var toast = Toast.MakeText(Application.Context, "NXT disconnected.", ToastLength.Short);
                toast.Show();

                Finish();
            });
        }

        /// <summary>
        /// Handles the Polled event of the sensor on port 1.
        /// </summary>
        private void OnSensor1Polled(NxtDevice sender)
        {
            // Update TextView.
            RunOnUiThread(() => txtSensor1.Text = string.Format("1: {0}\n{1}", sender.FriendlyName, sender.Value));
        }

        /// <summary>
        /// Handles the Polled event of the sensor on port 2.
        /// </summary>
        private void OnSensor2Polled(NxtDevice sender)
        {
            // Update TextView.
            RunOnUiThread(() => txtSensor2.Text = string.Format("2: {0}\n{1}", sender.FriendlyName, sender.Value));
        }

        /// <summary>
        /// Handles the Polled event of the sensor on port 3.
        /// </summary>
        private void OnSensor3Polled(NxtDevice sender)
        {
            // Update TextView.
            RunOnUiThread(() => txtSensor3.Text = string.Format("3: {0}\n{1}", sender.FriendlyName, sender.Value));
        }

        /// <summary>
        /// Handles the Polled event of the sensor on port 4.
        /// </summary>
        private void OnSensor4Polled(NxtDevice sender)
        {
            // Update TextView.
            RunOnUiThread(() => txtSensor4.Text = string.Format("4: {0}\n{1}", sender.FriendlyName, sender.Value));
        }

        #endregion
    }
}
