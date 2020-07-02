using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WPFSceenCast
{
    class Captura
    {
        private DirectBitmap UltimoQuadro { get; set; }
        private Timer tmrExecucao { get; set; }
        public event Action<List<PixelModificado>> houveCaptura;

        public Captura()
        {
            tmrExecucao = new Timer();
            tmrExecucao.Interval = 50;
            tmrExecucao.Tick += TmrExecucao_Tick;
            tmrExecucao.Enabled = false;
        }

        public void Iniciar()
        {
            tmrExecucao.Enabled = true;
        }

        public void Parar()
        {
            tmrExecucao.Enabled = false;
        }

        [DllImport("msvcrt.dll")]
        private static extern int memcmp(IntPtr b1, IntPtr b2, long count);

        public static bool CompareMemCmp(Bitmap b1, Bitmap b2)
        {
            if ((b1 == null) != (b2 == null)) return false;
            if (b1.Size != b2.Size) return false;

            var bd1 = b1.LockBits(new Rectangle(new Point(0, 0), b1.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bd2 = b2.LockBits(new Rectangle(new Point(0, 0), b2.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                IntPtr bd1scan0 = bd1.Scan0;
                IntPtr bd2scan0 = bd2.Scan0;

                int stride = bd1.Stride;
                int len = stride * b1.Height;

                return memcmp(bd1scan0, bd2scan0, len) == 0;
            }
            finally
            {
                b1.UnlockBits(bd1);
                b2.UnlockBits(bd2);
            }
        }

        private void TmrExecucao_Tick(object sender, EventArgs e)
        {
            DirectBitmap print = new DirectBitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            var grafico = Graphics.FromImage(print.Bitmap);
            var tamanho = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Size.Height);
            grafico.CopyFromScreen(0, 0, 0, 0, tamanho,CopyPixelOperation.SourceCopy);

            if (UltimoQuadro == null)
            {
                UltimoQuadro = print;
                return;
            }

            if (CompareMemCmp(print.Bitmap,UltimoQuadro.Bitmap))
            {
                print.Dispose();
                return;
            }

            var alteracoes = new BlockingCollection<PixelModificado>();

            Parallel.For(0, print.Width, x =>
            {
                Parallel.For(0, print.Height, y =>
                {
                    var cor = print.GetPixel(x, y);
                    var ultimacor = UltimoQuadro.GetPixel(x, y);
                    if (cor != ultimacor)
                    {
                        var pixel = new PixelModificado { corAlpha = cor.A, corRed = cor.R, corGreen = cor.G, corBlue = cor.B, X = x, Y = y };
                        alteracoes.Add(pixel);
                    }
                });
            });

            houveCaptura(alteracoes.ToList());

            UltimoQuadro.Dispose();
            UltimoQuadro = print;
        }
    }
}
