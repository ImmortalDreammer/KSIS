using System;
using System.IO;
using System.Linq;
using System.Net;
using Flurl.Util;

namespace lab_8_1_server
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = 3000;
            var rootDir = "wwwroot";
            var server = new HttpLabServer(port,rootDir);
            server.Initialize();
            server.Start();
        }
    }

    public class HttpLabServer
    {
        private readonly string _rootDirectory;
        private readonly int _port;
        private HttpListener _httpListener;

        public HttpLabServer(int port, string rootDirectory)
        {
            _rootDirectory = rootDirectory;
            _port = port;
        }

        public void Initialize()
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://localhost:{_port}/");
            _httpListener.Start();
        }

        private string GetFilePath(Uri uri)
        {
            var path = uri.LocalPath.Replace("/", "\\").Trim('\\');
            return Path.Combine(Directory.GetCurrentDirectory(), _rootDirectory, path);
        }

        public void Start()
        {
            while (true)
            {
                var context = _httpListener.GetContext();
                var filePath = GetFilePath(context.Request.Url);

                if (!File.Exists(filePath))
                {
                    context.Response.StatusCode = 404;
                    LogContext(context);
                    context.Response.Close();
                    continue;
                }

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var fileReader = new StreamReader(fileStream))
                using (var outputStreamWriter = new StreamWriter(context.Response.OutputStream))
                {
                    outputStreamWriter.Write(fileReader.ReadToEnd());
                }

                context.Response.StatusCode = 200;
                LogContext(context);
                context.Response.Close();
            }
        }

        public void LogContext(HttpListenerContext context)
        {
            Console.WriteLine($"<--- {context.Request.HttpMethod} {context.Request.Url.LocalPath}");
            context.Request.Headers.AllKeys.ToList().ForEach(key =>
            {
                Console.WriteLine($"\t{key,15}: {context.Request.Headers[key],15}");
            });
            Console.WriteLine($"---> {context.Request.HttpMethod} {context.Request.Url.LocalPath} {context.Response.StatusCode}");
            context.Response.Headers.AllKeys.ToList().ForEach(key =>
            {
                Console.WriteLine($"\t{key,15}: {context.Request.Headers[key],15}");
            });
        }
    }
}