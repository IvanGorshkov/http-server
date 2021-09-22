using System;


namespace DZ2_Highload {
    public class HTTPHeadersRequest {
        public Method Method { get; }
        public String Path { get; set; }
        public String Protocol { get; }

        internal HTTPHeadersRequest(String Method, String Path, String Protocol)
        {
            switch (Method)
            {
                case "GET":
                {
                    this.Method = DZ2_Highload.Method.GET;
                    break;
                }
                case "HEAD":
                {
                    this.Method = DZ2_Highload.Method.HEAD;
                    break;
                }

                default:
                {
                    this.Method = DZ2_Highload.Method.UNKOWN;
                    break;
                }
            }

            this.Protocol = Protocol[..8];
            // Console.WriteLine(this.Protocol);
            this.Path = Uri.UnescapeDataString(Path);
            int index = this.Path.IndexOf("?");
            if (index >= 0)
                this.Path = this.Path.Substring(0, index);
        }
    }

    public enum Method {
        GET,
        HEAD,
        UNKOWN,
    }

    public class ContentType {
        private String extension;

        public ContentType(String extension)
        {
            this.extension = extension;
        }
        public string GetContentType()
        {
            switch (extension) {
                case ".htm":
                case ".html":
                    return "text/html";
                case ".css":
                    return "text/css";
                case ".js":
                    return "application/javascript";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".swf":
                    return "application/x-shockwave-flash";
                default:
                    if (extension.Length > 1)
                    {
                        return "application/" + extension.Substring(1);
                    }

                    return "application/unknown";
            }
        }
        
    }
    public class Status {
        public static int OK
        {
            get => 200;
        }

        public static int NOT_FOUND
        {
            get => 404;
        }
        public static int FORBIDDEN
        {
            get => 403;
        }
        public static int METHOD_NOT_ALLOWED
        {
            get => 405;
        }

        public static String GetTextStatus(int Status)
        {
             switch (Status)
                {
                    case 200:
                    {
                        return "OK";
                    }
                    case 404:
                    {
                        return "Not Found";
                    }
                    case 403:
                    {
                        return "Forbidden";
                    }
                    default:
                    {
                        return "Method Not Allowed";
                    }
                }
        }
    }
}