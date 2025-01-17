using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TPV.Controlador;
using TPV.Modelo;

namespace TPV.Vista
{
    /// <summary>
    /// Lógica de interacción para VentanaTicketPantalla.xaml
    /// </summary>
    public partial class VentanaTicketPantalla : Window
    {
        private Ticket? _ticketActual;
        private bool _cierreVentana = false; //Para controlar el cierre de la ventana
        public VentanaTicketPantalla()
        {
            InitializeComponent();
            this.imgLogo.Source=ControladorComun.LeerImagen(ControladorComun.TpvBase!.TiendaActual!.UriImagen!);
            this.DataContext = ControladorComun.TpvBase;
            this.dGridPosVenta.ItemsSource = ControladorComun.TpvBase!.PosicionVentaActual!.LineasPantalla;
            this._ticketActual = ControladorComun.TpvBase!.GeneraNuevoTicket(false);
            if (this._ticketActual != null) this._ticketActual.FechaHora = DateTime.Now;
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

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            if (this._ticketActual!.Impreso) ControladorComun.TpvBase!.PosicionVentaActual!.LineasPantalla.Clear();            
            this.Close();
        }

        private void btnCobrar_Click(object sender, RoutedEventArgs e)
        {
            if (ControladorComun.TpvBase!.FormasPago!.Count == 0)
            {
                MessageBox.Show(this, "No hay formas de pago configuradas", "Error");
                return;
            }

            if (this._ticketActual == null)
            {
                MessageBox.Show(this, "Error al generar el ticket", "Error");
            }
            else
            {

                if (this._ticketActual.Impreso) //Si está impreso cambiamos el botón de cobrar por el de imprimir
                {
                    this.btnCobrar.Content = "Imprimir";
                    try
                    {
                        string formatoTicket = string.Empty;
                        if ((formatoTicket = ControladorComun.TpvBase!.ImprimirTicket(this._ticketActual)) == "Error")
                        {
                            throw new Exception();
                        }
                        else
                        {
                            if (ControladorComun.debugMode)
                            {
                                //Sacaremos el ticket en una ventana especial. Lo he intentado con MessageBox pero no hay forma de alinear las columnas
                                Window ticketWindow = new Window
                                {
                                    Title = "Ticket en pantalla - " + this._ticketActual.NumTicket,
                                    Width = 400,
                                    Height = 300
                                };
                                TextBox ticketTextBox = new TextBox
                                {
                                    FontFamily = new System.Windows.Media.FontFamily("Consolas"), // Fuente monoespaciada para que aparezcan las columnas alineadas
                                    FontSize = 14,
                                    TextWrapping = TextWrapping.Wrap,
                                    IsReadOnly = true,
                                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                                };
                                ticketTextBox.Text = formatoTicket;
                                ticketWindow.Content = ticketTextBox;
                                ticketWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                                ticketWindow.Height = 600;
                                ticketWindow.ShowDialog();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Error al imprimir el ticket: " + ex.Message, "Error");
                    }
                }
                else 
                {
                    
                    //Sacar ventana preguntando por la forma de pago
                    Window SubVentanaCobro = new SubVentanaCobro(this._ticketActual);
                    SubVentanaCobro.ShowDialog();
                    if (this._ticketActual.Impreso)
                    {
                        this.btnCobrar.Content = "Imprimir";
                    }
                }
            }
        }
    }
}
