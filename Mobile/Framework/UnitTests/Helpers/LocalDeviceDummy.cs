using System.Threading;
using Automobile.Mobile.Framework.Device;

namespace Automobile.Mobile.Framework.UnitTests.Helpers
{
    public class MobileDeviceDummy : MobileDevice
    {
        private Thread _thread;

        public MobileDeviceDummy() : base(ConnectionType.Direct, "")
        {
            Browser = new MobileBrowserDummy();
        }

        public override void BeginAutomation()
        {
            _thread = new Thread(base.BeginAutomation);
            _thread.Start();
            while (!_thread.IsAlive);
        }

        public void Shutdown()
        {
            _thread.Abort();
        }

        public override Orientation Orientation { get; set; }

        public bool Screenshot { get; set; }

        public override byte[] TakeScreenshot()
        {
            Screenshot = true;
			return null;
        }
    }
}