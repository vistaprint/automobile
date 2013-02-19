﻿using System;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Automobile.Mobile.Framework.Data;

namespace Automobile.Registrar
{
    class Program
    {
        static void Main(string[] args)
        {
            MobileDb.Instance = new SQLiteClient("MobileDB.sqlite");

            var config = new HttpSelfHostConfiguration("http://localhost:8080");

            config.Routes.MapHttpRoute(
                "API Default", "{controller}/{id}",
                new { id = RouteParameter.Optional });

            using (HttpSelfHostServer server = new HttpSelfHostServer(config))
            using (MobileDb.Instance = new SQLiteClient("MobileDB.sqlite"))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("Press Enter to quit.");
                Console.ReadLine();
            }
        }
    }
}
