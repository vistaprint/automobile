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

AndroidBrowser.cs 
*/
using System;
using Android.Webkit;
using Automobile.Mobile.Framework.Browser;

namespace Automobile.Mobile.Android.Automation
{
    public class AndroidBrowser : MobileBrowser
    {
        public AndroidBrowser(WebView webView)
        {
            WebView = webView;
            InitWebView();
        }

        public AndroidChromeClient Chrome { get; private set; }

        public override string Url
        {
            get { return WebView.Url; }
            set { WebView.LoadUrl(value); }
        }

        /// <summary>
        /// Execute some javascript in the browser
        /// </summary>
        /// <param name="js">script to execute</param>
        /// <returns>return value of the script, if any</returns>
        public override string ExecJavascript(string js)
        {
            // There is a bug in several versions of the android api affecting the call to execute js in a webview
            // Because of this we execute the js as a url, and write to the console with a unique prefix to get the output back
            Chrome.ClearConsole();
            Url = string.Format("javascript:window.console.debug('{0}' + eval(\"{1}\"));", AndroidChromeClient.JS_PREFIX, js);

            try
            {
                // This timeout is chosen to be consistent with the equivalent timeout on the iOS side, which is imposed by apple
                Chrome.JsTrigger.WaitOne(10000);
            }
            catch (TimeoutException)
            {
                throw new TimeoutException(string.Format("Timed out waiting for javascript execution: {0} \n Console:\n{1}", js, Chrome.Console));
            }
            

            return Chrome.Return;
        }

        /// <summary>
        /// Navigate to a url optionally using the base url
        /// </summary>
        /// <param name="url">url to navigate to</param>
        /// <param name="useBaseUrl">toggle baseurl</param>
        public override void Navigate(string url, bool useBaseUrl)
        {
            if(useBaseUrl && !url.Contains("://"))
            {
                Url = BaseUrl + url;
            }
            else
            {
                Url = url;
            }         
            WaitForReady();
        }

        /// <summary>
        /// Wait for the browser to complete any loading
        /// </summary>
        public override void WaitForReady()
        {
            try
            {
                Client.LoadTrigger.WaitOne(10000);
            }
            catch (TimeoutException)
            {
                throw new TimeoutException("Timed out waiting for browser to be ready");
            }
        }

        /// <summary>
        /// Refresh the browser
        /// </summary>
        public override void Refresh()
        {
            WebView.Reload();
        }

        public WebView WebView { get; private set; }

        /// <summary>
        /// Initialize the WebView, WebClient, etc
        /// </summary>
        private void InitWebView()
        {
            // Load our custom web/chrome clients
            var client = new AndroidWebClient();
            Chrome = new AndroidChromeClient();
            Client = client;
            WebView.SetWebViewClient(client);
            WebView.SetWebChromeClient(Chrome);
            WebView.SetPictureListener(client);

            // Enable JS
            WebView.Settings.JavaScriptEnabled = true;

            // TODO: make configable
            // Don't save form data/passwords
            WebView.Settings.SaveFormData = false;
            WebView.Settings.SavePassword = false;

            // Clear cookies
            CookieSyncManager.CreateInstance(WebView.Context);
            CookieManager.Instance.RemoveAllCookie();
        }
    }
}