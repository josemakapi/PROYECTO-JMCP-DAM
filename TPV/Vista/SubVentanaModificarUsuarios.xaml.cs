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
using TPV.Modelo;

namespace TPV.Vista
{
    /// <summary>
    /// Lógica de interacción para SubVentanaModificarUsuarios.xaml
    /// </summary>
    public partial class SubVentanaModificarUsuarios : Window
    {
        private List<Usuario> _usuariosLista;
        public List<Usuario> UsuariosLista { get => this._usuariosLista; set => this._usuariosLista = value; }
        private bool _cierreVentana = false; //Para controlar el cierre de la ventana

        public SubVentanaModificarUsuarios()
        {
            InitializeComponent();
            this._usuariosLista = new List<Usuario>();
            foreach (Usuario usuario in ControladorComun.TpvBase!.ListaUsuarios)
            {
                if (usuario.Id != ControladorComun.TpvBase!.UsuarioActual!.Id)
                {
                    this._usuariosLista.Add(usuario);
                }
            }
            //this.cmbUsuarios.ItemsSource = _usuariosLista;
            this.DataContext = this;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cmbUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e) //pdte controlar no desactivarse a sí mismo
        {
            this.RefrescarControles();
        }

        private void btnRol_Click(object sender, RoutedEventArgs e)
        {
            if (this.cmbUsuarios.SelectedItem != null)
            {
                Usuario usuario = (Usuario)this.cmbUsuarios.SelectedItem;
                if (usuario.EsEncargado)
                {
                    usuario.EsEncargado = false;
                }
                else
                {
                    usuario.EsEncargado = true;
                }
                this.SincronizarCambios(usuario);
                this.RefrescarControles();
            }
            else
            {
                MessageBox.Show(this, "Debe seleccionar un usuario", "Error");
            }
        }

        private void btnActivar_Click(object sender, RoutedEventArgs e)
        {
            if (this.cmbUsuarios.SelectedItem != null)
            {
                Usuario usuario = (Usuario)this.cmbUsuarios.SelectedItem;
                if (usuario.EsActivo)
                {
                    usuario.EsActivo = false;
                }
                else
                {
                    usuario.EsActivo = true;
                }
                this.SincronizarCambios(usuario);
                this.RefrescarControles();
            }
            else
            {
                MessageBox.Show(this, "Debe seleccionar un usuario", "Error");
            }
        }

        private void RefrescarControles()
        {
            if (this.cmbUsuarios.SelectedItem != null)
            {
                Usuario usuario = (Usuario)this.cmbUsuarios.SelectedItem;
                if (usuario.EsEncargado == true)
                {
                    this.btnRol.Content = "Hacer vendedor";
                }
                else
                {
                    this.btnRol.Content = "Hacer encargado";
                }

                if (usuario.EsActivo == true)
                {
                    this.btnActivar.Content = "Desactivar";
                }
                else
                {
                    this.btnActivar.Content = "Activar";
                }
            }
        }

        private void SincronizarCambios(Usuario usuario)
        {
            foreach (Usuario u in ControladorComun.TpvBase!.ListaUsuarios)
            {
                if (usuario.Id == u.Id)
                {
                    u.EsEncargado = usuario.EsEncargado;
                    u.EsActivo = usuario.EsActivo;
                    ControladorComun.BD!.ActualizarObjeto<Usuario>(u);
                }
            }
        }
    }
}
