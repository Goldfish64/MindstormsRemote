/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: NxtBrick.cs
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
using Java.IO;

namespace Blueberry.Nxt
{
    /// <summary>
    /// Represents an NXT brick.
    /// </summary>
    public class NxtBrick
    {
        #region Private variables

        private BluetoothDevice bluetoothDevice;
        private BluetoothSocket bluetoothSocket;

        #endregion

        #region Constructor

        public NxtBrick(BluetoothDevice bluetoothDevice)
        {
            // Save Bluetooth device.
            this.bluetoothDevice = bluetoothDevice ?? throw new ArgumentNullException(nameof(bluetoothDevice), "The Bluetooth device can't be null.");
        }

        #endregion

        #region Properties

        public short BatteryLevel
        {
            get
            {
                // Are we connected?
                if (bluetoothSocket?.IsConnected != true)
                    throw new InvalidOperationException();

                var response = SendPacket(new byte[] { (byte)CommandTypes.DirectCommand, (byte)RemoteCommands.GetBatteryLevel });
                return BitConverter.ToInt16(response, 3);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Connects to the NXT over Bluetooth.
        /// </summary>
        public void Connect()
        {
            // Are we connected already?
            if (bluetoothSocket?.IsConnected == true)
                return;

            // Get UUIDs.
            var uuids = bluetoothDevice.GetUuids();
            if (uuids.Length < 1)
                throw new InvalidOperationException("The Bluetooth device didn't return any UUIDs.");

            // Create and open socket.
            try
            {
                bluetoothSocket = bluetoothDevice.CreateRfcommSocketToServiceRecord(uuids[0].Uuid);
                bluetoothSocket.Connect();
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Disconnects the Bluetooth connection.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                // Close socket.
                bluetoothSocket?.Close();
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        internal byte[] SendPacket(byte[] data)
        {
            // Create packet.
            var bytes = new byte[data.Length + 2];
            BitConverter.GetBytes((short)data.Length).CopyTo(bytes, 0);
            data.CopyTo(bytes, 2);

            try
            {
                // Send packet bytes.
                bluetoothSocket.OutputStream.Write(bytes, 0, bytes.Length);

                // Read length bytes.
                var lengthBytes = new byte[2];
                bluetoothSocket.InputStream.Read(lengthBytes, 0, lengthBytes.Length);

                // Read response bytes.
                var response = new byte[BitConverter.ToInt16(lengthBytes, 0)];
                bluetoothSocket.InputStream.Read(response, 0, response.Length);
                return response;
            }
            catch (IOException)
            {
                throw;
            }
        }

        #endregion
    }
}