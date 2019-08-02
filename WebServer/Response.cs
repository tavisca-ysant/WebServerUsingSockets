using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace WebServer
{
    public class Response
    {
        private Byte[] _data = null;
        private String _status;
        private String _mime;
        private Response(String status, String mime, Byte[] data)
        {
            _status = status;
            _data = data;
            _mime = mime;
        }

        public static Response From(Request request)
        {
            if (request == null)
                return MakeNullRequest();
            if (request.Type == "GET")
            {
                String file = Environment.CurrentDirectory + HttpServer.Web_Directory + request.URL; 
                FileInfo fileInfo = new FileInfo(file);
               // Console.WriteLine("File Info "+fileInfo.FullName);
                //Console.WriteLine("File: " + file);
                if (fileInfo.Exists && fileInfo.Extension.Contains("."))
                {
                    return MakeFromFile(fileInfo);
                }
                else
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(fileInfo + "/");
                    if (!directoryInfo.Exists)
                        return MakePageNotFound();
                    FileInfo[] files = directoryInfo.GetFiles();
                    foreach(var _file in files)
                    {
                        String Name = _file.Name;
                        
                        if (Name.Contains("default.html") || Name.Contains("default.htm")
                           || Name.Contains("index.html") || Name.Contains("index.htm"))
                            return MakeFromFile(_file);
                    }
                }

            }
            else
                return MakeMethodNotAllowed();
            return MakePageNotFound();
        }

        private static Response MakeFromFile(FileInfo fileInfo)
        {
            FileStream fileStream = fileInfo.OpenRead();
            BinaryReader binaryReader = new BinaryReader(fileStream);
            Byte[] data = new Byte[fileStream.Length];
            binaryReader.Read(data, 0, data.Length);
            String Data = Encoding.UTF8.GetString(data, 0, data.Length);
            Debug.WriteLine("Response: "+Data);
            fileStream.Close();
            String mime = MIMEAssistant.GetMIMEType(fileInfo.Name);
            return new Response("200: OK", mime, data);
        }
            
        private static Response MakeNullRequest()
        {

            String file = Environment.CurrentDirectory + HttpServer.Message_Directory + "400.html";
            FileInfo fileInfo = new FileInfo(file);
            FileStream fileStream = fileInfo.OpenRead();
            BinaryReader binaryReader = new BinaryReader(fileStream);
            Byte[] data = new Byte[fileStream.Length];
            binaryReader.Read(data, 0, data.Length);
            fileStream.Close();
            return new Response("400: Bad Request", "text/html",data);
        }

        private static Response MakeMethodNotAllowed()
        {

            String file = Environment.CurrentDirectory + HttpServer.Message_Directory + "405.html";
            FileInfo fileInfo = new FileInfo(file);
            FileStream fileStream = fileInfo.OpenRead();
            BinaryReader binaryReader = new BinaryReader(fileStream);
            Byte[] data = new Byte[fileStream.Length];
            binaryReader.Read(data, 0, data.Length);
            fileStream.Close();
            return new Response("405: Method Not allowed", "text/html", data);
        }

        private static Response MakePageNotFound()
        {

            String file = Environment.CurrentDirectory + HttpServer.Message_Directory + "404.html";
            FileInfo fileInfo = new FileInfo(file);
            FileStream fileStream = fileInfo.OpenRead();
            BinaryReader binaryReader = new BinaryReader(fileStream);
            Byte[] data = new Byte[fileStream.Length];
            binaryReader.Read(data, 0, data.Length);
            fileStream.Close();
            return new Response("404: Page Not Found", "text/html", data);
        }

        public void Post(NetworkStream networkStream)
        {

            StreamWriter streamWriter = new StreamWriter(networkStream);
            streamWriter.WriteLine(String.Format("{0} {1}\r\nServer: {2}\r\nContent-Type: {3}\r\nAccept-Ranges: bytes\r\nContent-Length: {4}\r\n"
                                                 , HttpServer.Version, _status, HttpServer.Name, _mime, _data.Length));
            streamWriter.Flush();
            networkStream.Write(_data, 0, _data.Length);
        }
    }
}
