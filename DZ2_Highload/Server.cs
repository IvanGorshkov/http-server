using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DZ2_Highload {
    class Server {
        private int Port;
        private int threadCount;
        private static int ID;
        public Server(int Port, int threadCount) {
            this.Port = Port;
            this.threadCount = threadCount;
        }
        
        public void Start() {
            TcpListener serverSocket = new TcpListener(IPAddress.Any, Port); 
            serverSocket.Start();
            List<Thread> threads = CreateThreads();
            threads.ForEach(item =>  item.Start(serverSocket));
            threads.ForEach(item =>  item.Join());
        }

        private static void ClientThread(Object StateInfo)
        {
            ID++;
            new Worker((TcpListener)StateInfo, ID);
        }
        private List<Thread> CreateThreads() {
            List<Thread> result = new List<Thread>();
            for (int i = 0; i != threadCount; ++i)
            { 
                result.Add(new Thread(new ParameterizedThreadStart(ClientThread)));
            }
            return result;
        }
    }
}