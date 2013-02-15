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

        /// <summary>
        /// Executed when activity is on the screen and active
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            // Start automation
            _autoThread.Start();
        }
    }
}

