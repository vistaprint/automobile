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

AppDelegate.cs 
*/
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using iOS.Automation;
using Automobile.Mobile.Framework;

namespace iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		private UIWindow window;

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// Load settings
			var conn = NSUserDefaults.StandardUserDefaults.StringForKey("connType");
			var connType = conn == null ? ConnectionType.DbRegistration : (ConnectionType) Enum.Parse(typeof(ConnectionType), conn);
			var connString = NSUserDefaults.StandardUserDefaults.StringForKey("connString") ?? "Server=dbinfra.vistaprint.net;Database=QAplatform;UID=qaplatform;PWD=@utom@t3QA";

			// Init config controller
			var webController = new iOSWebViewController(connType, connString);

			// init the root controller
			var rootController = new UINavigationController();
			rootController.NavigationBarHidden = true;
			rootController.PushViewController(webController, false);

			// init the window, add the root controller
			window = new UIWindow(UIScreen.MainScreen.Bounds);
			window.RootViewController = rootController;
			window.MakeKeyAndVisible();

			return true;
		}
	}
}

