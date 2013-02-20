using System.Collections.Generic;
using System.Web.Http;
using Automobile.Mobile.Framework;
using Automobile.Mobile.Framework.Data;
using Automobile.Mobile.Framework.Device;

namespace Automobile.Registrar
{
    public class RegistrarController : ApiController 
    {
        public DeviceInfo GetDeviceById(string id)
        {
            return MobileDb.Instance.GetFirstMatch(new DeviceInfo {UniqueId = id});
        }

        public DeviceInfo GetFirstMatch(DeviceInfo device)
        {
            return MobileDb.Instance.GetFirstMatch(device);
        }

        public void PutAvailibility(string id, bool availible)
        {
            MobileDb.Instance.SetAvailibility(new DeviceInfo { UniqueId = id}, availible);
        }

        public void PutDevice(DeviceInfo device)
        {
            MobileDb.Instance.Submit(device);
        }
    }
}