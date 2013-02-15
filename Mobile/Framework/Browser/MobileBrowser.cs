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

MobileBrowser.cs 
*/
using System;

namespace Automobile.Mobile.Framework.Browser
{
    public abstract class MobileBrowser : IMobileBrowser
    {
        protected MobileBrowser() {}

        protected virtual string BaseUrl
        {
            get 
            {
                var url = new Uri(Url);
                return url.Scheme + "://" + url.Host;
            }
        }        
        
        public virtual void Navigate(string url)
        {
            Navigate(url, true);    
        }

        public abstract string Url { get; set; }

        public virtual IWebClient Client { get; protected set; }

        public abstract string ExecJavascript(string js);

        public abstract void Navigate(string url, bool useBaseUrl);
        public abstract void WaitForReady();
        public abstract void Refresh();
    }
}