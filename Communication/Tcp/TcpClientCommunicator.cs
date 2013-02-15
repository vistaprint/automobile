/*
Copyright 2013 Vistaprint

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

TcpClientCommunicator.cs 
*/
using System.Net;
using System.Net.Sockets;

namespace Automobile.Communication.Tcp
{
    /// <summary>
    /// A client side communicator
    /// </summary>
    public class TcpClientCommunicator : TcpCommunicator
    {
        /// <summary>
        /// Connects to a specified server
        /// </summary>
        /// <param name="ip">IP to connect to</param>
        /// <param name="port">Port to connect on</param>
        public TcpClientCommunicator(string ip, int port) : base(IPAddress.Parse(ip), port) {}

        /// <summary>
        /// Connects to a specified server
        /// </summary>
        /// <param name="ip">IP to connect to</param>
        /// <param name="port">Port to connect on</param>
        public TcpClientCommunicator(IPAddress ip, int port) : base(ip, port) { }

        /// <summary>
        /// Prepare the stream for reading and writing
        /// </summary>
        public override void Initialize()
        {
            Server = new IPEndPoint(IP, Port);
            Client = new TcpClient();
            Client.Connect(Server);
            base.Initialize();
        }

        /// <summary>
        /// Close the stream
        /// </summary>
        public override void Close()
        {
            base.Close();
            Server = null;
        }

         /// <summary>
         /// The server connected to
         /// </summary>
         private IPEndPoint Server { get; set; }
    }
}