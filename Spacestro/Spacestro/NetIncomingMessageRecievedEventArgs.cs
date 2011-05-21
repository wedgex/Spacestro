using System;
using Lidgren.Network;

namespace Spacestro
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
