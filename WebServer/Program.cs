using System;

namespace WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server on port 8080");
            HttpServer httpServer = new HttpServer(8080);
            httpServer.Start();
        }
    }
}
