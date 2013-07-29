using Automobile.Mobile.Framework;
using Automobile.Mobile.Framework.Data;
using Automobile.Mobile.Framework.Device;

namespace Automobile.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            
            MobileDb.CreateRegistrarClient("http://localhost:8080", new JsonProvider());

            var match = MobileDb.Instance.GetFirstMatch(new DeviceInfo {MobileOs = MobileOs.Android});

            var device = new ProxyDevice(match.IP);
            device.Connect();
            device.Browser.Navigate("http://google.com/");
            device.Disconnect();
        }
    }
}
