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

IMobileBrowser.cs 
*/
namespace Automobile.Mobile.Framework.Browser
{
    /// <summary>
    /// A browser on a mobile device
    /// </summary>
    public interface IMobileBrowser
    {
        string Url { get; set; }
        string ExecJavascript(string js);
        void Navigate(string url);
        void Navigate(string url, bool useBaseUrl);
        void WaitForReady();
        void Refresh();
    }
}