using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Automobile.Mobile.Framework.Device;

namespace Automobile.Mobile.Framework.Data
{
    public class NullClient : IMobileDb
    {
        public void Submit(DeviceInfo device) { }

        public DeviceInfo GetFirstMatch(DeviceInfo device) { return null; }

        public void SetAvailibility(DeviceInfo device, bool availible){ }

        public void Dispose(){ }
    }
}
