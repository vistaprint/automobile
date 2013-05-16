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
        private const string DEVICE_STRING = "MobileOS:{0},OsVersion:{1},UniqueId:{2},IP:{3}";
        private DeviceInfo _currentInfo;
        private IJsonProvider _json;

        public UdpClient(string multicastIp, int port, IJsonProvider json)
        {
            var ip = IPAddress.Parse(multicastIp);
            _endPoint = new IPEndPoint(ip, port);
            _udpClient = new System.Net.Sockets.UdpClient(port);
            _udpClient.JoinMulticastGroup(ip);

            _timer = new Timer(30 * 1000); // 30 seconds
            _timer.AutoReset = true;
            _timer.Elapsed += Broadcast;

            _json = json;
        }

        public void Dispose()
        {
            _udpClient.DropMulticastGroup(_endPoint.Address);
            _udpClient.Close();
        }

        public void Register(DeviceInfo info)
        {
            _currentInfo = info;
            BeginBroadcast();
        }

        public DeviceInfo GetFirstMatch(DeviceInfo info)
        {
            return GetFirstMatch(info, true);
        }

        public DeviceInfo GetFirstMatch(DeviceInfo device, bool filterByAvailible)
        {
            if (!filterByAvailible)
            {
                throw new Exception("Unavailible devices cannot be searched with the UdpClient.");
            }

            var time = DateTime.Now;

            while (DateTime.Now - time < TimeSpan.FromSeconds(45))
            {
                var async = _udpClient.BeginReceive(null, null);

                if (async.AsyncWaitHandle.WaitOne())
                {
                    IPEndPoint remoteEp = null;
                    var bytes = _udpClient.EndReceive(async, ref remoteEp);
                    var encoding = new ASCIIEncoding();
                    var found = _json.Deserialize<DeviceInfo>(encoding.GetString(bytes));
                    if (IsMatch(device, found))
                    {
                        return found;
                    }
                }
            }

            return null;
        }

        public void SetAvailibility(DeviceInfo device, bool availible)
        {
            if (_currentInfo == null)
            {
                throw new Exception("Cannot set availibility: No device registered with this UdpClient instance.");
            }

            if (_currentInfo != device)
            {
                throw new Exception("Cannot set availibility: Device does not match registered device; register a new device before setting availibility.");
            }

            if (availible && !_timer.Enabled)
            {
                BeginBroadcast();
            }
            else if (!availible)
            {
                EndBroadcast();
            }
        }

        private bool IsMatch(DeviceInfo orignal, DeviceInfo match)
        {
            return (orignal.MobileOs == null || orignal.MobileOs == match.MobileOs) &&
                    (orignal.OsVersion == null || orignal.OsVersion == match.OsVersion) &&
                    (orignal.UniqueId == null || orignal.UniqueId == match.UniqueId);
        }

        private void BeginBroadcast()
        {
            Broadcast(null, null);
            _timer.Start();
        }

        private void Broadcast(object sender, ElapsedEventArgs args)
        {
            var device = _json.Serialize(_currentInfo);
            var encoding = new ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(device);
            _udpClient.Send(bytes, bytes.Length, _endPoint);
        }

        private void EndBroadcast()
        {
            _timer.Stop();
        }
    }
}
