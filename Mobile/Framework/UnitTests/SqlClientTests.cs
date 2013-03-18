using Automobile.Mobile.Framework.Data;
using NUnit.Framework;

namespace Automobile.Mobile.Framework.UnitTests
{
    [TestFixture]
    public class SqlClientTests
    {
        [SetUp]
        public void Setup()
        {
            MobileDb.CreateSqlClient(@"Server=.\SQLExpress;Database=Automobile;Trusted_Connection=True;");
        }

        [TearDown]
        public void Teardown()
        {
            MobileDb.Instance.Dispose();
        }

        [Test]
        public void TestRegistration()
        {
            DeviceInfo info = new DeviceInfo
            {
                DeviceModel = "aDevice",
                IP = "0.0.0.0",
                MobileOs = MobileOs.None,
                OsVersion = "1.0",
                UniqueId = "0"
            };
            MobileDb.Instance.Register(info);
            var match = MobileDb.Instance.GetFirstMatch(info);
            Assert.IsTrue(info.IP == match.IP, "Actual: {0} Expected: {1}", match.IP, info.IP);
        }

        [Test]
        public void TestAvailibility()
        {
            DeviceInfo info = new DeviceInfo
            {
                DeviceModel = "aDevice",
                IP = "0.0.0.0",
                MobileOs = MobileOs.None,
                OsVersion = "1.0",
                UniqueId = "0"
            };
            MobileDb.Instance.Register(info);

            MobileDb.Instance.SetAvailibility(info, false);
            // Unavailible devices shouldn't be found in GetFirstMatch
            Assert.IsNull(MobileDb.Instance.GetFirstMatch(info));

            MobileDb.Instance.SetAvailibility(info, true);
            var match = MobileDb.Instance.GetFirstMatch(info);
            // Availible devices should be found
            Assert.IsTrue(info.IP == match.IP, "Actual: {0} Expected: {1}", match.IP, info.IP);
        }

        [Test]
        public void TestNoMatch()
        {
            // No match should return null
            Assert.IsNull(MobileDb.Instance.GetFirstMatch(new DeviceInfo { UniqueId = "GarbageIdShouldNeverExist"}));
        }
    }
}