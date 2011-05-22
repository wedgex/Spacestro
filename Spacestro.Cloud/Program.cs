using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Spacestro.Cloud.Library;

namespace Spacestro.Cloud
{
    class Program
    {
        static void Main(string[] args)
        {
            Cloud cloud = new Cloud("spacestro", 8383);
            cloud.MessageRecieved += new EventHandler<Library.NetIncomingMessageRecievedEventArgs>(cloud_MessageRecieved);
            cloud.Start();
        }

        static void cloud_MessageRecieved(object sender, Library.NetIncomingMessageRecievedEventArgs e)
        {
            int msgType = e.Message.ReadInt32();

            switch (msgType)
            {
                case (int) CloudMessageType.Keystate:
                    // TODO handle the keystrokes for realsies.
                    InputState state = new InputState(e.Message.ReadBoolean(), e.Message.ReadBoolean(), e.Message.ReadBoolean(), e.Message.ReadBoolean());
                    string input = "recieved keys: ";
                    if (state.Left) input += "left ";
                    if (state.Up) input += "up ";
                    if (state.Right) input += "right ";
                    if (state.Down) input += "down";
                    Console.WriteLine(input);
                    break;
            }
        }
    }
}
