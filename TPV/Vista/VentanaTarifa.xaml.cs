using System.Windows;
using System.Windows.Media.Animation;
using TPV.Controlador;

namespace TPV.Vista
{
    /// <summary>
    /// Lógica de interacción para VentanaTarifa.xaml
    /// </summary>
    public partial class VentanaTarifa : Window
    {
        private bool _cierreVentana = false; //Para controlar el cierre de la ventana
        public VentanaTarifa()
        {
            InitializeComponent();
            cmbTarifa.ItemsSource = ControladorComun.TpvBase!.ListaTarifas();
            cmbTarifa.SelectedValue = ControladorComun.TpvBase!.TarifaActual;
            cmbTarifa.SelectedIndex = 0;
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

        private void btnTarifa_Click(object sender, RoutedEventArgs e)
        {
            if (cmbTarifa.SelectedItem != null)
            {
                ControladorComun.TpvBase!.TarifaActual = (Modelo.Tarifa)cmbTarifa.SelectedItem;
                this.Close();
            }
            else
            {
                MessageBox.Show("Debe seleccionar una tarifa","Información");
            }
        }
    }
}
