using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DZ2_Highload
{
    class Client {
        
        
        // Конструктор класса. Ему нужно передавать принятого клиента от TcpListener
        public Client(TcpListener Client, int id)
        {
            
            Console.WriteLine("Init Thread: {0}", id);
            while (true)
            {
                try
                {
                    Console.WriteLine("Try to catch: {0}", id);
                    new Worker(Client.AcceptTcpClient(), id);
                    Console.WriteLine("Lose: {0}", id);
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }

    class Server {
        private int Port;
        private int threadCount;
        private static int ID;
        public Server(int Port, int threadCount) {
            this.Port = Port;
            this.threadCount = threadCount;
        }
        
        public void start() {
            try {
                TcpListener serverSocket = new TcpListener(IPAddress.Any, Port);
                serverSocket.Start();
                List<Thread> threads = CreateThreads(serverSocket);
                threads.ForEach(item =>
                    {
                        item.Start(serverSocket);
                    } );
                
                for (int i = 0; i != threads.Count; ++i) {
                    threads[i].Join();
                }
            } catch (Exception aException) {
            }
        }

        static void ClientThread(Object StateInfo)
        {
            ID++;
            new Client((TcpListener)StateInfo, ID);
        }
        private List<Thread> CreateThreads(TcpListener serverSocket) {
            List<Thread> result = new List<Thread>();
            for (int i = 0; i != threadCount; ++i)
            { 
                result.Add(new Thread(new ParameterizedThreadStart(ClientThread)));
            }
            return result;
        }
    }
    
    /*
    class Server {
        TcpListener Listener; // Объект, принимающий TCP-клиентов
        private int Port;
        // Запуск сервера
        
        static void ClientThread(Object StateInfo)
        {
            new Client((TcpClient)StateInfo);
        }
        
        public Server(int Port)
        {
            this.Port = Port;
        }

        public void StartServer()
        {
            // Создаем "слушателя" для указанного порта
            Listener = new TcpListener(IPAddress.Any, Port);
            Listener.Start(); // Запускаем его
            
            // В бесконечном цикле
            while (true)
            {
              //  new Client(Listener.AcceptTcpClient());
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), Listener.AcceptTcpClient());
            }
        }
        // Остановка сервера
        ~Server()
        {
            // Если "слушатель" был создан
            if (Listener != null)
            {
                // Остановим его
                Listener.Stop();
            }
        }
    }
    */
    /*
    class Program {
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8080);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();
                
                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                    DateTime dt = DateTime.Now;

                    //getting Milliseconds only from the currenttime
                    int ms = dt.Millisecond;

                    //printing the current date & time
                    Console.WriteLine("The current time is: " + dt.ToString());

                    //printing the Milliseconds value of the current time
                    Console.WriteLine("Milliseconds of current time: " + ms.ToString());

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            string Html = "<html><body><h1>It works!</h1></body></html>";
            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            string Str = "HTTP/1.1 200 OK\nContent-type: text/html\nContent-Length:" + Html.Length.ToString() + "\n\n" +
                         Html;
            // Приведем строку к виду массива байт
            byte[] Buffer = Encoding.ASCII.GetBytes(Str);

            handler.BeginSend(Buffer, 0, Buffer.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket) ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
    */
/*
    class Server
    {
        private TcpListener Listener { get; set; }

        public Server() : this(8080) {}

        public Server(int port)
        {
            Listener = new TcpListener(IPAddress.Any, port);
            Listener.Start();

            for (;;)
            {
                var client = Listener.AcceptTcpClient();

                var thread = new Thread(new ParameterizedThreadStart(HandleClient));
                thread.Start(client);
            }
        }

        private static void HandleClient(object tcpClient)
        {
            new Client((TcpClient)tcpClient);
        }

        ~Server()
        {
            if (Listener != null)
            {
                Listener.Stop();
            }
        }
    }
*/
    class Program {
        static void Main(string[] args)
        {
            Server server = new Server(1234, 32);
            server.start();
        }
    }
    /*

    class Program
    {
        static void Main(string[] args)
        {
            // Определим нужное максимальное количество потоков
            // Пусть будет по 4 на каждый процессор
            int MaxThreadsCount = Environment.ProcessorCount * 4;
            // Установим максимальное количество рабочих потоков
            ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
            // Установим минимальное количество рабочих потоков
            ThreadPool.SetMinThreads(2, 2);
            
           Server HLServer = new Server(8080);
           HLServer.StartServer();
        }
    }*/
}
