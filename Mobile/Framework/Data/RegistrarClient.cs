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

RegistrarClient.cs 
*/

using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using Automobile.Mobile.Framework.Device;

namespace Automobile.Mobile.Framework.Data
{
    public class RegistrarClient : IMobileDb
    {
        private readonly string _baseUrl;
        private readonly DataContractJsonSerializer _serializer;

        public RegistrarClient(string baseUrl)
        {
            _baseUrl = baseUrl;
            _serializer = new DataContractJsonSerializer(typeof(DeviceInfo));
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void Submit(DeviceInfo device)
        {
            var request = WebRequest.Create(_baseUrl + @"/registrar");
            _serializer.WriteObject(request.GetRequestStream(), device);

            request.ContentLength = request.GetRequestStream().Length;
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.GetResponse();
        }

        public DeviceInfo GetFirstMatch(DeviceInfo device)
        {
            throw new System.NotImplementedException();
        }

        public void SetAvailibility(DeviceInfo device, bool availible)
        {
            throw new System.NotImplementedException();
        }
    }
}