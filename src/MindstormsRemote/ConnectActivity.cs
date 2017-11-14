/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: ConnectActivity.cs
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
using Android.Widget;
using MindstormsRemote.Framework;
using System.Collections;
using System.Collections.Generic;
using Android.Views;
using Java.Util;
using System.Threading;
using System.IO;
using System;
using Blueberry.Nxt;

namespace MindstormsRemote
{
    [Activity(Label = "ConnectActivity", MainLauncher = true)]
    public class ConnectActivity : ListActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Call base method.
            base.OnCreate(savedInstanceState);

            // Load the layout into view.
            SetContentView(Resource.Layout.ConnectPage);

            //var listview = FindViewById<ListView>(Resource.Id.);
            var adapter = new BluetoothDevicesAdapter(this);
            //adapter.Add(device.Name);
            ListView.Adapter = adapter;



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
                btReceiver.BroadcastReceived += BtReceiver_BroadcastReceived;

                RegisterReceiver(btReceiver, new IntentFilter(BluetoothDevice.ActionFound));

                // Scan for NXT and EV3 bricks.
                BluetoothAdapter.DefaultAdapter.StartDiscovery();
            }
        }

        private void BtReceiver_BroadcastReceived(object sender, BroadcastReceivedEventArgs e)
        {
            // Did we find a Bluetooth device?
            if (e.Intent.Action == BluetoothDevice.ActionFound)
            {
                var device = e.Intent.GetParcelableExtra(BluetoothDevice.ExtraDevice) as BluetoothDevice;
                var adapter = ListView.Adapter as BluetoothDevicesAdapter;

                adapter.Add(device);
            }
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            base.OnListItemClick(l, v, position, id);

            // Stop discovery.
            BluetoothAdapter.DefaultAdapter.CancelDiscovery();

            // Get device.
            var device = l.GetItemAtPosition(position) as BluetoothDevice;
            var nxt = new NxtBrick(device);
            nxt.Connect();
            nxt.PlayTone(1000, 100);
            nxt.MotorA = new NxtMotor();
            nxt.MotorA.OnForward(40, 360, true);
            //Thread.Sleep(2000);
           // nxt.MotorA.Off();
        }

        private void MotorA_Polled(NxtDevice sender)
        {
            //throw new NotImplementedException();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}