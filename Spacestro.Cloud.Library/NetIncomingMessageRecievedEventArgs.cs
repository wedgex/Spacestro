using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace Spacestro.Cloud.Library
{
    /// <summary>
    /// Event args to use whenever an incoming message arrives.
    /// </summary>
    public class NetIncomingMessageRecievedEventArgs : EventArgs
    {
        public NetIncomingMessage Message { get; private set; }

        public NetIncomingMessageRecievedEventArgs(NetIncomingMessage msg)
        {
            this.Message = msg;
        }
    }
}
