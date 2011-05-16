using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace Spacestro
{
    class ServerMessenger
    {
        private static ServerMessenger serverMessenger;

        private const int MESSAGE_SIZE = 4096;

        private Thread listenThread;
        private TcpClient tcpClient;

        public event EventHandler MessageRecieved;
                
        public static ServerMessenger Instance 
        { 
            get 
            {
                if(serverMessenger == null) serverMessenger = new ServerMessenger();

                return serverMessenger;                
            }
        }

        public bool Connected
        {
            get
            {
                return this.tcpClient.Connected;
            }
        }

        private ServerMessenger()
        {
            // TODO: Determine if this is really the best place to be doing this.
            // TODO: load the server address and port from a config file or login screen.
            // TDOO: handle any exceptions from failing to connect.
            this.tcpClient = new TcpClient("localhost", 8383);

            this.listenThread = new Thread(new ThreadStart(ListenForMessages));
            this.listenThread.Start();     
       
        }

        private void ListenForMessages()
        {
            NetworkStream serverStream = this.tcpClient.GetStream();
            byte[] message = new byte[MESSAGE_SIZE];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    bytesRead = serverStream.Read(message, 0, MESSAGE_SIZE);
                }
                catch (Exception ex)
                {
                    // TODO: Log exception.
                    break;
                }

                if (bytesRead > 0)
                {
                    MessageRecieved(this, new EventArgs());
                }
            }

            this.tcpClient.Close();
        }

        public void SendMessage(ClientMessages message)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] byteMessage = new byte[0];

            switch (message)
            {
                case ClientMessages.Accelerate:
                    byteMessage = encoder.GetBytes("Accelerate");
                    break;
                case ClientMessages.Decelerate:
                    byteMessage = encoder.GetBytes("Decelerate");
                    break;
                case ClientMessages.TurnLeft:
                    byteMessage = encoder.GetBytes("TurnLeft");
                    break;
                case ClientMessages.TurnRight:
                    byteMessage = encoder.GetBytes("TurnRight");
                    break;
                case ClientMessages.GetPosition:
                    byteMessage = encoder.GetBytes("GetPosition");
                    break;
            }

            if (byteMessage.Length > 0)
            {
                // TODO make sure if this is the proper way to handle this.
                lock (this.tcpClient)
                {
                    this.tcpClient.GetStream().Write(byteMessage, 0, byteMessage.Length);
                    this.tcpClient.GetStream().Flush();
                }
            }
        }
    }

    public enum ClientMessages { Accelerate, Decelerate, TurnLeft, TurnRight, GetPosition }
}
