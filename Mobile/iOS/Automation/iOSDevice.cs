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

iOSDevice.cs 
*/
using System;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using Automobile.Mobile.Framework.Device;
using Automobile.Mobile.Framework;
using Automobile.Mobile.Framework.Data;
using Newtonsoft.Json;
using System.Text;


namespace Automobile.Mobile.iOS.Automation
{
	public class iOSDevice : MobileDevice
	{
		private readonly UIDevice _device;
		
		public iOSDevice (UIDevice device, UIWebView webView, ConnectionType connType, string connString) : base(connType, connString, new JsonProvider())
		{
			_device = device;
			Browser = new iOSBrowser(webView);
			DeviceInfo = new DeviceInfo();
			DeviceInfo.MobileOs = MobileOs.iOS;
			DeviceInfo.OsVersion = _device.SystemVersion;
			DeviceInfo.DeviceModel = _device.Model;
			DeviceInfo.UniqueId = _device.IdentifierForVendor.ToString();
			DeviceInfo.IP = IP;
		}
		
		/// <summary>
		/// Gets the web view.
		/// </summary>
		/// <value>
		/// The web view.
		/// </value>
		public UIWebView WebView
		{
			get
			{
				return (Browser as iOSBrowser).WebView;
			}
		}
		
		/// <summary>
		/// Gets or sets the orientation.
		/// </summary>
		/// <value>
		/// The orientation.
		/// </value>
		public override Orientation Orientation 
		{
			get 
			{
				switch(_device.Orientation)
				{
					case UIDeviceOrientation.Portrait:
						return Orientation.Portrait;
					case UIDeviceOrientation.LandscapeRight:
						return Orientation.Landscape;
					default:
						return Orientation.Other;
				}
			}
			set 
			{
				UIDeviceOrientation orientation = UIDeviceOrientation.Unknown;
				Selector sel = new Selector("setOrientation:");
				switch(value)
				{

					case Orientation.Portrait:
						orientation = UIDeviceOrientation.Portrait;
						break;
					case Orientation.Landscape:
						orientation = UIDeviceOrientation.LandscapeRight;
						break;
					default:
						break;
				}
				Messaging.void_objc_msgSend_int(_device.Handle, sel.Handle, (int)orientation);
			}
		}
		
		/// <summary>
		/// Takes the screenshot.
		/// </summary>
		/// <returns>
		/// The screenshot.
		/// </returns>
		public override byte[] TakeScreenshot()
		{
			UIGraphics.BeginImageContext(WebView.Bounds.Size);
			var ctx = UIGraphics.GetCurrentContext();
			
			WebView.Layer.RenderInContext(ctx);
			var img = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
						
			return img.AsPNG().ToArray();
		}
		
		/// <summary>
		/// Gets the IP
		/// </summary>
		/// <value>
		/// The IP
		/// </value>
		private string IP
		{
			get { return Dns.GetHostAddresses(Dns.GetHostName()).First(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString(); }
		}
	}
}

