using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThanksgivingGames.WebServer
{
    public class GameServer
    {
        private HttpListener _listener;
        private CancellationTokenSource _cts;
        public int Port { get; private set; }
        public string LocalAddress { get; private set; }

        public GameServer(int port = 5000)
        {
            Port = port;
        }

        public async Task StartAsync()
        {
            _cts = new CancellationTokenSource();

            LocalAddress = GetLocalIPAddress();
            if (LocalAddress == null)
                throw new Exception("Could not find local IP address.");

            string prefix = $"http://{LocalAddress}:{Port}/";
            _listener = new HttpListener();
            _listener.Prefixes.Add(prefix);
            _listener.Start();

            Console.WriteLine($"Server started at {prefix}");

            await Task.Run(() => ListenAsync(_cts.Token));
        }

        public void Stop()
        {
            _cts?.Cancel();
            _listener?.Stop();
        }

        private async Task ListenAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var context = await _listener.GetContextAsync();

                    string path = context.Request.Url.AbsolutePath.ToLower();

                    string responseHtml = "<html><body style='font-family:sans-serif;text-align:center;padding-top:50px;'>" +
                        "<h2>Welcome to Thanksgiving Games!</h2>" +
                        "<p>You're connected successfully.</p>" +
                        "</body></html>";

                    byte[] buffer = Encoding.UTF8.GetBytes(responseHtml);
                    context.Response.ContentType = "text/html";
                    context.Response.ContentLength64 = buffer.Length;
                    await context.Response.OutputStream.WriteAsync(buffer);
                    context.Response.Close();
                }
                catch (Exception ex)
                {
                    if (!token.IsCancellationRequested)
                        Console.WriteLine($"Server error: {ex.Message}");
                }
            }
        }

        private string GetLocalIPAddress()
        {
            try
            {
                using (var socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    // Connect to a remote endpoint — doesn’t actually send data
                    socket.Connect("8.8.8.8", 65530);
                    var ipEndPoint = socket.LocalEndPoint as IPEndPoint;
                    return ipEndPoint?.Address.ToString();
                }
            }
            catch
            {
                return "127.0.0.1";  // fallback to localhost
            }
        }
    }
}
