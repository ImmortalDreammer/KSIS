using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                Console.Write("Enter addres: ");
                string str = Console.ReadLine();
                if (str.Equals("exit"))
                    return;
                Uri uri = new Uri(str);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(uri.Host, uri.Port);

                string reqStr = $"GET {uri.AbsolutePath} HTTP/1.1\r\n";
                string host = $"Host: {uri.Host}\r\n";
                string contentLength = "Content-Length: 0\r\n";
                string userAgent = "User-Agent: chrome\r\n";
                string acceptLanguage = "Accepted-Language: en/us\r\n";
                string fullRequestString = reqStr + userAgent + contentLength + host + acceptLanguage + "\r\n\r\n";
                byte[] req = Encoding.ASCII.GetBytes(fullRequestString);
                Console.WriteLine(fullRequestString);

                socket.Send(req);

                byte[] response = new byte[256];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = socket.Receive(response, response.Length, 0);
                    builder.Append(Encoding.ASCII.GetString(response, 0, bytes));
                } while (socket.Available > 0);

                Console.WriteLine(builder.ToString());
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            } while (true);
        }
    }
}