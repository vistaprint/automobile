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

iOSBrowser.cs 
*/
using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Automobile.Mobile.Framework.Browser;

namespace iOS.Automation
{
	public class iOSBrowser : MobileBrowser
	{
		private readonly UIWebView _webView;
		private readonly NSObject nso = new NSObject(); // spare object so we can use InvokeOnMainThread

		public iOSBrowser(UIWebView webview)
		{
			_webView = webview;
			Client = new iOSWebClient(_webView);
		}

		/// <summary>
		/// The underlying native browser
		/// </summary>
		public UIWebView WebView { get { return _webView; } }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		public override string Url
		{
			get
			{
				// Anything to to with UI has to go on the main thread
				string url = "";
				nso.InvokeOnMainThread(delegate
                {
					url = _webView.Request.Url.ToString();
				});
				
				return url;
			}
			set
			{
				Navigate(value);
			}
		}

		private string JsReturn { get; set; }

		/// <summary>
		/// Executes some javascript in the browser
		/// </summary>
		/// <returns>
		/// Return value of the javascript
		/// </returns>
		/// <param name='js'>
		/// Javascript to execute
		/// </param>
        public override string ExecJavascript(string js)
		{
			// Anything to to with UI has to go on the main thread
			nso.InvokeOnMainThread(delegate
			{
				try
				{
					// This has a 10 second timeout
					JsReturn = _webView.EvaluateJavascript(js);
				}
				catch
				{
					JsReturn = null;
				}
			});

			if(JsReturn == null)
			{
				throw new TimeoutException("Timed out waiting for javascript execution");
			}

			return JsReturn;
		}

		/// <summary>
		/// Refresh the page.
		/// </summary>
		public override void Refresh()
		{
			_webView.Reload();
		}

		/// <summary>
		/// Navigate to specified url.
		/// </summary>
		/// <param name='url'>
		/// URL.
		/// </param>
		/// <param name='useBaseUrl'>
		/// Use the base url or not
		/// </param>
        public override void Navigate(string url, bool useBaseUrl)
		{
			// Anything to to with UI has to go on the main thread
			nso.InvokeOnMainThread(delegate
           	{
				if(useBaseUrl && !url.Contains("://"))
				{
					url = BaseUrl + url; 
				}
				var request = new NSUrlRequest(new NSUrl(url));
				_webView.LoadRequest(request);
			});

			WaitForReady();
		}

		/// <summary>
		/// Waits for the browser to be ready.
		/// </summary>
        public override void WaitForReady()
		{
			Client.LoadTrigger.WaitOne();
		}
		

	}
}

