using System.IO;
using System;

namespace DZ2_Highload {
    public class ConfigFile {
        public int threadLimit { get; }
        public string documentsRoot { get; }
        public bool fileExists { get; }
        public int port { get; }

        public ConfigFile(string pathDocumentRoot)
        {
            if (!File.Exists(pathDocumentRoot))
            {
                fileExists = false;
                Console.WriteLine(pathDocumentRoot);
                return;
            }
            fileExists = true;
            var str = File.ReadAllLines(pathDocumentRoot);
            foreach (string s in str)
            {
                switch (s.Split(" ")[0])
                {
                    case "thread_limit": 
                        threadLimit = Convert.ToInt32(s.Split(" ")[1]);
                        break;
                    case "document_root":
                        documentsRoot = s.Split(" ")[1];
                        break;
                    case "port":
                        port = Convert.ToInt32(s.Split(" ")[1]);
                        break;
                    default: 
                        break;
                }
            }
        }
    }
}