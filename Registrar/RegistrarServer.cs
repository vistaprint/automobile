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

RegistrarServer.cs 
*/
using System;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Automobile.Mobile.Framework.Data;

namespace Automobile.Registrar
{
    public class RegistrarServer : IDisposable
    {
        private readonly HttpSelfHostServer _server;

        public RegistrarServer(string baseAddress, string dbFile)
        {
            var config = new HttpSelfHostConfiguration(baseAddress);

            config.Routes.MapHttpRoute(
                "Registrar API", "registrar/{controller}/{id}",
                new { id = RouteParameter.Optional });

            _server = new HttpSelfHostServer(config);         
            MobileDb.Instance = new SQLiteClient(dbFile);       
            _server.OpenAsync().Wait();

        }

        public void Dispose()
        {
            _server.Dispose();
            MobileDb.Instance.Dispose();
        }
    }
}