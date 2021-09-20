using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace DZ2_Highload {
    public class Worker {
        private HTTPHeadersRequest HTTPRequest;
        
         
        // Конструктор класса. Ему нужно передавать принятого клиента от TcpListener
        public Worker(TcpListener Client, int id)
        {
            
            Console.WriteLine("Init Thread: {0}", id);
            while (true)
            {
                try
                {
                    Console.WriteLine("Try to catch: {0}", id);
                    Run(Client.AcceptTcpClient(), id);
                    Console.WriteLine("Lose: {0}", id);
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
        
        private void Run(TcpClient Client, int ID)
        {
            
            Console.WriteLine("Working Thread: {0}", ID);
            string Request = "";
            byte[] Buffer = new byte[1024];
            int Count;
            while ((Count = Client.GetStream().Read(Buffer,  0, Buffer.Length)) >  0)
            {
                Request += Encoding.ASCII.GetString(Buffer,  0, Count);
                if (Request.IndexOf("\r\n\r\n") >=  0 || Request.Length > 4096)
                {
                    break;
                }
            }

            String[] Splited = Request.Split(" ");
            try
            {
                this.HTTPRequest = new HTTPHeadersRequest(Splited[0], Splited[1]);
                    
            }
            catch (Exception e)
            {
                Console.WriteLine("Request {0}: {1}, {2}", ID, Request, e.ToString());
            
            }
            
         //   Console.WriteLine("Request: {0}", Request);
       //     Console.WriteLine("Request: {0}", Splited[1]);
           Console.WriteLine("HTTPRequest: {0}, {1}", HTTPRequest.Method, HTTPRequest.Path);

            if (HTTPRequest.Path == "/../")
            {
                SendHeaders(Client, Status.FORBIDDEN, "\n");
                Client.Close();
                return;
            }
            
            if (HTTPRequest.Path == "/")
            {
                HTTPRequest.Path = "/httptest/dir2/index.html";
            }

            if (this.HTTPRequest.Method != Method.GET && this.HTTPRequest.Method != Method.HEAD)
            {
                
                SendHeaders(Client, Status.METHOD_NOT_ALLOWED, "\n");
                Client.Close();
                return;
            }

            if (!File.Exists("../../.." + HTTPRequest.Path))
            {
                if(Directory.Exists("../../.." + HTTPRequest.Path))
                {
                    SendHeaders(Client, Status.FORBIDDEN, "\n");
                    Client.Close();
                    return;
                }
                SendHeaders(Client, Status.NOT_FOUND, "\n");
                Client.Close();
                return;
            } 
            
            FileInfo fi = new FileInfo("../../.." + HTTPRequest.Path);
            
            SendHeaders(Client, Status.OK, HTTPRequest.Method == Method.HEAD ? ContentType(fi.Extension) : ContentLength() + ContentType(fi.Extension));
            if (HTTPRequest.Method == Method.HEAD)
            {
                
                Client.Close();
                return;
            }
            Client.GetStream().BeginWrite(
                File.ReadAllBytes("../../.." + HTTPRequest.Path),
                0, 
                (int)fi.Length,
                new AsyncCallback(SendCallback),
                Client);
        }
       

            private static void SendCallback(IAsyncResult ar)
            {
            try
            {
            // Retrieve the socket from the state object.  
            TcpClient handler = (TcpClient) ar.AsyncState;

            handler.Close();

            }
            catch (Exception e)
            {
            Console.WriteLine(e.ToString());
            }
        }

        private String ContentLength()
        {
            FileInfo fi = new FileInfo("../../.." + HTTPRequest.Path);
            return $"Content-Length: {fi.Length}\n";
        }
        
        private String ContentType(String Path)
        {
            ContentType contentType = new ContentType(Path);
            
            return $"Content-Type: {contentType.GetContentType()}\n\n";
        }
        private void SendHeaders(TcpClient Client, int Status, String content)
        {
            DateTime x = DateTime.Now;
           
            string Str = $"HTTP/1.1 {Status} {DZ2_Highload.Status.GetTextStatus(Status)} \n" +
                         "Server: http-server\n" +
                         $"Date: {x.ToUniversalTime().ToString("r")}\n" +
                         $"Connection: keep-alive\n{content}";
            byte[] Buffer = Encoding.ASCII.GetBytes(Str);
            // Отправим его клиенту
            Client.GetStream().Write(Buffer,  0, Buffer.Length);
        }
    }
}