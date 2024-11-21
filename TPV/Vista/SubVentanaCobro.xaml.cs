using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TPV.Controlador;
using TPV.Modelo;

namespace TPV.Vista
{
    /// <summary>
    /// Lógica de interacción para SubVentanaCobro.xaml
    /// </summary>
    public partial class SubVentanaCobro : Window
    {
        private Ticket _ticketActual;
        private bool _cierreVentana = false; //Para controlar el cierre de la ventana

        public SubVentanaCobro(Ticket ticket)
        {
            InitializeComponent();
            this._ticketActual = ticket;
            this.DataContext = ControladorComun.TpvBase;
            this.cmbFPago.SelectedItem = ControladorComun.TpvBase!.FormasPago!.FirstOrDefault(x => x.Nombre == "Efectivo");
            DoubleAnimation animacion = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(Window.OpacityProperty, animacion);
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
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

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCobrar_Click(object sender, RoutedEventArgs e)
        {
            if (this.cmbFPago.SelectedItem == null)
            {
                MessageBox.Show(this, "Debe seleccionar una forma de pago", "Error");
                return;
            }
            
            if (this.txtEntregado == null || this.txtEntregado.Text == string.Empty)
            {
                MessageBox.Show(this, "Debe introducir el importe entregado", "Error");
                txtEntregado!.Focus();
            }
            else
            {
                FormaPago nuevaFPago = (FormaPago)cmbFPago.SelectedItem!;
                Vencimiento nuevoVencimiento;
                try
                {
                    if (this.txtDevuelto.Text == string.Empty)
                    { //Tarjeta
                        nuevoVencimiento = new Vencimiento(this._ticketActual.NumTicket, nuevaFPago.Nombre, Double.Parse(this.txtEntregado.Text, new CultureInfo("es-ES")));
                    }
                    else
                    { //Efectivo
                        if ((Double.Parse(this.txtDevuelto.Text) < 0) || (Math.Round(this._ticketActual.CalcularTotal(), 2) - (Math.Round(Double.Parse(this.txtEntregado.Text, new CultureInfo("es-ES")),2) - Math.Round(Double.Parse(this.txtDevuelto.Text, new CultureInfo("es-ES")),2)) != 0))
                        {
                            MessageBox.Show(this, "Hay un descuadre entre el entregado y el devuelto", "Error");
                            return;
                        }
                        nuevoVencimiento = new Vencimiento(this._ticketActual.NumTicket, nuevaFPago.Nombre, Double.Parse(this.txtEntregado.Text, new CultureInfo("es-ES")), Double.Parse(this.txtDevuelto.Text, new CultureInfo("es-ES")));
                    }

                    this._ticketActual.Vencimientos.Add(nuevoVencimiento);
                    this._ticketActual.Impreso = true;
                    this._ticketActual.FechaHoraImpresion = DateTime.Now;
                    try
                    {
                        ControladorComun.BD!.PersistirObjeto(_ticketActual);

                        string formatoTicket = string.Empty;
                        if ((formatoTicket = ControladorComun.TpvBase!.ImprimirTicket(_ticketActual)) == "Error")
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
                        MessageBox.Show(this, "Error al persistir el ticket: " + ex.Message, "Error");
                    }
                    
                    this.Close();
                }
                catch (FormatException)
                {
                    MessageBox.Show(this, "Error al escribir el valor entregado. Por favor, introduzca un número válido.", "Error");
                }
            }
        }

        private void cmbFPago_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FormaPago nueva = (FormaPago)this.cmbFPago.SelectedItem;
            if (nueva.SinDevolucion)
            {//Tarjetas
                this.txtDevuelto.Visibility = Visibility.Collapsed;
                this.lblDevuelto.Visibility = Visibility.Collapsed;
                Grid.SetColumnSpan(txtEntregado, 2);
                Grid.SetColumnSpan(lblEntregado, 2);
                this.txtEntregado.Text = Math.Round(this._ticketActual.CalcularTotal(),2).ToString();
            }
            else
            {//Efectivo
                Grid.SetColumnSpan(txtEntregado, 1);
                Grid.SetColumnSpan(lblEntregado, 1);
                this.txtDevuelto.Visibility = Visibility.Visible;
                this.lblDevuelto.Visibility = Visibility.Visible;
            }
        }

        private void ValidarMascaraPrecio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Para validar solo números, un punto decimal y hasta dos decimales
            Regex regex = new Regex(@"^\d*\,?\d{0,2}$");
            TextBox textBox = sender as TextBox;
            string newText = textBox!.Text.Insert(textBox.SelectionStart, e.Text);
            e.Handled = !regex.IsMatch(newText);
        }

        private void txtEntregado_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.txtEntregado.Text.Length < 1)
            {
                this.txtEntregado.Text = "0";
                if (Double.Parse(txtDevuelto.Text, new CultureInfo("es-ES")) < 0) { txtDevuelto.Text = "0"; }
            }
            FormaPago nueva = (FormaPago)this.cmbFPago.SelectedItem;
            if (!nueva.SinDevolucion)
            {
                this.txtDevuelto.Text = Math.Round((Double.Parse(this.txtEntregado.Text, new CultureInfo("es-ES")) - this._ticketActual.CalcularTotal()),2).ToString();
            }
        }

        private void txtEntregado_LostFocus(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
