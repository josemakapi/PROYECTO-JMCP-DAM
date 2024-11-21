using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using TPV.Controlador;
using TPV.Modelo;

namespace TPV.Vista
{
    /// <summary>
    /// Ventana que se carga al inicio del programa para seleccionar la tienda y conectarse a la BD
    /// </summary>
    public partial class VentanaPreInicio : Window
    {
        private bool flagCierreVentana;
        public VentanaPreInicio()
        {
            InitializeComponent();
            grpTienda.Visibility = Visibility.Hidden;
            btnTienda.Visibility = Visibility.Hidden;
            chkDebug.Visibility = Visibility.Hidden;
            chkNube.IsEnabled = false;
            flagCierreVentana = true;
        }

        private void txtBPuerto_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnConectar_Click(object sender, RoutedEventArgs e)
        {
            if (chkNube.IsChecked == true)
            {
                if (ControladorComun.IniciarBD(txtBUsuario.Text, txtBPassword.Password))
                {
                    ControladorComun.PreInicializaTienda();
                    OcultaControles();
                }
                else
                {
                    MessageBox.Show("Error al conectar a la BD. Corrija los datos e inténtelo de nuevo");
                }
            }
            else
            {
                if (ControladorComun.IniciarBD(txtBHost.Text, Convert.ToInt16(txtBPuerto.Text), txtBUsuario.Text, txtBPassword.Password))
                {
                    ControladorComun.PreInicializaTienda();
                    OcultaControles();
                }
                else
                {
                    MessageBox.Show("Error al conectar a la BD. Corrija los datos e inténtelo de nuevo");
                }
            }
        }

        private void OcultaControles()
        {
            cmbBTiendas.ItemsSource = ControladorComun.Tiendas;
            if (cmbBTiendas.Items.Count > 0) cmbBTiendas.SelectedValue = ControladorComun.Tiendas![0];
            grpBD.Visibility = Visibility.Hidden;
            grpTienda.Visibility = Visibility.Visible;
            btnTienda.Visibility = Visibility.Visible;
            btnConectar.Content = "¡Conectado!";
            btnConectar.IsEnabled = false;
            chkNube.Visibility = Visibility.Hidden;
            chkDebug.Visibility = Visibility.Visible;
        }

        private void btnTienda_Click(object sender, RoutedEventArgs e)
        {
            if (cmbBTiendas.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar una tienda");
            }
            else
            {
                ControladorComun.TiendaActual = (Tienda)cmbBTiendas.SelectedItem;
                if (chkDebug.IsChecked == true)
                {
                    ControladorComun.debugMode = true;
                }
                else
                {
                    ControladorComun.debugMode = false;
                }
                flagCierreVentana = false;
                this.Close();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!flagCierreVentana) ControladorComun.CargarPantallaVentas();
        }

        private void chkNube_Checked(object sender, RoutedEventArgs e)
        {
            chkCheck();
        }

        private void chkNube_Unchecked(object sender, RoutedEventArgs e)
        {
            chkCheck();
        }

        private void chkCheck()
        {
            if (chkNube.IsChecked == true)
            {
                txtBHost.IsEnabled = false;
                txtBPuerto.IsEnabled = false;
            }
            else
            {
                txtBHost.IsEnabled = true;
                txtBPuerto.IsEnabled = true;
            }
        }
    }
}
