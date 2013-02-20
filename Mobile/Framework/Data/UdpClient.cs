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

UdpClient.cs 
*/
using System.Net;
using Automobile.Mobile.Framework.Device;
using System.Timers;
using System.Text;
using System;

namespace Automobile.Mobile.Framework.Data
{
    public class UdpClient : IMobileDb
    {
        private System.Net.Sockets.UdpClient _udpClient;
        private IPEndPoint _endPoint;
        private Timer _timer;
        private const string DEVICE_STRING = "MobileOS:{0},DeviceModel:{1},OsVersion:{2},UniqueId:{3},IP:{4}";
        private DeviceInfo _currentInfo;

        public UdpClient(string multicastIp, int port)
        {
            var ip = IPAddress.Parse(multicastIp);
            _udpClient = new System.Net.Sockets.UdpClient();
            _udpClient.JoinMulticastGroup(ip);
            _endPoint = new IPEndPoint(ip, port);

            _timer = new Timer(30 * 1000); // 30 seconds
            _timer.AutoReset = true;
            _timer.Elapsed += Broadcast;
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void Submit(DeviceInfo info)
        {
            _currentInfo = info;
            BeginBroadcast();
        }

        public DeviceInfo GetFirstMatch(DeviceInfo info)
        {
            throw new System.NotImplementedException();
        }

        public void SetAvailibility(DeviceInfo device, bool availible)
        {
            if (availible && !_timer.Enabled)
            {
                _currentInfo = device;
                BeginBroadcast();
            }
            else if (!availible)
            {
                EndBroadcast();
            }
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
            var device = string.Format(DEVICE_STRING, _currentInfo.MobileOs, _currentInfo.OsVersion,
                                       _currentInfo.UniqueId, _currentInfo.IP);
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
