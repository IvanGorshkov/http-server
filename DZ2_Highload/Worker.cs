using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;

namespace DZ2_Highload {
    public class Worker {
        private HTTPHeadersRequest _headersRequest;
        private string _root;
        private NetworkStream _networkStream;
        private byte[] _buffer = new byte[1024];
    //    private static Mutex mut = new Mutex();
        
        public Worker(TcpListener listener, int id, string root)
        {
            _root = root; 
            Console.WriteLine("Init Thread: {0}", id);
            while (true)
            {
                try
                { 
                    Console.WriteLine("Try to catch: {0}", id);
           //         mut.WaitOne();
                   Run(listener.AcceptTcpClient(), id);
                //    listener.BeginAcceptTcpClient(this.OnAcceptConnection,  listener);
                }
                catch (Exception e)
                {
                     Console.WriteLine(e.ToString());
                }
            }
        }
        
        
        private void Run(TcpClient сlient, int ID)
        {
      //      mut.ReleaseMutex();
            Console.WriteLine("Working Thread: {0}", ID);
            _networkStream = сlient.GetStream();
            var request = "";
            int count;
            while ((count = _networkStream.Read(_buffer,  0, _buffer.Length)) >  0)
            {
                request += Encoding.ASCII.GetString(_buffer,  0, count);
                if (request.IndexOf("\r\n\r\n") >=  0 || request.Length > 2048)
                    break;
            }

            var splited = request.Split(" ");
            try
            {
                _headersRequest = new HTTPHeadersRequest(splited[0], splited[1], splited[2]);
                    
            } catch (Exception e)
            {
                // Console.WriteLine("Request {0}: {1}, {2}", ID, Request, e.ToString());
            
            }
            // Console.WriteLine("HTTPRequest: {0}, {1}", _headersRequest.Method, _headersRequest.Path);
            
            if (_headersRequest.Path.Contains("/../"))
            {
                SendHeaders(Status.FORBIDDEN, "\n");
                сlient.Close();
                return;
            }
            
            if (_headersRequest.Path == "/" || _headersRequest.Path == "/httptest/dir2/")
            {
                _headersRequest.Path = "/httptest/dir2/index.html";
            }


            if (_headersRequest.Method != Method.GET && this._headersRequest.Method != Method.HEAD)
            {
                
                SendHeaders(Status.METHOD_NOT_ALLOWED, "\n");
                сlient.Close();
                return;
            }

            if (!File.Exists(_root + _headersRequest.Path))
            {
                if (Directory.Exists(_root + _headersRequest.Path))
                {
                    SendHeaders(Status.FORBIDDEN, "\n");
                    сlient.Close();
                    return;
                }
                SendHeaders(Status.NOT_FOUND, "\n");
                сlient.Close();
                return;
            } 
            
            FileInfo fi = new FileInfo(_root + _headersRequest.Path);
            
            SendHeaders(Status.OK, _headersRequest.Method == Method.HEAD ?  ContentLength(fi) + "\r\n" : ContentLength(fi) + ContentType(fi.Extension) + "\r\n");
            if (_headersRequest.Method == Method.HEAD)
            {
                сlient.Close();
                return;
            }
            SendFile();
            сlient.Close();
        }

        private void SendFile()
        {
            using var fs = new FileStream(_root + _headersRequest.Path, FileMode.Open, FileAccess.Read);
            
            byte[] buff = new byte[2048];
            int read = -1;
            while (read != 0) {
                read = fs.Read(buff, 0, buff.Length);
                if (read != 0) {
                    _networkStream.Write(buff, 0, read);
                }
            }
            
            _networkStream.Close();
            fs.Close();
        }
        
        private String ContentLength(FileInfo fi)
        {
            return $"Content-Length: {fi.Length}\r\n";
        }
        
        private String ContentType(String Extension)
        {
            return $"Content-Type: {DZ2_Highload.ContentType.GetContentType(Extension)}\r\n";
        }
        
        private void SendHeaders(int Status, String content)
        {
            var Str = $"{_headersRequest.Protocol} {Status} {DZ2_Highload.Status.GetTextStatus(Status)}\r\n" +
                         "Server: http-server\r\n" +
                         $"Date: {DateTime.Now.ToUniversalTime().ToString("r")}\r\n" +
                         "Connection: keep-alive\r\n" +
                         $"{content}";
            var Buffer = Encoding.ASCII.GetBytes(Str);
            if (_networkStream.CanWrite)
            {
                _networkStream.Write(Buffer,  0, Buffer.Length);
            }

        }
    }
}