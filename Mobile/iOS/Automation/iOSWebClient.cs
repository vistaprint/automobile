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

iOSWebClient.cs 
*/
using System;
using System.Timers;
using System.Threading;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Automobile.Mobile.Framework.Browser;

namespace Automobile.Mobile.iOS.Automation
{
	/// <summary>
	/// Handles events from a webview
	/// </summary>
	public class iOSWebClient : NSUrlConnectionDelegate, IWebClient
	{
		private readonly System.Timers.Timer _timer;
		
		// for debugging,
		// (starts == finishes + errors) should be true
		private int _starts = 0;
		private int _finishes = 0;
		private int _errors = 0;
		
		private bool _auth = false;
		private UIWebView _view;
		 
		public iOSWebClient (UIWebView view)
		{
			// syncs ready state across threads
			LoadTrigger = new EventWaitHandle(true, EventResetMode.ManualReset);

			// Set event handlers
			view.LoadStarted += LoadStarted;
			view.LoadFinished += LoadFinished;
			view.LoadError += LoadError;
			view.ShouldStartLoad = ShouldStartLoad;
			_view = view;
			
			// Used to wait a (few) sec on page load in case of a redirect
			// TODO: make config-able? better way to do this?
			_timer = new System.Timers.Timer(2000);
			_timer.Elapsed += (sender, e) => LoadTrigger.Set();

		}

		public EventWaitHandle LoadTrigger { get; private set; }

		/// <summary>
		/// Called when a page starts loading
		/// </summary>
		private void LoadStarted(object o, EventArgs args)
		{
			_timer.Stop();
			LoadTrigger.Reset();
			_starts++;
		}
		
		/// <summary>
		/// Load is finished.
		/// </summary>
		private void LoadFinished(object o, EventArgs args)
		{
			_timer.Start();
			_finishes++;
		}

		/// <summary>
		/// Error loading, load did not finish
		/// </summary>
		private void LoadError(object o, EventArgs e)
		{	
			_timer.Start();
			_errors++;
		}

		/// <summary>
		/// Start a connection to do the https auth, and cancel the webview request if https
		/// </summary>
		private bool ShouldStartLoad(UIWebView view, NSUrlRequest request, UIWebViewNavigationType navType)
		{
			if(_auth || request.Url.Scheme.ToLower() != "https")
			{
				return true;
			}

			new NSUrlConnection(request, this, true);

			return false;
		}

		/// <summary>
		/// Handle the auth challenge on the connection
		/// </summary>
		public override void ReceivedAuthenticationChallenge (NSUrlConnection connection, NSUrlAuthenticationChallenge challenge)
		{
			if(challenge.PreviousFailureCount == 0)
			{
				var cred = NSUrlCredential.FromTrust(challenge.ProtectionSpace.ServerTrust);
				challenge.Sender.UseCredentials(cred, challenge);
			}
			else
			{
				challenge.Sender.CancelAuthenticationChallenge(challenge);
			}
		}

		/// <summary>
		/// Should be authed now, start up the webview request again
		/// </summary>
		public override void ReceivedResponse (NSUrlConnection connection, NSUrlResponse response)
		{
			_auth = true;
			_view.LoadRequest(connection.CurrentRequest);
			connection.Cancel();
		}

		/// <summary>
		/// Tells the connection it can authenticate
		/// </summary>
		public override bool CanAuthenticateAgainstProtectionSpace (NSUrlConnection connection, NSUrlProtectionSpace protectionSpace)
		{
			return protectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodServerTrus;
		}
	}
}

