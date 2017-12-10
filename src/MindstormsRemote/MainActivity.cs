/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: MainActivity.cs
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
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using MindstormsRemote.Framework;

namespace MindstormsRemote
{
    /// <summary>
    /// Represents the main activity.
    /// </summary>
    [Activity(Label = "Mindstorms Remote", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        #region Private variables

        private ListView listView;

        #endregion

        #region Methods

        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Call base method.
            base.OnCreate(savedInstanceState);

            // Load the layout into view.
            SetContentView(Resource.Layout.MainPage);

            // Wire up button events.
            FindViewById<Button>(Resource.Id.BtnScan).Click += OnScanClick;

            // Get ListView.
            listView = FindViewById<ListView>(Resource.Id.LstBluetoothDevices);
            listView.ItemClick += OnItemClick;

            // Check if Bluetooth is supported
            if (BluetoothAdapter.DefaultAdapter != null)
            {
                // Is Bluetooth enabled? If not, show prompt to enable it.
                if (!BluetoothAdapter.DefaultAdapter.IsEnabled)
                {
                    var intentEnableBt = new Intent(BluetoothAdapter.ActionRequestEnable);
                    StartActivityForResult(intentEnableBt, 1);
                }

                // Register a receiver so we know when devices are found.
                var btReceiver = new EventReceiver();
                btReceiver.BroadcastReceived += OnBtBroadcastReceived;
                RegisterReceiver(btReceiver, new IntentFilter(BluetoothDevice.ActionFound));
            }
        }

        private void OnScanClick(object sender, EventArgs e)
        {
            // Make progress indicator visible.
            FindViewById<ProgressBar>(Resource.Id.PrgSearching).Visibility = ViewStates.Visible;

            // Delete list and restart scan.
            BluetoothAdapter.DefaultAdapter.CancelDiscovery();
            listView.Adapter = new BluetoothDevicesAdapter(this);
            BluetoothAdapter.DefaultAdapter.StartDiscovery();
        }

        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            // Stop discovery.
            BluetoothAdapter.DefaultAdapter.CancelDiscovery();
            FindViewById<ProgressBar>(Resource.Id.PrgSearching).Visibility = ViewStates.Gone;

            // Get device.
            var device = listView.GetItemAtPosition(e.Position) as BluetoothDevice;

            // Change to controller activity and pass the MAC address of the chosen NXT to it.
            var controllerActivity = new Intent(this, typeof(ControllerActivity));
            controllerActivity.PutExtra(Constants.BluetoothAddressExtra, device.Address);

            StartActivity(controllerActivity);
            listView.Adapter = null;
        }

        protected override void OnDestroy()
        {
            // Stop discovery.
            BluetoothAdapter.DefaultAdapter.CancelDiscovery();

            base.OnDestroy();
        }

        private void OnBtBroadcastReceived(object sender, BroadcastReceivedEventArgs e)
        {
            // Did we find a Bluetooth device?
            if (e.Intent.Action == BluetoothDevice.ActionFound)
            {
                // We only want to add NXTs.
                var device = e.Intent.GetParcelableExtra(BluetoothDevice.ExtraDevice) as BluetoothDevice;
                if (device.BluetoothClass.MajorDeviceClass == MajorDeviceClass.Toy && device.BluetoothClass.DeviceClass == DeviceClass.ToyRobot)
                {
                    // Add to list.
                    var adapter = listView.Adapter as BluetoothDevicesAdapter;
                    adapter.Add(device);
                }
            }
        }

        #endregion
    }
}