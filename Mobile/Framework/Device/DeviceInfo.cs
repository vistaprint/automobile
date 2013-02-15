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

DeviceInfo.cs 
*/
namespace Automobile.Mobile.Framework.Device
{
    /// <summary>
    /// Collection of information of a device
    /// Used to register a device IP
    /// </summary>
    public class DeviceInfo
    {
        public DeviceInfo() {}

        public MobileOs MobileOs { get; set; }
        public string DeviceModel { get; set; }
        public string OsVersion { get; set; }
        public string UniqueId { get; set; }
        public string IP { get; set; }
    }
}