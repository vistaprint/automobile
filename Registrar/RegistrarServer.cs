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
                "API Default", "{controller}/{id}",
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