using Microsoft.AspNetCore.SignalR;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPFSceenCast;

namespace WPFScreenCast
{
    public class StreamHub : Hub
    {
        public async Task Enviar(IEnumerable<PixelModificado> alteracoes)
        {
            if (Clients != null)
            {
                await Clients.All.SendAsync("Enviar", alteracoes);
            }
        }
    }
}
