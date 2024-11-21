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
using TPV.Controlador;

namespace TPV.Vista
{
    /// <summary>
    /// Lógica de interacción para VentanaAcciones.xaml
    /// </summary>
    public partial class VentanaAcciones : Window
    {
        private bool _cierreVentana = false; //Para controlar el cierre de la ventana
        public VentanaAcciones()
        {
            InitializeComponent();
            DoubleAnimation animacion = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(Window.OpacityProperty, animacion);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) //Manejador que hace un efecto de fade out al cerrar  la ventana
        {
            if (!this._cierreVentana)
            {
                e.Cancel = true; //Evento de cancelación
                this._cierreVentana = true;

                var animacionCierre = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3));
                animacionCierre.Completed += (s, a) => this.Close(); //Para cerrar la ventana al terminar la animación
                this.BeginAnimation(Window.OpacityProperty, animacionCierre);
            }
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnTemaAC_Click(object sender, RoutedEventArgs e)
        {
            this.GuardarTemaUsuario("TemaAC");
            ControladorComun.CambiarTema("TemaAC");
            
        }

        private void btnTemaOriginal_Click(object sender, RoutedEventArgs e)
        {
            this.GuardarTemaUsuario("TemaOriginal");
            ControladorComun.CambiarTema("TemaOriginal");
            
        }

        private void GuardarTemaUsuario(string TemaVisual)
        {
            ControladorComun.TpvBase!.UsuarioActual!.TemaVisual = TemaVisual;
            ControladorComun.BD!.ActualizarObjeto(ControladorComun.TpvBase!.UsuarioActual!);
        }
    }
}
