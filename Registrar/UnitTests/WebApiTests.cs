using Automobile.Mobile.Framework;
using Automobile.Mobile.Framework.Data;
using NUnit.Framework;

namespace Automobile.Registrar.UnitTests
{
    [TestFixture]
    public class WebApiTests
    {
        public TestRegistrationServer Server;
        public const string BaseUrl = "http://localhost:8080";

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            Server = new TestRegistrationServer(BaseUrl);
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            Server.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            Server.NewDb();
        }

        [Test]
        public void TestNoMatch()
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

            Assert.IsNull(client.GetFirstMatch(info));
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

        [Test]
        public void TestAvailibility()
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

            client.SetAvailibility(info, false);
            // Unavailible devices shouldn't be found in GetFirstMatch
            Assert.IsNull(client.GetFirstMatch(info));

            client.SetAvailibility(info, true);
            // Availible devices should be found
            var match = client.GetFirstMatch(info);
            Assert.IsTrue(info.IP == match.IP, "Actual: {0} Expected: {1}", match.IP, info.IP);
        }

    }
}