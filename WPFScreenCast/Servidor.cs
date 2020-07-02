using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPFSceenCast;
using Microsoft.AspNetCore.SignalR;

namespace WPFScreenCast
{
    public class Servidor
    {
        public IWebHost SrvSignalR { get; }
        private Captura PCaptura { get; }
        private CancellationToken TokenCancelamento { get; set; }
        private IHubContext<StreamHub> streamHub;
        public Servidor()
        {
            SrvSignalR = CriarHostAsp().Build();
            streamHub = SrvSignalR.Services.GetService(typeof(IHubContext<StreamHub>)) as IHubContext<StreamHub>;
            PCaptura = new Captura();
            PCaptura.houveCaptura += PCaptura_houveCaptura;
        }

        private async void PCaptura_houveCaptura(List<PixelModificado> obj)
        {
            await streamHub.Clients.All.SendAsync("Enviar", obj);
        }

        public async Task Iniciar()
        {
            TokenCancelamento = new CancellationToken();
            await SrvSignalR.StartAsync(TokenCancelamento);
            PCaptura.Iniciar();
        }

        public async Task Parar()
        {
            PCaptura.Parar();
            await SrvSignalR.StopAsync(TokenCancelamento);
        }

        private static IWebHostBuilder CriarHostAsp() =>
            WebHost.CreateDefaultBuilder()
            .UseStartup<Startup>().UseUrls("http://0.0.0.0:5000");
    }


}
