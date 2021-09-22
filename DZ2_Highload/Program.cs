
using System;

namespace DZ2_Highload
{
    class Program {
        static void Main(string[] args)
        {;
            ConfigFile configFile; 
            if (args.Length == 1)
            {
                configFile  = new ConfigFile(args[0]);
            }
            else
            {
                return;
            }
            
            if (!configFile.fileExists)
            {
                return;
            }
            
            Server server = new Server(configFile);
            try
            {
                server.Start();
            }
            catch (Exception)
            {
                // Console.WriteLine("Port is used");
            }
        }
    }
}
