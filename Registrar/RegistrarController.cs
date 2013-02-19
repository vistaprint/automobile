using System.Collections.Generic;
using System.Web.Http;
using Automobile.Mobile.Framework;
using Automobile.Mobile.Framework.Data;
using Automobile.Mobile.Framework.Device;

namespace Automobile.Registrar
{
    public class RegistrarController : ApiController 
    {
        public IEnumerable<DeviceInfo> GetAllDevices()
        {
            return new List<DeviceInfo> { new DeviceInfo {DeviceModel = "Model", IP = "255.255.255.255", MobileOs = MobileOs.None, OsVersion = "1", UniqueId = "0"}};
        }

        public DeviceInfo GetDeviceById(string id)
        {
            return MobileDb.Instance.GetFirstMatch(new DeviceInfo {UniqueId = id});
        }

         public void PutDevice(DeviceInfo device)
         {
             MobileDb.Instance.Submit(device);
         }
    }
}