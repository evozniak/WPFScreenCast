using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFSceenCast;

namespace WPFScreenCast
{
    public class Cliente
    {
        private HubConnection Conexao { get; }
        private CancellationToken tokenCancelamento;
        System.Windows.Controls.Image imgCanvas;
        DirectBitmap dbm;
        public Cliente(System.Windows.Controls.Image imgCanvas, string IpDestino)
        {
            Conexao = new HubConnectionBuilder()
                .WithUrl($"http://{IpDestino}:5000/streamHub")
                .Build();

            Conexao.On("Enviar", (List<PixelModificado> alteracoes) =>
            {
                ReconstruirBitMap(alteracoes);
            });
            this.imgCanvas = imgCanvas;
        }
        public void ReconstruirBitMap(List<PixelModificado> dados)
        {
            if (dbm == null)
                dbm = new DirectBitmap(1920, 1080);


            Parallel.ForEach(dados, pixel =>
            {
                var cor = System.Drawing.Color.FromArgb(pixel.corAlpha, pixel.corRed, pixel.corGreen, pixel.corBlue);
                dbm.SetPixel(pixel.X, pixel.Y, cor);
            });

            using (MemoryStream memory = new MemoryStream())
            {
                dbm.Bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                imgCanvas.Source = bitmapImage as ImageSource;
                dbm.Bitmap.Save(@"C:\temp\captura.bmp");
            }

        }

        public async Task Conectar()
        {
            tokenCancelamento = new CancellationToken();
            await Conexao.StartAsync(tokenCancelamento);
        }

        public async Task Desconectar()
        {
            await Conexao.StopAsync(tokenCancelamento);
        }
    }
}
