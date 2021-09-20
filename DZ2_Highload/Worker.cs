using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace DZ2_Highload {
    public class Worker {
        private HTTPHeadersRequest HTTPRequest;
        public Worker(TcpClient Client, int ID)
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
            this.HTTPRequest = new HTTPHeadersRequest(Splited[0], Splited[1]);
            Console.WriteLine("Request: {0}", Splited[1]);
            Console.WriteLine("HTTPRequest: {0}, {1}", HTTPRequest.Method, HTTPRequest.Path);

            if (HTTPRequest.Path == "/../")
            {
                SendHeaders(Client, Status.FORBIDDEN);
            }
            SendHeaders(Client, Status.OK);
            Client.Close();
        }

        private void SendHeaders(TcpClient Client, int Status)
        {
            DateTime x = DateTime.Now;
           
            string Str = $"HTTP/1.1 {Status} {DZ2_Highload.Status.GetTextStatus(Status)} \n" +
                         $"Server: http-server\n" +
                         $"Date: {x.ToUniversalTime().ToString("r")}\n" +
                         "Connection: keep-alive\n\n";
            byte[] Buffer = Encoding.ASCII.GetBytes(Str);
            // Отправим его клиенту
            Client.GetStream().Write(Buffer,  0, Buffer.Length);
        }
    }
}