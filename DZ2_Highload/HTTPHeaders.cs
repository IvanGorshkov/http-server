using System;
using System.Diagnostics;

namespace DZ2_Highload {
    public struct HTTPHeadersRequest {
        public Method Method { get; }
        public String Path { get; }

        internal HTTPHeadersRequest(String Method, String Path)
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
                    this.Method = DZ2_Highload.Method.HEAD;
                    break;
                }
            }

            this.Path = Path;
        }
    }

    public enum Method {
        GET,
        HEAD,
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