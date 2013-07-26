using Automobile.Mobile.Framework;
using Automobile.Mobile.Framework.Data;

namespace Automobile.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            
            MobileDb.CreateRegistrarClient("http://localhost:8080", new JsonProvider());

            var device = new DeviceInfo {UniqueId = "id"};
            MobileDb.Instance.Register(device);

            MobileDb.Instance.SetAvailibility(device, false);
        }
    }
}
