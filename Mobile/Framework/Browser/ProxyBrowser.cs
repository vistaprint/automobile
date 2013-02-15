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

RemoteBrowser.cs 
*/
using Automobile.Mobile.Framework.Commands;
using Automobile.Mobile.Framework.Device;

namespace Automobile.Mobile.Framework.Browser
{
    /// <summary>
    /// Browser on a remote device
    /// </summary>
    public class ProxyBrowser : IMobileBrowser
    {
        private readonly IProxyDevice _owner;

        public ProxyBrowser(IProxyDevice owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Execute javascript on the remote device
        /// </summary>
        /// <param name="js">Javascript to execute</param>
        /// <returns>Evaluated value of the javascript statement</returns>
        public string ExecJavascript(string js)
        {
            return _owner.SendCommand<string>(new ExecJavascriptCommand(js));
        }

        /// <summary>
        /// Navigate to a url
        /// </summary>
        /// <param name="url">url to navigate to</param>
        public void Navigate(string url)
        {
            Navigate(url, true);
        }

        /// <summary>
        /// Navigate to a url
        /// </summary>
        /// <param name="url">url to navigate to</param>
        /// <param name="useBaseUrl">use the base Url when navigating</param>
        public void Navigate(string url, bool useBaseUrl)
        {
            _owner.SendCommand(new UrlCommand(url));
            WaitForReady();
        }

        /// <summary>
        /// Wait for the browser to be ready.
        /// This means a page is loaded and drawn to the screen.
        /// </summary>
        public void WaitForReady()
        {
            _owner.SendCommand(new WaitForReadyCommand());
        }

        public void Refresh()
        {
            _owner.SendCommand(new RefreshCommand());
        }

        /// <summary>
        /// Current URL for the browser
        /// </summary>
        public string Url 
        { 
            get
            {
                var url = _owner.SendCommand<string>(new UrlCommand()) ?? "";
                return url;
            }  
            set
            {
                Navigate(value);
            } 
        }
    }
}