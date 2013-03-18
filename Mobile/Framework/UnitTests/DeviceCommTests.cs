using NUnit.Framework;
using Automobile.Mobile.Framework.Device;
using Automobile.Mobile.Framework.UnitTests.Helpers;

namespace Automobile.Mobile.Framework.UnitTests
{
    [TestFixture]
    public class DeviceCommTests
    {
        public MobileDeviceDummy Local { get; set; }
        public ProxyDevice Remote { get; set; }

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            Local = new MobileDeviceDummy();
            Local.BeginAutomation();
        }

        [SetUp]
        public void Setup()
        {
            Remote = new ProxyDevice("127.0.0.1");
            Remote.Connect();
        }

        [TearDown]
        public void Teardown()
        {
			if(Remote != null)
			{
				Remote.Disconnect();
			}
            
        }

        [TestFixtureTearDown]
        public void FixtureTeardown()
        {
            Local.Shutdown();
        }

        [Test]
        public void DeviceConnectionTest()
        {
            Assert.IsTrue(Remote.Connected);
        }

        [Test]
        public void DeviceScreenShotTest()
        {
           Remote.TakeScreenshot();
           Assert.IsTrue(Local.Screenshot);
        }

        [Test]
        public void DeviceOrientationTest()
        {
            Remote.Orientation = Orientation.Portrait;
            Assert.AreEqual(Orientation.Portrait, Local.Orientation);
            Assert.AreEqual(Remote.Orientation, Local.Orientation);
            Remote.Orientation = Orientation.Landscape;
            Assert.AreEqual(Orientation.Landscape, Local.Orientation);
            Assert.AreEqual(Remote.Orientation, Local.Orientation);
        }

        [Test]
        public void BrowserUrlTest()
        {
            const string url1 = @"http://www.google.com/";
            Remote.Browser.Navigate(url1);
            Assert.AreEqual(url1, Local.Browser.Url);
            Assert.AreEqual(url1, Remote.Browser.Url);
            const string url2 = @"http://www.vistaprint.com/";
            Remote.Browser.Url = url2;
            Assert.AreEqual(url2, Local.Browser.Url);
            Assert.AreEqual(url2, Remote.Browser.Url);
        }

        [Test]
        public void BrowserJsTest()
        {
            const string js = @"alert('Hi');";
            var ret = Remote.Browser.ExecJavascript(js);
            Assert.AreEqual(js, ret);
            Assert.AreEqual(js, (Local.Browser as MobileBrowserDummy).Js);
        }
    }
}
