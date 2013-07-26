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

StatusHandler.cs 
*/

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Automobile.Registrar
{
    public class StatusHandler : HttpMessageHandler 
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() => Handle(request), cancellationToken);
        }
        
        private HttpResponseMessage Handle(HttpRequestMessage request)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            return response;
        }
    }
}