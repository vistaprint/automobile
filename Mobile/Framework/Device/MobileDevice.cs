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

LocalDevice.cs 
*/

using System;
using System.ComponentModel;
using Automobile.Communication;
using Automobile.Communication.Messaging;
using Automobile.Communication.Tcp;
using Automobile.Mobile.Framework.Browser;
using Automobile.Mobile.Framework.Commands;
using Automobile.Mobile.Framework.Data;


namespace Automobile.Mobile.Framework.Device
{
    /// <summary>
    /// The device being executed on
    /// </summary>
    public abstract class MobileDevice : IMobileDevice
    {
        private TcpCommunicator _communicator;

        // TODO: make configable
        private const int TCP_PORT = 4242;
        private const int UDP_PORT = 4343;

        protected MobileDevice(ConnectionType connectionType, string connectionString) : this(connectionType, connectionString, null) { }

        protected MobileDevice(ConnectionType connectionType, string connectionString, IJsonProvider json)
        {
            // Create appropriate Client for our connection type
            switch (connectionType)
            {
                case ConnectionType.DbRegistration:
                    MobileDb.CreateSqlClient(connectionString);
                    break;
                case ConnectionType.Direct:
                    MobileDb.CreateNullClient();
                    break;
                case ConnectionType.Registrar:
                    MobileDb.CreateRegistrarClient(connectionString, json);
                    break;
                case ConnectionType.UdpBroadcast:
                    MobileDb.CreateUdpClient(connectionString, UDP_PORT);
                    break;
            }

        }

        /// <summary>
        /// The browser belonging to this device
        /// </summary>
        public IMobileBrowser Browser { get; protected set; }

        /// <summary>
        /// Device info for this device
        /// </summary>
        public DeviceInfo DeviceInfo { get; protected set; }

        /// <summary>
        /// Orientation of the device
        /// </summary>
        public abstract Orientation Orientation { get; set; }

        /// <summary>
        /// Take a screenshot of the device
        /// </summary>
        /// <returns>Array of bytes representing the image</returns>
        public abstract byte[] TakeScreenshot();
        
        // TODO: Too long/messy, needs refactoring
        /// <summary>
        /// Begin the main automation loop, accepting connections and executing commands
        /// </summary>
        public virtual void BeginAutomation()
        {
            _communicator = new TcpServerCommunicator(TCP_PORT);

            MobileDb.Instance.Register(DeviceInfo);

            while (true)
            {
                _communicator.Initialize();
                MobileDb.Instance.SetAvailibility(DeviceInfo, false);

                while (_communicator.Connected)
                {
                    Response response;
                    var message = _communicator.WaitForMessage<Command>();
                    try
                    {
                        switch (message.CommandType)
                        {
                            case CommandType.ExecJavascript:
                                var jsCommand = message as ExecJavascriptCommand;
                                response = new CommandResponse<string>(true, Browser.ExecJavascript(jsCommand.Javascript));
                                break;
                            case CommandType.Url:
                                var urlCommand = message as UrlCommand;

                                switch (urlCommand.Mode)
                                {
                                    case CommandMode.Get:
                                        response = new CommandResponse<string>(true, Browser.Url);
                                        break;
                                    case CommandMode.Set:
                                        Browser.Navigate(urlCommand.Url);
                                        response = new Response(true);
                                        break;
                                    case CommandMode.Invoke:
                                        throw new InvalidEnumArgumentException(
                                            "Cannot Invoke the URL CommandType");
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;
                            case CommandType.Screenshot:
                                response = new CommandResponse<byte[]>(true, TakeScreenshot());
                                break;
                            case CommandType.Orientation:
                                var orientationCommand = message as OrientationCommand;
                                switch (orientationCommand.Mode)
                                {
                                    case CommandMode.Get:
                                        response = new CommandResponse<Orientation>(true, Orientation);
                                        break;
                                    case CommandMode.Set:
                                        Orientation = orientationCommand.Orientation;
                                        response = new Response(true);
                                        break;
                                    case CommandMode.Invoke:
                                        throw new InvalidEnumArgumentException(
                                            "Cannot Invoke the Orientation CommandType");
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;
                            case CommandType.Disconnect:
                                Browser.Navigate("about:blank", false);
                                _communicator.SendResponse(new Response(true));
                                _communicator.Close();
                                MobileDb.Instance.SetAvailibility(DeviceInfo, true);
                                continue;
                            case CommandType.WaitForReady:
                                Browser.WaitForReady();
                                response = new Response(true);
                                break;
                            case CommandType.Refresh:
                                Browser.Refresh();
                                response = new Response(true);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    catch(Exception e)
                    {
                        response = new CommandResponse<Exception>(false, e);
                    }
                    

                    _communicator.SendResponse(response);
                }
            }
        }
    }
}
