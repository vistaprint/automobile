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

ConfigActivity.cs 
*/
using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Automobile.Mobile.Android.Automation;
using Automobile.Mobile.Framework;

namespace Automobile.Mobile.Android.Config
{
    [Activity(Label = "Automation Config")]
    public class ConfigActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            var prefs = GetSharedPreferences("AutomationConfig", FileCreationMode.WorldWriteable);
            var startAutomation = FindViewById<Button>(Resource.Id.StartAutomation);
            var dbButton = FindViewById<RadioButton>(Resource.Id.dbButton);
            var directButton = FindViewById<RadioButton>(Resource.Id.directButton);
            var connStringText = FindViewById<TextView>(Resource.Id.ConnectionStringTextView);
            var connTypeKey = GetString(Resource.String.ConnectionType);
            var connStringKey = GetString(Resource.String.ConnectionString);

            if (!prefs.Contains(connTypeKey))
            {
                prefs.Edit().PutInt(connTypeKey, (int)ConnectionType.Direct).Commit();
            }

            dbButton.Click += (sender, args) =>
                              {
                                  var view =
                                      FindViewById<LinearLayout>(Resource.Id.ServerLayout).Visibility =
                                      ViewStates.Visible;
                                  prefs.Edit().PutInt(connTypeKey, (int)ConnectionType.DbRegistration)
                                                .PutString(connStringKey, connStringText.Text).Commit();
                              };
            directButton.Click += (sender, args) =>
                               {
                                   var view =
                                       FindViewById<LinearLayout>(Resource.Id.ServerLayout).Visibility =
                                       ViewStates.Gone;
                                   prefs.Edit().PutInt(connTypeKey, (int)ConnectionType.Direct).
                                       Commit();
                               };
            startAutomation.Click += (sender, args) =>
                                     {
                                         if(dbButton.Checked)
                                         {
                                             prefs.Edit().PutString(connStringKey, connStringText.Text).Commit();
                                         }
                                         StartActivity(typeof (AutomationActivity));
                                     };

            
        }
    }
}