/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: EventReceiver.cs
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

using Android.Content;

namespace MindstormsRemote.Framework
{
    public class EventReceiver : BroadcastReceiver
    {
        #region Overridden methods

        public override void OnReceive(Context context, Intent intent)
        {
            // Raise event.
            BroadcastReceived?.Invoke(this, new BroadcastReceivedEventArgs(context, intent));
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a broadcast is received.
        /// </summary>
        public event BroadcastReceivedEventHandler BroadcastReceived;

        #endregion
    }

    /// <summary>
    /// Contains event data for the <see cref="BroadcastReceivedEventArgs"/> delegate.
    /// </summary>
    public class BroadcastReceivedEventArgs
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcastReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="context">The <see cref="Context"/> object.</param>
        /// <param name="intent">The <see cref="Intent"/> object.</param>
        public BroadcastReceivedEventArgs(Context context, Intent intent)
        {
            Context = context;
            Intent = intent;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The Context in which the receiver is running.
        /// </summary>
        public Context Context { get; }

        /// <summary>
        /// Gets the Intent being received.
        /// </summary>
        public Intent Intent { get; }

        #endregion
    }

    /// <summary>
    /// Represents the method that will handle the <see cref="EventReceiver.BroadcastReceived"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void BroadcastReceivedEventHandler(object sender, BroadcastReceivedEventArgs e);
}
