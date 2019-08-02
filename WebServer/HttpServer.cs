using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WebServer
{
   public class HttpServer
    {
        public const String Message_Directory = "/bin/Debug/netcoreapp2.2/root/Message/";
        public const String Web_Directory = "/bin/Debug/netcoreapp2.2/root/web";

        public const String Version = "HTTP/1.1";
        public const String Name = "C# Http Server";
        private bool _running = false;
        
        private static IPAddress _ipAddr = Dns.GetHostEntry("localhost").AddressList[1];
        // private TcpListener _tcpListener;
        private static IPEndPoint _iPEndPoint;
        private Socket _socket;
        public HttpServer(int port)
        {
            //_tcpListener = new TcpListener(IPAddress.Any, port);
            _iPEndPoint = new IPEndPoint(_ipAddr, port);
            _socket = new Socket(_ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        }

        public void Start()
        {
            Thread serverThread = new Thread(new ThreadStart(Run));
            serverThread.Start();
        }

        private void Run()
        {

            _running = true;
            //_tcpListener.Start();
            _socket.Bind(_iPEndPoint);
            _socket.Listen(10);
            while (_running)
            {
                Console.WriteLine("Waiting for connection");
                Socket tcpClient = _socket.Accept();
                Console.WriteLine("Client connected");
                HandleClient(tcpClient);

                tcpClient.Close();
            }
            _running = false;
            _socket.Disconnect(true);

        }

        private void HandleClient(Socket tcpClient)
        {
            NetworkStream networkStream = new NetworkStream(tcpClient);
            StreamReader streamReader = new StreamReader(networkStream);

            String message = "";
            while((streamReader.Peek() != -1))
            {
                message += streamReader.ReadLine() + "\n";
            }
            Debug.WriteLine("Request: \n" + message);

            Request request = Request.GetRequest(message);
            Response response = Response.From(request);
            response.Post(networkStream);
        }
    }
}
