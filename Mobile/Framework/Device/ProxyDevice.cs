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

RemoteDevice.cs 
*/
using Automobile.Communication.Messaging;
using Automobile.Communication.Tcp;
using Automobile.Mobile.Framework.Browser;
using Automobile.Mobile.Framework.Commands;

namespace Automobile.Mobile.Framework.Device
{
    /// <summary>
    /// A remotely accessed mobile device
    /// </summary>
    public class ProxyDevice : IProxyDevice
    {
        private TcpCommunicator _communicator;

        public ProxyDevice(string ip)
        {
            Browser = new ProxyBrowser(this);
            _communicator = new TcpClientCommunicator(ip, 4242);
        }

        /// <summary>
        /// Orientation of the remote device
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return SendCommand<Orientation>(new OrientationCommand(default(Orientation), CommandMode.Get));
            }
            set 
            { 
                SendCommand(new OrientationCommand(value, CommandMode.Set)); 
            }
        }

        /// <summary>
        /// The browser on the remote device
        /// </summary>
        public IMobileBrowser Browser { get; private set; }

        /// <summary>
        /// If a remote device is connected
        /// </summary>
        public bool Connected { get { return _communicator != null && _communicator.Connected; } }

        /// <summary>
        /// Take a screenshot of the remote device
        /// </summary>
        /// <returns>Array of bytes which is the image</returns>
        public byte[] TakeScreenshot()
        {
            var resp = SendCommand<byte[]>(new ScreenshotCommand());
            return resp;
        }

        /// <summary>
        /// Initialize a connection to the specified remote device
        /// </summary>
        public void Connect()
        {
            _communicator.Initialize();
        }

        /// <summary>
        /// Disconnect from the remote device
        /// </summary>
        public void Disconnect()
        {
            SendCommand(new DisconnectCommand());
            _communicator.Close();
            _communicator = null;
        }

        /// <summary>
        /// Send a command to the remote device
        /// </summary>
        /// <param name="command">Command to send</param>
        public void SendCommand(Command command)
        {
            var resp = _communicator.SendMessage<Response>(command);

            if (!resp.Success)
            {
                throw new CommandException(command, "Command Failed");
            }
        }

        /// <summary>
        /// Sent a command to the remote device
        /// </summary>
        /// <typeparam name="TResponse">Expected response type</typeparam>
        /// <param name="command">Command to send</param>
        /// <returns>Response recieved from the remote device</returns>
        public TResponse SendCommand<TResponse>(Command command)
        {
            var resp = _communicator.SendMessage<CommandResponse<TResponse>>(command);

            if(!resp.Success)
            {
                throw new CommandException(command, "Command Failed");
            }

            return resp.Contents;
        }
    }
}