using Automobile.Mobile.Framework.Data;

namespace Automobile.Registrar.UnitTests
{
    public class TestRegistrationServer : RegistrarServer
    {
        private const string MEMORYDB = ":memory:";

        public TestRegistrationServer(string baseAddress) : base(baseAddress, MEMORYDB) {}

        public void NewDb()
        {
            MobileDb.Instance.Dispose();
            MobileDb.Instance = new SQLiteClient(MEMORYDB);
        }
    }
}