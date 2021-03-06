﻿/*
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

RegistrarClient.cs 
*/

using System.IO;
using System.Net;
using System.Text;

namespace Automobile.Mobile.Framework.Data
{
    public class RegistrarClient : IMobileDb
    {
        private readonly string _baseUrl;
        private readonly IJsonProvider _json;

        public RegistrarClient(string baseUrl, IJsonProvider json)
        {
            _baseUrl = baseUrl;
            _json = json;
        }

        public void Dispose() { }

        public void Register(DeviceInfo device)
        {
            var request = WebRequest.Create(_baseUrl + "/registrar/device");
            request.Method = "PUT";
            request.ContentType = "application/json";
            var str = _json.Serialize(device);
            var count = Encoding.ASCII.GetByteCount(str);
            request.ContentLength = count;
            request.GetRequestStream().Write(Encoding.ASCII.GetBytes(str), 0, count);
            request.GetResponse();
        }

        public DeviceInfo GetFirstMatch(DeviceInfo device)
        {
            var url = _baseUrl +
                      string.Format(
                          "/registrar/device?MobileOs={0}&DeviceModel={1}&OsVersion={2}&UniqueId={3}",
                          device.MobileOs, device.DeviceModel, device.OsVersion, device.UniqueId);

            var request = WebRequest.Create(url);
            try
            {
                var response = request.GetResponse();            
                var responseStream = response.GetResponseStream();
                if(responseStream == null)
                {
                    return null;
                }
                var str = new StreamReader(responseStream).ReadToEnd();
                return _json.Deserialize<DeviceInfo>(str);
            }
            catch(WebException e)
            {
                if ((e.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw;
            }
        }

        public DeviceInfo GetFirstMatch(DeviceInfo device, bool filterByAvailible)
        {
            throw new System.NotImplementedException();
        }

        public void SetAvailibility(DeviceInfo device, bool availible)
        {
            var request = WebRequest.Create(string.Format("{0}/registrar/device/{1}?availible={2}", _baseUrl, device.UniqueId, availible));
            request.GetResponse();
        }
    }
}