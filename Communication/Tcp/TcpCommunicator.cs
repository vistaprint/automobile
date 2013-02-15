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

TcpCommunicator.cs 
*/
using System.Net;
using System.Net.Sockets;

namespace Automobile.Communication.Tcp
{
    /// <summary>
    /// A communicator which operates over TCP
    /// </summary>
    public abstract class TcpCommunicator : Communicator
    {
        /// <summary>
        /// A Communicator with a given IP and Port
        /// </summary>
        /// <param name="ip">IP associated with this TcpCommunicator</param>
        /// <param name="port">Port associated with this TcpCommunicator</param>
        protected TcpCommunicator(IPAddress ip, int port)
        {
            IP = ip;
            Port = port;
        }

        /// <summary>
        /// If the TcpCommunicator is connected
        /// </summary>
        public bool Connected
        {
            get { return Client != null && Client.Connected; }
        }

        /// <summary>
        /// IP address associated with this TcpCommunicator
        /// </summary>
        public IPAddress IP { get; protected set; }

        /// <summary>
        /// Port associated with this TcpCommunicator
        /// </summary>
        public int Port { get; protected set; }

        /// <summary>
        /// Connected client, null if not connected
        /// </summary>
        protected TcpClient Client { get; set; }

        /// <summary>
        /// Prepare the stream for reading and writing
        /// </summary>
        public override void Initialize()
        {
            Stream = Client.GetStream();
        }

        /// <summary>
        /// Close the stream
        /// </summary>
        public override void Close()
        {
            base.Close();
            Client.Close();
            Client = null;
        }
    }
}