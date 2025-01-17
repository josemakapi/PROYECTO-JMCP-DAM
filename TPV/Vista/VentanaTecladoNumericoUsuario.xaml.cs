using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using TPV.Controlador;
using TPV.Modelo;

namespace TPV.Vista
{
    /// <summary>
    /// Teclado numérico que sirve tanto para validar claves de usuario como para introducir números
    /// </summary>
    public partial class VentanaTecladoNumericoUsuario : Window
    {
        private string _numeros;
        private bool _esClave;
        private bool _cierreVentana = false; //Para controlar el cierre de la ventana

        /// <summary>
        /// 
        /// </summary>
        /// <param name="esClave">True si se va a usar para validar claves de usuario. False si se va a usar para recoger valores numéricos</param>
        public VentanaTecladoNumericoUsuario(bool esClave)
        {
            this.InitializeComponent();
            this._numeros = string.Empty;
            this._esClave = esClave;
            DoubleAnimation animacion = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(Window.OpacityProperty, animacion);

            if (this._esClave)
            {
                this.VentanaTecladoNumericoUsuarioXAML.Title = "Introduzca su clave de usuario";
                this.lblVisor.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.VentanaTecladoNumericoUsuarioXAML.Title = "Introduzca el número";
                this.lblVisor.Visibility = Visibility.Collapsed;
            }
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

        private void ActualizarVisor()
        {
            if (this._numeros.Length < 1)
            {
                this.lblVisor.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.lblVisor.Visibility = Visibility.Visible;
                if (this._esClave)
                {
                    this.lblVisor.Content = new string('*', this._numeros.Length);
                }
                else
                {
                    this.lblVisor.Content = this._numeros;
                }
            }

        }
        private void btnBorrar_Click(object sender, RoutedEventArgs e)
        {
            if (this._numeros.Length > 0)
            {
                this._numeros = this._numeros.Substring(0, this._numeros.Length - 1);
            }
            this.ActualizarVisor();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (this._esClave)
            {
                if (ControladorComun.TpvBase!.CompruebaClave(this._numeros))
                {
                    ControladorComun.TpvBase!.UsuarioActual = ControladorComun.BD!.BuscarObjetosString<Usuario>("Clave", this._numeros)[0];
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Clave incorrecta");
                    this._numeros = string.Empty;
                    this.ActualizarVisor();
                }

            }
            else
            {
                ControladorComun.TpvBase!.NumerosTeclado = Convert.ToInt16(this._numeros);
                this.Close();
            }

        }

        private void btn7_Click(object sender, RoutedEventArgs e)
        {
            this._numeros += "7";
            this.ActualizarVisor();
        }
        private void btn0_Click(object sender, RoutedEventArgs e)
        {
            this._numeros += "0";
            this.ActualizarVisor();
        }
        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            this._numeros += "3";
            this.ActualizarVisor();
        }
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            this._numeros += "1";
            this.ActualizarVisor();
        }

        private void btn6_Click(object sender, RoutedEventArgs e)
        {
            this._numeros += "6";
            this.ActualizarVisor();
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            this._numeros += "2";
            this.ActualizarVisor();
        }

        private void btn5_Click(object sender, RoutedEventArgs e)
        {
            this._numeros += "5";
            this.ActualizarVisor();
        }

        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            this._numeros += "4";
            this.ActualizarVisor();
        }

        private void btn9_Click(object sender, RoutedEventArgs e)
        {
            this._numeros += "9";
            this.ActualizarVisor();
        }

        private void btn8_Click(object sender, RoutedEventArgs e)
        {
            this._numeros += "8";
            this.ActualizarVisor();
        }
    }
}
