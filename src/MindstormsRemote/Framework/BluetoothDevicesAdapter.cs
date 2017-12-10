/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: BluetoothDevicesAdapter.cs
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
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace MindstormsRemote.Framework
{
    /// <summary>
    /// Represents an <see cref="Adapter"/> for <see cref="BluetoothDevice"/> items.
    /// </summary>
    public class BluetoothDevicesAdapter : BaseAdapter
    {
        #region Private variables

        private List<BluetoothDevice> devices;
        private Activity activity;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothDevicesAdapter"/> class.
        /// </summary>
        public BluetoothDevicesAdapter(Activity activity)
        {
            // Initialize variables.
            this.activity = activity;
            devices = new List<BluetoothDevice>();
        }

        #endregion

        #region Properties

        public override int Count
        {
            get { return devices.Count; }
        }

        #endregion

        #region Methods

        public override Java.Lang.Object GetItem(int position)
        {
            return devices[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            // Get item and view.
            var device = devices[position];
            var view = (convertView ?? activity.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, parent, false)) as TextView;
            //var text = view.FindViewById<TextView>(Resource.Id.textItem);
            view.Text = device.Name;

            // Return the view.
            return view;
        }

        public void Add(BluetoothDevice device)
        {
            if (!devices.Contains(device))
                devices.Add(device);
            NotifyDataSetChanged();
        }

        public void Remove(BluetoothDevice device)
        {
            if (devices.Contains(device))
                devices.Remove(device);
            NotifyDataSetChanged();
        }

        public void Clear()
        {
            devices.Clear();
            NotifyDataSetChanged();
        }

        #endregion
    }
}
