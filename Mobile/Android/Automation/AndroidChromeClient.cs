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

AndroidChromeClient.cs 
*/
using System.Threading;
using Android.Webkit;

namespace Automobile.Mobile.Android.Automation
{
    public class AndroidChromeClient : WebChromeClient
    {
        public const string JS_PREFIX = "MOBILERC:";

        public AndroidChromeClient()
        {
            JsTrigger = new EventWaitHandle(false, EventResetMode.AutoReset);
        }


        public EventWaitHandle JsTrigger { get; private set; }
        public string Return { get; private set; }
        public string Console { get; private set; }


        public override bool OnConsoleMessage(ConsoleMessage consoleMessage)
        {
            if (consoleMessage.Message().Contains(JS_PREFIX))
            {
                Return = consoleMessage.Message().Replace(JS_PREFIX, string.Empty);
                JsTrigger.Set();           
            }
            else
            {
                Console += consoleMessage.Message() + "\n";
            }
            
            
            return true;
        }

        public void ClearConsole()
        {
            Console = string.Empty;
        }
    }
}