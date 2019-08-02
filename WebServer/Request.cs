using System;
using System.Collections.Generic;
using System.Text;

namespace WebServer
{
    public class Request
    {
        public String Type { get; set;  }

        public String URL { get; set; }

        public String Host { get; set; }

        public String Referer { get; set; }

        private Request(String type, String url, String host, String referer)
        {
            Type = type;
            URL = url;
            Host = host;
            Referer = referer;
        }
        public static Request GetRequest(String request)
        {
            if (String.IsNullOrEmpty(request))
                return null;
            String[] tokens = request.Split(' ', '\n');
            String type = tokens[0];
            String url = tokens[1];
            String host = tokens[4];
            String referer = "";
            for(int index = 0; index < tokens.Length; index++)
            {
                if (tokens[index] == "Referer:")
                {
                    referer = tokens[index + 1];
                    break;
                }      
            }
            Console.WriteLine(String.Format("{0} {1} @ {2} \nReferer: {3}", type, url, host, referer));

            return new Request(type, url, host, referer);
        }

    }
}
