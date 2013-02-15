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

AndroidWebClient.cs 
*/
using System.Threading;
using Android.Graphics;
using Android.Webkit;
using Automobile.Mobile.Framework.Browser;

namespace Automobile.Mobile.Android.Automation
{
    /// <summary>
    /// Handles events originating from a webview
    /// </summary>
    public class AndroidWebClient : WebViewClient, WebView.IPictureListener, IWebClient
    {
        private bool _set;

        public AndroidWebClient()
        {
            LoadTrigger = new EventWaitHandle(true, EventResetMode.ManualReset);
            _set = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public EventWaitHandle LoadTrigger { get; private set; }

        /// <summary>
        /// Webview recieves a SSL error,
        /// Proceeds past any error
        /// </summary>
        /// <param name="view"></param>
        /// <param name="handler"></param>
        /// <param name="error"></param>
        public override void OnReceivedSslError(WebView view, SslErrorHandler handler, global::Android.Net.Http.SslError error)
        {
            // TODO: configable
            handler.Proceed();
        }

        /// <summary>
        /// Page has began loading
        /// </summary>
        /// <param name="view"></param>
        /// <param name="url"></param>
        /// <param name="favicon"></param>
        public override void OnPageStarted(WebView view, string url, global::Android.Graphics.Bitmap favicon)
        {
            LoadTrigger.Reset();
            _set = false;
            base.OnPageStarted(view, url, favicon);
        }
        
        /// <summary>
        /// A new picture is rendered to the screen
        /// </summary>
        /// <param name="view"></param>
        /// <param name="picture"></param>
        public void OnNewPicture(WebView view, Picture picture)
        {
            if (!_set)
            {
                // TODO: only add if not there, also include jquery mobile
                view.LoadUrl(@"javascript:!function(){var jsFile = window.document.createElement('script');
                        jsFile.setAttribute('type', 'text/javascript');
                        jsFile.setAttribute('src', '//ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js');
                        window.document.getElementsByTagName('head')[0].appendChild(jsFile);}()");

                LoadTrigger.Set();
                _set = false;
            }
        }
    }
}