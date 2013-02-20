namespace Automobile.Registrar
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new RegistrarServer("http://localhost:8080", "MobileDB.sqlite");
        }
    }
}
