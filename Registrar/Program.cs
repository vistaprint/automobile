using VP.Capabilities.Quality.Platform.Mobile.Framework.Registrar;

namespace Registrar
{
    class Program
    {
        static void Main(string[] args)
        {
            MobileDb.Initialize("MobileDB.sqlite");
        }
    }
}
