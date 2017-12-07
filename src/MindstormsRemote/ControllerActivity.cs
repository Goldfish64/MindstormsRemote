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
using Android.Bluetooth;
using Blueberry.Nxt;
using System.Threading.Tasks;
using Android.Support.V7.App;
using Android.Preferences;
using MindstormsRemote.Framework;

namespace MindstormsRemote
{
    [Activity(Theme = "@style/Theme.AppCompat")]
    public class ControllerActivity : AppCompatActivity
    {
        private BluetoothDevice device;
        private NxtBrick brick;
        private NxtMotor motorL;
        private NxtMotor motorR;
        private byte powerLevel = 75;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Call base method.
            base.OnCreate(savedInstanceState);

            // Load the layout into view.
            SetContentView(Resource.Layout.ControllerPage);

            // Wire up controller button events.
            FindViewById<Button>(Resource.Id.BtnDriveNW).Touch += OnControllerButtonTouch;
            FindViewById<Button>(Resource.Id.BtnDriveN).Touch += OnControllerButtonTouch;
            FindViewById<Button>(Resource.Id.BtnDriveNE).Touch += OnControllerButtonTouch;
            FindViewById<Button>(Resource.Id.BtnDriveW).Touch += OnControllerButtonTouch;
            FindViewById<Button>(Resource.Id.BtnDriveE).Touch += OnControllerButtonTouch;
            FindViewById<Button>(Resource.Id.BtnDriveSW).Touch += OnControllerButtonTouch;
            FindViewById<Button>(Resource.Id.BtnDriveS).Touch += OnControllerButtonTouch;
            FindViewById<Button>(Resource.Id.BtnDriveSE).Touch += OnControllerButtonTouch;

            // Get MAC address passed.
            var address = Intent.GetStringExtra(ConnectActivity.BluetoothAddressExtra);
            device = BluetoothAdapter.DefaultAdapter.GetRemoteDevice(address);

            brick = new NxtBrick(device);
            brick.Connect();
            brick.PlayTone(1000, 100);
            motorL = new NxtMotor();
            motorR = new NxtMotor();
            brick.MotorC = motorL;
            brick.MotorB = motorR;

           // var us = new NxtUltrasonicSensor();
            //brick.Sensor4 = us;
           // var s = us.ReadMeasurementUnits();
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
                motorL.Coast();
                motorR.Coast();
            }
        }

        protected override void OnResume()
        {
            // Call base method.
            base.OnResume();

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
                FindViewById<TextView>(Resource.Id.TxtSensor1).Text = "Port 1: None";
            if (brick.Sensor2 == null)
                FindViewById<TextView>(Resource.Id.TxtSensor2).Text = "Port 2: None";
            if (brick.Sensor3 == null)
                FindViewById<TextView>(Resource.Id.TxtSensor3).Text = "Port 3: None";
            if (brick.Sensor4 == null)
                FindViewById<TextView>(Resource.Id.TxtSensor4).Text = "Port 4: None";
        }

        private void OnSensor1Polled(NxtDevice sender)
        {
            RunOnUiThread(() =>
            {
                var textView = FindViewById<TextView>(Resource.Id.TxtSensor1);
                textView.Text = string.Format("Port 1: {0}\n{1}", sender.FriendlyName, sender.Value);
            });
        }

        private void OnSensor2Polled(NxtDevice sender)
        {
            RunOnUiThread(() =>
            {
                var textView = FindViewById<TextView>(Resource.Id.TxtSensor2);
                textView.Text = string.Format("Port 2: {0}\n{1}", sender.FriendlyName, sender.Value);
            });
        }

        private void OnSensor3Polled(NxtDevice sender)
        {
            RunOnUiThread(() =>
            {
                var textView = FindViewById<TextView>(Resource.Id.TxtSensor3);
                textView.Text = string.Format("Port 3: {0}\n{1}", sender.FriendlyName, sender.Value);
            });
        }

        private void OnSensor4Polled(NxtDevice sender)
        {
            RunOnUiThread(() =>
            {
                var textView = FindViewById<TextView>(Resource.Id.TxtSensor4);
                textView.Text = string.Format("Port 4: {0}\n{1}", sender.FriendlyName, sender.Value);
            });
        }

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

        public override void OnAttachedToWindow()
        {
            // Call base method.
            base.OnAttachedToWindow();

            // Set title to device.
            Window.SetTitle(device?.Name);
        }

        protected override void OnDestroy()
        {
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
    }
}