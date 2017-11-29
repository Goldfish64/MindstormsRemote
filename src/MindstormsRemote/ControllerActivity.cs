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
            // nxt.MotorA.OnForward(40);

            // brick.Sensor1 = new NxtTouchSensor();
            // brick.Sensor3 = new NxtColorSensor();
            // (nxt.Sensor3 as NxtColorSensor).DetectionMode = NxtColorSensorModes.LightBlue;
            //brick.Sensor1.Polled += (s) =>
            //{
            //    if ((s as NxtTouchSensor).IsPressed == true)
            //        (brick.Sensor3 as NxtColorSensor).DetectionMode = NxtColorSensorModes.LightBlue;
            //    else
            //        (brick.Sensor3 as NxtColorSensor).DetectionMode = NxtColorSensorModes.LightGreen;

            //    brick.Sensor3.Poll();
            //};
            //   brick.Sensor1.PollingInterval = 100;

            // var seekbar = FindViewById<SeekBar>(Resource.Id.seekLeftMotor);
            // seekbar.ProgressChanged += Seekbar_ProgressChanged;
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

        public override void OnAttachedToWindow()
        {
            // Call base method.
            base.OnAttachedToWindow();

            // Set title to device.
            Window.SetTitle(device?.Name);
        }

        private void Seekbar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (e.Progress > 10)
                Task.Run(() => brick.MotorA.OnForward((byte)((e.Progress - 10) * 10)));
            else
                Task.Run(() => brick.MotorA.OnBackward((byte)((10 - e.Progress) * 10)));
            //   throw new NotImplementedException();
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
    }
}