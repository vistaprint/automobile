using Automobile.Mobile.Framework;
using Automobile.Mobile.Framework.Data;
using Automobile.Mobile.Framework.Device;
using NUnit.Framework;

namespace Automobile.Registrar.UnitTests
{
    [TestFixture]
    public class WebApiTests
    {
        public RegistrarServer Server;
        public const string BaseUrl = "http://localhost:8080";

        [SetUp]
        public void SetUp()
        {
            Server = new RegistrarServer(BaseUrl, ":memory:");
        }

        [TearDown]
        public void TearDown()
        {
            Server.Dispose();
        }

        [Test]
        public void TestRegistration()
        {
            var client = new RegistrarClient(BaseUrl);

            var info = new DeviceInfo
            {
                DeviceModel = "aDevice",
                IP = "0.0.0.0",
                MobileOs = MobileOs.None,
                OsVersion = "1.0",
                UniqueId = "0"
            };

            client.Register(info);
            var match = client.GetFirstMatch(info);
            Assert.IsTrue(info.IP == match.IP, "Actual: {0} Expected: {1}", match.IP, info.IP);
        }

    }
}