using Automobile.Mobile.Framework;
using Automobile.Mobile.Framework.Device;
using NUnit.Framework;

namespace Automobile.Registrar.UnitTests
{
    [TestFixture]
    public class MobileDbTests
    {
        [Test]
        public void TestRegistration()
        {
            MobileDb.Initialize(":memory:", true);
            DeviceInfo info = new DeviceInfo
                              {
                                  DeviceModel = "aDevice",
                                  IP = "0.0.0.0",
                                  MobileOs = MobileOs.None,
                                  OsVersion = "1.0",
                                  UniqueId = "0"
                              };
            MobileDb.Submit(info);
            var match = MobileDb.GetFirstMatch(info);
            Assert.IsTrue(info.IP == match.ToString(), "Actual: {0} Expected: {1}", match, info.IP);
            MobileDb.Close();
        }

        [Test]
        public void TestNoMatch()
        {
            MobileDb.Initialize(":memory:", true);
            // No match should return null
            Assert.IsNull(MobileDb.GetFirstMatch(new DeviceInfo { MobileOs = MobileOs.Android }));
            MobileDb.Close();
        }
    }
}
