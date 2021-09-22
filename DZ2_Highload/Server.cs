using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DZ2_Highload {
    class Server {
        private int _port { get; }
        private int _threadCount { get; }
        private static int _id;
        private static string _root;
        public Server(ConfigFile configFile) {
            _port = configFile.port;
            _threadCount = configFile.threadLimit;
            _root = configFile.documentsRoot;
        }

        public void Start() {
            var listener = new TcpListener(IPAddress.Any, _port);
            try
            {
                listener.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            var threads = CreateThreads();
            threads.ForEach(item =>  item.Start(listener));
            threads.ForEach(item =>  item.Join());
        }

        private static void WorkerThread(Object listener)
        {
            _id++;
            new Worker((TcpListener)listener, _id, _root);
        }
        private List<Thread> CreateThreads() {
            var result = new List<Thread>();
            for (var i = 0; i != _threadCount; ++i)
            { 
                result.Add(new Thread(new ParameterizedThreadStart(WorkerThread)));
            }
            return result;
        }
    }
}