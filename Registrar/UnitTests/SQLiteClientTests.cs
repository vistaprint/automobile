using Automobile.Mobile.Framework;
using Automobile.Mobile.Framework.Data;
using Automobile.Mobile.Framework.Device;
using NUnit.Framework;

namespace Automobile.Registrar.UnitTests
{
    [TestFixture]
    public class SQLiteClientTests
    {
        [Test]
        public void TestRegistration()
        {
            MobileDb.Instance = new SQLiteClient(":memory:", true);
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
            MobileDb.Instance.Dispose();
        }

        [Test]
        public void TestNoMatch()
        {
            MobileDb.Instance = new SQLiteClient(":memory:", true);
            // No match should return null
            Assert.IsNull(MobileDb.Instance.GetFirstMatch(new DeviceInfo { MobileOs = MobileOs.Android }));
            MobileDb.Instance.Dispose();
        }
    }
}
