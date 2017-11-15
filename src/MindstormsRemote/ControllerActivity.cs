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

namespace MindstormsRemote
{
    [Activity(Label = "ControllerActivity", Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class ControllerActivity : Activity
    {
        private NxtBrick brick;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Call base method.
            base.OnCreate(savedInstanceState);

            // Load the layout into view.
            SetContentView(Resource.Layout.ControllerPage);

            // Get MAC address passed.
            var address = Intent.GetStringExtra(ConnectActivity.BluetoothAddressExtra);
            var device = BluetoothAdapter.DefaultAdapter.GetRemoteDevice(address);

            brick = new NxtBrick(device);
            brick.Connect();
            brick.PlayTone(1000, 100);
            brick.MotorA = new NxtMotor();
            // nxt.MotorA.OnForward(40);

            brick.Sensor1 = new NxtTouchSensor();
            brick.Sensor3 = new NxtColorSensor();
            // (nxt.Sensor3 as NxtColorSensor).DetectionMode = NxtColorSensorModes.LightBlue;
            //brick.Sensor1.Polled += (s) =>
            //{
            //    if ((s as NxtTouchSensor).IsPressed == true)
            //        (brick.Sensor3 as NxtColorSensor).DetectionMode = NxtColorSensorModes.LightBlue;
            //    else
            //        (brick.Sensor3 as NxtColorSensor).DetectionMode = NxtColorSensorModes.LightGreen;

            //    brick.Sensor3.Poll();
            //};
            //brick.Sensor1.PollingInterval = 100;

            var seekbar = FindViewById<SeekBar>(Resource.Id.seekBar1);
            seekbar.ProgressChanged += Seekbar_ProgressChanged;
        }

        private void Seekbar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (e.Progress > 10)
                Task.Run(() => brick.MotorA.OnForward((byte)((e.Progress-10)*10)));
            else
                Task.Run(() => brick.MotorA.OnBackward((byte)((10 - e.Progress)*10)));
            //   throw new NotImplementedException();
        }

        protected override void OnDestroy()
        {
            brick.Dispose();

            // Call base method.
            base.OnDestroy();
        }
    }
}