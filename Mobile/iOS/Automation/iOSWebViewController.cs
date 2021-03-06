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

iOSWebViewController.cs 
*/
using System;
using System.Threading;
using MonoTouch.UIKit;
using Automobile.Mobile.Framework;

namespace Automobile.Mobile.iOS.Automation
{
	public class iOSWebViewController : UIViewController
	{
		public iOSWebViewController(ConnectionType connType, string connString)
		{
			// Initialize the webview and add it to this view
			var web = new UIWebView(UIScreen.MainScreen.Bounds);
			Add(web);

			// Create the device and start the automation thread
			var device = new iOSDevice(UIDevice.CurrentDevice, web, connType, connString);
			new Thread(device.BeginAutomation).Start();
		}
	}
}

