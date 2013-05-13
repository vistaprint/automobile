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

AutomationActivity.cs 
*/
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Automobile.Mobile.Android.Config;
using Automobile.Mobile.Framework;

namespace Automobile.Mobile.Android.Automation
{
    /// <summary>
    /// Main class for an android application
    /// </summary>
    [Activity(Label = "Automation", MainLauncher = true, Icon = "@drawable/phone_icon", Theme = "@android:style/Theme.NoTitleBar", ConfigurationChanges = ConfigChanges.Orientation)]
    public class AutomationActivity : Activity
    {
        private AndroidDevice _device;
        private Thread _autoThread;
        private string _connTypeKey;
        private string _connStringKey;
        private PowerManager.WakeLock _wakeLock;
        private WifiManager.WifiLock _wifiLock;

        /// <summary>
        /// Executed when activity is started
        /// </summary>
        /// <param name="bundle">Can be used to restore activity to a previous saved state</param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _connTypeKey = GetString(Resource.String.ConnectionType);
            _connStringKey = GetString(Resource.String.ConnectionString);
            var prefs = GetSharedPreferences("AutomationConfig", FileCreationMode.WorldWriteable);

            // No connection type set, launch the config screen
            if(!prefs.Contains(_connTypeKey))
            {
                StartActivity(typeof (ConfigActivity));
                Finish();
                return;
            }

            // Init device
            _device = new AndroidDevice(this, 
                (ConnectionType) prefs.GetInt(_connTypeKey, (int)ConnectionType.Direct),
                prefs.GetString(_connStringKey, ""));

            // Make an automation thread, so it doesn't block UI
            _autoThread = new Thread(_device.BeginAutomation);
        }

        protected override void OnStart()
        {
            base.OnStart();
            // Keep screen and wifi awake
            var powerManager = (PowerManager)GetSystemService(PowerService);
            var wifiManager = (WifiManager)GetSystemService(WifiService);
            _wakeLock = powerManager.NewWakeLock(WakeLockFlags.Full, "no sleep");
            _wakeLock.Acquire();
            _wifiLock = wifiManager.CreateWifiLock(WifiMode.FullHighPerf, "wifi on");
            _wifiLock.Acquire();
        }

        /// <summary>
        /// Executed when activity is on the screen and active
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            // Start automation
            if(_autoThread.ThreadState != ThreadState.Running)
            {
                _autoThread.Start();
            }        
        }

        protected override void OnStop()
        {
            base.OnStop();
            // This app can't do it's job in the background. Clean up and kill the activity.
            // Release locks
            _wakeLock.Release();
            _wifiLock.Release();
            // Stop the automation thread
            _autoThread.Abort();
            _autoThread.Join();
            // Kill the activity
            Finish();
        }
    }
}