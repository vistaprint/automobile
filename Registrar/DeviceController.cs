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

DeviceController.cs 
*/
using System.Net;
using System.Web.Http;
using Automobile.Mobile.Framework.Data;

namespace Automobile.Registrar
{
    public class DeviceController : ApiController 
    {
        public DeviceInfo GetDeviceById(string id)
        {
            var d = MobileDb.Instance.GetFirstMatch(new DeviceInfo {UniqueId = id});
            if(d == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return d;
        }

        public DeviceInfo GetFirstMatch([FromUri]DeviceInfo device)
        {
            var d = MobileDb.Instance.GetFirstMatch(device);
            if (d == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return d;
        }

        public void GetAvailibility(string id, bool availible)
        {
            var d = MobileDb.Instance.GetFirstMatch(new DeviceInfo { UniqueId = id }, false);
            if (d == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            MobileDb.Instance.SetAvailibility(d, availible);
        }

        public void PutDevice(DeviceInfo device)
        {
            MobileDb.Instance.Register(device);
        }
    }
}