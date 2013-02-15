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

AndroidDevice.cs 
*/
using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Net.Wifi;
using Android.OS;
using Android.Telephony;
using Android.Webkit;
using Android.Text.Format;
using Automobile.Mobile.Android.Config;
using Automobile.Mobile.Framework;
using Automobile.Mobile.Framework.Device;

namespace Automobile.Mobile.Android.Automation
{
    /// <summary>
    /// Represents the android device
    /// </summary>
    public class AndroidDevice : MobileDevice
    {
        private readonly Activity _activity;

        public AndroidDevice(Activity activity, ConnectionType connectionType, string connectionString) : base(connectionType, connectionString)
        {
            // Setup the WebView/Browser
            _activity = activity;
            Browser = new AndroidBrowser(new WebView(activity));
            _activity.SetContentView(WebView);
            
            // Populate device info
            Wifi = (WifiManager)activity.GetSystemService(Context.WifiService);
            DeviceInfo = new DeviceInfo();
            DeviceInfo.MobileOs = MobileOs.Android;
            DeviceInfo.DeviceModel = Build.Model;
            DeviceInfo.OsVersion = Build.VERSION.Release;
            DeviceInfo.IP = IP;
            var tm = (TelephonyManager)activity.GetSystemService(Context.TelephonyService);
            DeviceInfo.UniqueId = tm.DeviceId;   
        } 

        public WebView WebView
        {
            get { return (Browser as AndroidBrowser).WebView; }
        }

        public WifiManager Wifi { get; private set; }

        public override Orientation Orientation
        {
            get
            {
                switch(_activity.RequestedOrientation)
                {
                    case ScreenOrientation.Portrait:
                        return Orientation.Portrait;
                    case ScreenOrientation.Landscape:
                        return Orientation.Landscape;
                    default:
                        return Orientation.Other;
                }
            }
            set
            {
                ScreenOrientation orientation;
                switch(value)
                {
                    case Orientation.Portrait:
                        orientation = ScreenOrientation.Portrait;
                        break;
                    case Orientation.Landscape:
                        orientation = ScreenOrientation.Landscape;
                        break;
                    case Orientation.Other:
                        orientation = ScreenOrientation.Unspecified;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value");
                }
                _activity.RequestedOrientation = orientation;
            }
        }

        public override void BeginAutomation()
        {
            try
            {
                base.BeginAutomation();
            }
            catch (Exception e)
            {
                // This block is useful for debugging
                // The framework is targeted for .net and not mono so this is the first place we can catch an exception there
                throw e;
            }
            
        }

        /// <summary>
        /// Take a screenshot of the device
        /// </summary>
        /// <returns>array of bytes of the screenshot</returns>
        public override byte[] TakeScreenshot()
        {
            // Force a regen of the drawing cache
            WebView.DestroyDrawingCache();
            WebView.BuildDrawingCache();
            // Copy the cache to a new bitmap
            var bmp = WebView.DrawingCache.Copy(WebView.DrawingCache.GetConfig(), true);

            // Compress the bitmap into a png stream
            var stream = new MemoryStream();
            bmp.Compress(Bitmap.CompressFormat.Png, 100, stream); // Quality is ignored on a png
            
            // Return bytes out of the stream
            return stream.GetBuffer();
        }

        /// <summary>
        /// Get the IP address from the android OS
        /// </summary>
        private string IP
        {
            get 
            { 
                return Formatter.FormatIpAddress(Wifi.ConnectionInfo.IpAddress);
            }
        }
    }
}