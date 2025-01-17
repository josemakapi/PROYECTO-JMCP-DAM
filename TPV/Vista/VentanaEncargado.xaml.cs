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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TPV.Vista
{
    /// <summary>
    /// Lógica de interacción para VentanaEncargado.xaml
    /// </summary>
    public partial class VentanaEncargado : Window
    {
        private bool _cierreVentana = false; //Para controlar el cierre de la ventana
        public VentanaEncargado()
        {
            InitializeComponent();
            DoubleAnimation animacion = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(Window.OpacityProperty, animacion);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) //Manejador que hace un efecto de fade out al cerrar  la ventana
        {
            if (!_cierreVentana)
            {
                e.Cancel = true; //Evento de cancelación
                _cierreVentana = true;

                var animacionCierre = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3));
                animacionCierre.Completed += (s, a) => this.Close(); //Para cerrar la ventana al terminar la animación
                this.BeginAnimation(Window.OpacityProperty, animacionCierre);
            }
        }

        private void btnAtras_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAdmon_Click(object sender, RoutedEventArgs e)
        {
            new SubVentanaModificarUsuarios().ShowDialog();
        }

        private void btnAnulaciones_Click(object sender, RoutedEventArgs e)
        {
            new SubVentanaAnulaciones().ShowDialog();
        }

        private void btnAlta_Click(object sender, RoutedEventArgs e)
        {
            new SubVentanaNuevoUsuario().ShowDialog();
        }
    }
}
