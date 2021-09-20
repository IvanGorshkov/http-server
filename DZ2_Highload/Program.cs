
namespace DZ2_Highload
{
    class Program {
        static void Main(string[] args)
        {
            Server server = new Server(1234, 8);
            server.Start();
        }
    }
}
