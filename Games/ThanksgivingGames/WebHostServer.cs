using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace ThanksgivingGames
{
    public class WebHostServer
    {
        private IHost _host;
        public int Port { get; }
        public string LocalAddress { get; private set; }

        public WebHostServer(int port = 5000)
        {
            Port = port;
        }

        public async Task StartAsync()
        {
            LocalAddress = GetLocalIPAddress();

            _host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel()
                              .UseUrls($"http://0.0.0.0:{Port}")
                              .ConfigureServices(services => { })
                              .Configure(app =>
                              {
                                  var www = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

                                  app.UseRouting();

                                  app.UseEndpoints(endpoints =>
                                  {
                                      endpoints.MapGet("/", () => "Thanksgiving Game Server is running!");
                                      endpoints.MapGet("/join", () => "Player Join Page Coming Soon...");
                                  });
                              });
                })
                .Build();

            await _host.StartAsync();
        }

        public async Task StopAsync()
        {
            if (_host != null)
                await _host.StopAsync();
        }

        private static string GetLocalIPAddress()
        {
            foreach (var ni in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ni.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return ni.ToString();
            }

            return "127.0.0.1";
        }
    }
}
