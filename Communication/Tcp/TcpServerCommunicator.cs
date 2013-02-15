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

TcpServerCommunicator.cs 
*/
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Automobile.Communication.Tcp
{
    /// <summary>
    /// A server communicator
    /// </summary>
    public class TcpServerCommunicator : TcpCommunicator
    {

        private Thread _thread;
        public delegate void ConnectionCallback(TcpServerCommunicator communicator);

        /// <summary>
        /// Listens for a single connection on a given port
        /// </summary>
        /// <param name="port">Port to listen on</param>
        public TcpServerCommunicator(int port) : base(IPAddress.Any, port)
        {
            Listener = new TcpListener(IPAddress.Any, Port);
        }

        /// <summary>
        /// Listens for a single connection on a given port
        /// </summary>
        /// <param name="port">Port to listen on</param>
        /// <param name="listener">Listener to use for this communicator</param>
        private TcpServerCommunicator(int port, TcpClient client) : base(IPAddress.Any, port)
        {
            Client = client;
        }


        /// <summary>
        /// Prepare the stream for single connection reading and writing
        /// </summary>
        public override void Initialize()
        {
            Listener.Start();
            Client = AcceptClient();
            base.Initialize();
            Listener.Stop();
        }

        /// <summary>
        /// Recieve multiple connections, each connection is passed to the callback in a new thread. Does not return.
        /// </summary>
        /// <param name="callback"></param>
        public void Initialize(ConnectionCallback callback)
        {
            _thread = new Thread(() => AcceptMultiple(callback));
        }

        /// <summary>
        /// Accept a client
        /// </summary>
        private TcpClient AcceptClient()
        {
            while (!Listener.Pending())
            {
                Thread.Sleep(500);
            }

            return Listener.AcceptTcpClient();    
        }

        /// <summary>
        /// Recieve multiple connections, each connection is passed to the callback in a new thread. Does not return.
        /// </summary>
        /// <param name="callback"></param>
        private void AcceptMultiple(ConnectionCallback callback)
        {
            while(true)
            {
                var comm = new TcpServerCommunicator(Port, AcceptClient());
                var thread = new Thread(() => callback(comm));
                thread.Start();
            }
        }

        /// <summary>
        /// Close the stream
        /// </summary>
        public override void Close()
        {
            if(_thread != null)
            {
                _thread.Abort();
            }
            base.Close();
            Listener = null;
        }

        /// <summary>
        /// Listens for connections from a client
        /// </summary>
        private TcpListener Listener { get; set; }

    }
}