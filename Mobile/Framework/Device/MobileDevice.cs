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
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Timers;
using Automobile.Communication;
using Automobile.Communication.Messaging;
using Automobile.Communication.Tcp;
using Automobile.Mobile.Framework.Browser;
using Automobile.Mobile.Framework.Commands;

namespace Automobile.Mobile.Framework.Device
{
    /// <summary>
    /// The device being executed on
    /// </summary>
    public abstract class MobileDevice : IMobileDevice
    {
        private ConnectionType _connectionType;
        private TcpCommunicator _communicator;
        private UdpClient _udpClient;
        private IPEndPoint _endPoint;
        private Timer _timer;
        private string _connectionString;


        // TODO: make configable
        private const int TCP_PORT = 4242;
        private const int UDP_PORT = 4343;
        private const string DEVICE_STRING = "MobileOS:{0},DeviceModel:{1},OsVersion:{2},UniqueId:{3},IP:{4}";

        protected MobileDevice(ConnectionType connectionType, string connectionString)
        {
            _connectionType = connectionType;
            _connectionString = connectionString;

            // Untested
            if(connectionType == ConnectionType.UdpBroadcast)
            {
                var ip = IPAddress.Parse("234.234.234.234");
                _udpClient = new UdpClient();
                _udpClient.JoinMulticastGroup(ip);
                _endPoint = new IPEndPoint(ip, UDP_PORT);

                _timer = new Timer(30 * 1000); // 30 seconds
                _timer.AutoReset = true;
                _timer.Elapsed += Broadcast;
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
            if(_connectionType == ConnectionType.DbRegistration)
            {
                Register();
            }

            while (true)
            {
                if(_connectionType == ConnectionType.UdpBroadcast)
                {
                    BeginBroadcast();
                }
                _communicator.Initialize();
                if (_connectionType == ConnectionType.UdpBroadcast)
                {
                    EndBroadcast();
                }

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
        
        /// <summary>
        /// Execute sproc to register device in the DB specified by _connectionString
        /// </summary>
        private void Register()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            var sql = new SqlCommand(string.Format("exec mob_update_device_registration '{0}', '{1}', '{2}', '{3}'", DeviceInfo.UniqueId, DeviceInfo.MobileOs, DeviceInfo.OsVersion, DeviceInfo.IP), conn);
            var r = sql.ExecuteNonQuery();
        }

        // Untested
        private void BeginBroadcast()
        {
            Broadcast(null, null);
            _timer.Start();
        }

        // Untested
        private void Broadcast(object sender, ElapsedEventArgs args)
        {
            var device = string.Format(DEVICE_STRING, DeviceInfo.MobileOs, DeviceInfo.OsVersion,
                                       DeviceInfo.UniqueId, DeviceInfo.IP);
            var encoding = new ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(device);
            Byte[] len = BitConverter.GetBytes(bytes.Length);
            _udpClient.Send(len, 4, _endPoint);
            _udpClient.Send(bytes, bytes.Length, _endPoint);
        }

        // Untested
        private void EndBroadcast()
        {
            _timer.Stop();
        }
    }
}