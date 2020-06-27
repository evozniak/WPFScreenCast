using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFScreenCast;

namespace WPFSceenCast
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Captura captura;
        Servidor servidor;
        Cliente cliente;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnIniciar_Click(object sender, RoutedEventArgs e)
        {
            //captura = new Captura();
            //captura.Iniciar();
            servidor = new Servidor();
            await servidor.Iniciar();
        }

        private async void btnConectar_Click(object sender, RoutedEventArgs e)
        {
            cliente = new Cliente(imgTransmitida);
            await cliente.Conectar();
        }
    }
}
