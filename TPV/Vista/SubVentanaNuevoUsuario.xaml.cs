using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TPV.Controlador;
using TPV.Modelo;


namespace TPV.Vista
{
    /// <summary>
    /// Lógica de interacción para SubVentanaNuevoUsuario.xaml
    /// </summary>
    public partial class SubVentanaNuevoUsuario : Window
    {
        private bool _cierreVentana = false; //Para controlar el cierre de la ventana
        public SubVentanaNuevoUsuario()
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

        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            //Comprobar que no exista ya la clave en la tienda en la que estamos
            if ((this.txtClave.Password.Trim() != "") && (this.txtNombre.Text.Trim() != "") && (this.txtRepetirClave.Password.Trim() != ""))
            {
                if (this.txtClave.Password == this.txtRepetirClave.Password)
                {
                    if (Controlador.ControladorComun.TpvBase!.ListaUsuarios.Exists(x => x.Clave == this.txtClave.Password))
                    {
                        MessageBox.Show(this, "Ya existe un usuario con esa clave", "Error");
                        return;
                    }
                    else
                    {
                        //Por fin, crear usuario
                        Usuario nuevoUsuario = new Usuario(this.txtClave.Password, cmbRol.Text == "Encargado", this.txtNombre.Text);                        
                        try
                        {
                            ControladorComun.BD!.PersistirObjeto<Usuario>(nuevoUsuario);
                            Controlador.ControladorComun.TpvBase!.ListaUsuarios.Add(nuevoUsuario);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, "Error al crear el usuario: " + ex.Message, "Error");
                            return;
                        }
                        MessageBox.Show(this, "Usuario creado correctamente", "Información");
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show(this, "Las claves no coinciden", "Error");
                    return;
                }
            }
            else
            {
                MessageBox.Show(this, "Debe rellenar todos los campos", "Error");
                return;
            }
           

            
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtClave_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true; // Bloquear si no es un número
                return;
            }
            //e.Handled = regex.IsMatch(e.Text);
            if (this.txtClave.Password.Length >= 6)
            {
                e.Handled = true; //Que no supere los 6 caracteres
            }
        }
    }
}
