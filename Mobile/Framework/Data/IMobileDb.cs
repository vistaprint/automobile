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

IMobileDb.cs 
*/

using System;

namespace Automobile.Mobile.Framework.Data
{
    public interface IMobileDb : IDisposable
    {
        /// <summary>
        /// Submit a new device info, and marks it availible for automation
        /// </summary>
        /// <param name="device">info for a new device</param>
        void Register(DeviceInfo device);

        /// <summary>
        /// Returns the IP of the first registered device which matches all the info.
        /// Null values are ignored in the match, only searches availible devices
        /// </summary>
        /// <param name="device">Info to match</param>
        /// <returns>Full info of first match</returns>
        DeviceInfo GetFirstMatch(DeviceInfo device);

        /// <summary>
        /// Returns the IP of the first registered device which matches all the info.
        /// Null values are ignored in the match
        /// </summary>
        /// <param name="device">Info to match</param>
        /// <param name="filterByAvailible">if true, only return availible devices</param>
        /// <returns>Full info of first match</returns>
        DeviceInfo GetFirstMatch(DeviceInfo device, bool filterByAvailible);

        /// <summary>
        /// Set if a device is availible for automation (default for a new device is true)
        /// </summary>
        /// <param name="device">info for device to set availibility</param>
        /// <param name="availible"></param>
        void SetAvailibility(DeviceInfo device, bool availible);


    }
}