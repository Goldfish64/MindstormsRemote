/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: NxtDevice.cs
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

using System.Threading;

namespace Blueberry.Nxt
{
    /// <summary>
    /// Represents the base class for NXT motors and sensors.
    /// </summary>
    public abstract class NxtDevice
    {
        #region Protected variables

        /// <summary>
        /// Represents the parent NXT brick.
        /// </summary>
        protected NxtBrick brick;
        private Timer pollingTimer;
        private int pollingInterval = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NxtDevice"/> class.
        /// </summary>
        public NxtDevice()
        {
            // Initialize timer.
            pollingTimer = new Timer((state) =>
            {
                // If the NXT brick is not connected or null, return.
                if (brick?.IsConnected != true)
                    return;

                // Poll device.
                Poll();
            }, null, Timeout.Infinite, Timeout.Infinite);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the polling interval. A value of 0 or below disables polling.
        /// </summary>
        public int PollingInterval
        {
            get { return pollingInterval; }
            set
            {
                // Save and update interval.
                pollingInterval = value;
                if (pollingInterval > 0)
                {
                    if (brick?.IsConnected == true)
                        pollingTimer.Change(pollingInterval, pollingInterval);
                    else
                        pollingTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
                else
                {
                    pollingInterval = 0;
                    pollingTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        #endregion

        #region Abstract methods

        /// <summary>
        /// Polls the device.
        /// </summary>
        public virtual void Poll() => Polled?.Invoke(this);

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the device is polled.
        /// </summary>
        public event PolledHandler Polled;

        #endregion
    }

    /// <summary>
    /// Represents the method that will handle polling events.
    /// </summary>
    /// <param name="sender">The device that ran a poll.</param>
    public delegate void PolledHandler(NxtDevice sender);
}