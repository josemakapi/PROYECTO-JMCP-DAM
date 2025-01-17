using System.Windows;
using TPV.Modelo;
using TPV.Controlador;
using System.Windows.Media.Animation;

namespace TPV.Vista
{
    /// <summary>
    /// Lógica de interacción para SubVentanaAnulaciones.xaml
    /// </summary>
    public partial class SubVentanaAnulaciones : Window
    {
        private List<Ticket> _listaTickets; //Sólo los del día
        public List<Ticket> ListaTickets { get => _listaTickets; set => _listaTickets = value; }
        private bool _cierreVentana = false; //Para controlar el cierre de la ventana

        public SubVentanaAnulaciones()
        {
            InitializeComponent();
            this._listaTickets = ControladorComun.BD!.LeerObjetosTipo<Ticket>()
                    .Where(x => x.FechaHora.DayOfYear == DateTime.Today.DayOfYear
                        && x.FechaHora.Year == DateTime.Today.Year
                        && x.Tipo == 'T'
                        && x.Anulado == false)
                    .ToList();
            DataContext = this;
            this.cmbTicket!.SelectedIndex = 0;
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

        //private void CargarTickets()
        //{
        //    if (this.cmbTicket != null) this.cmbTicket.ItemsSource = null;
        //    if (this._listaTickets != null) this._listaTickets.Clear();
        //    this._listaTickets = ControladorComun.BD!.LeerObjetosTipo<Ticket>()
        //            .Where(x => x.FechaHora.DayOfYear == DateTime.Today.DayOfYear
        //                && x.FechaHora.Year == DateTime.Today.Year
        //                && x.Anulado == false
        //                && x.Tipo == 'T')
        //            .ToList();
        //    this.cmbTicket!.ItemsSource = this._listaTickets;
        //    this.cmbTicket!.SelectedIndex = 0;
        //}

        private void btnAnular_Click(object sender, RoutedEventArgs e)
        {
            if (this.cmbTicket.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un ticket", "Error");
            }
            else
            {
                if (txtMotivo.Text.Trim() == "")
                {
                    MessageBox.Show("Debe introducir un motivo", "Error");
                }
                else
                {
                    Ticket ticket = (Ticket)cmbTicket.SelectedItem!;
                    //llamar
                    ControladorComun.TpvBase!.AnularTicket(ticket, txtMotivo.Text);

                    //this._listaTickets = ControladorComun.BD!.LeerObjetosTipo<Ticket>()
                    //.Where(x => x.FechaHora.DayOfYear == DateTime.Today.DayOfYear
                    //    && x.FechaHora.Year == DateTime.Today.Year
                    //    && x.Anulado == false
                    //    && x.NumTicket.Substring(0) == "T") //Para excluir los ticket tipo anulaciones
                    //.ToList();
                    cmbTicket.SelectedIndex = 0;
                    txtMotivo.Clear();
                    MessageBox.Show("Ticket anulado correctamente", "Información");
                    //this.CargarTickets();
                    this.Close();
                }
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
