using System;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace DZ2_Highload {
    public class Worker {
        private HTTPHeadersRequest _headersRequest;
        private string _root;
        
        public Worker(TcpListener listener, int id, string root)
        {
            _root = root; 
            Console.WriteLine("Init Thread: {0}", id);
            while (true)
            {
                try
                {
                    Console.WriteLine("Try to catch: {0}", id);
                    Run(listener.AcceptTcpClient(), id);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
        
        private void Run(TcpClient сlient, int ID)
        {
            Console.WriteLine("Working Thread: {0}", ID);
            var Request = "";
            var Buffer = new byte[1024];
            int Count;
            while ((Count = сlient.GetStream().Read(Buffer,  0, Buffer.Length)) >  0)
            {
                Request += Encoding.ASCII.GetString(Buffer,  0, Count);
                if (Request.IndexOf("\r\n\r\n") >=  0 || Request.Length > 4096)
                {
                    break;
                }
            }

            var Splited = Request.Split(" ");
            try
            {
                _headersRequest = new HTTPHeadersRequest(Splited[0], Splited[1], Splited[2]);
                    
            } catch (Exception e)
            {
                Console.WriteLine("Request {0}: {1}, {2}", ID, Request, e.ToString());
            
            }
            Console.WriteLine("HTTPRequest: {0}, {1}", _headersRequest.Method, _headersRequest.Path);

            if (_headersRequest.Path.Contains("/../"))
            {
                SendHeaders(сlient, Status.FORBIDDEN, "\n");
                сlient.Close();
                return;
            }
            
            if (_headersRequest.Path == "/" || _headersRequest.Path == "/httptest/dir2/")
            {
                _headersRequest.Path = "/httptest/dir2/index.html";
            }

            if (_headersRequest.Method != Method.GET && this._headersRequest.Method != Method.HEAD)
            {
                
                SendHeaders(сlient, Status.METHOD_NOT_ALLOWED, "\n");
                сlient.Close();
                return;
            }

            if (!File.Exists(_root + _headersRequest.Path))
            {
                if (Directory.Exists(_root + _headersRequest.Path))
                {
                    SendHeaders(сlient, Status.FORBIDDEN, "\n");
                    сlient.Close();
                    return;
                }
                SendHeaders(сlient, Status.NOT_FOUND, "\n");
                сlient.Close();
                return;
            } 
            
            FileInfo fi = new FileInfo(_root + _headersRequest.Path);
            
            SendHeaders(сlient, Status.OK, _headersRequest.Method == Method.HEAD ?  ContentLength(fi) + "\r\n" : ContentLength(fi) + ContentType(fi.Extension) + "\r\n");
            if (_headersRequest.Method == Method.HEAD)
            {
                сlient.Close();
                return;
            }

            using var fs = new FileStream(_root + _headersRequest.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            
            int count;
            while (fs.Position < fs.Length)
            {
                count = fs.Read(Buffer, 0, Buffer.Length);
                сlient.GetStream().Write(Buffer, 0, count);
            }
            
            
            сlient.Close();
        }

        private String ContentLength(FileInfo fi)
        {
            return $"Content-Length: {fi.Length}\r\n";
        }
        
        private String ContentType(String Extension)
        {
            var contentType = new ContentType(Extension);
            
            return $"Content-Type: {contentType.GetContentType()}\r\n";
        }
        private void SendHeaders(TcpClient Client, int Status, String content)
        {
            var x = DateTime.Now;
           
            var Str = $"{this._headersRequest.Protocol} {Status} {DZ2_Highload.Status.GetTextStatus(Status)}\r\n" +
                         "Server: http-server\r\n" +
                         $"Date: {x.ToUniversalTime().ToString("r")}\r\n" +
                         "Connection: keep-alive\r\n" +
                         $"{content}";
            var Buffer = Encoding.ASCII.GetBytes(Str);
            Console.WriteLine(Str);
            Client.GetStream().Write(Buffer,  0, Buffer.Length);
        }
    }
}