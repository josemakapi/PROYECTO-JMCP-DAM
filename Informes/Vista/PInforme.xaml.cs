using Informes.Controlador;
using Informes.Modelo;
using MongoDB.Driver.Linq;

namespace Informes.Vista
{
    public partial class PInforme : ContentPage
    {
        private DateTime FechaInforme;
        private double TotalDevoluciones;
        private double TotalVenta;
        private double ImporteMasBajo;
        private double ImporteMasAlto;
        private double TicketMedio;
        private double MediaTodosTickets;
        private int NumeroTickets;
        private string MejorVendedor;
        private List<Ticket> listaTickets;

        public PInforme()
        {
            InitializeComponent();
            this.listaTickets = new List<Ticket>();
            this.calendarioInforme.Date = DateTime.Now;
            this.MejorVendedor = "";
        }

        private void calendarioInforme_DateSelected(object sender, DateChangedEventArgs e)
        {
            this.FechaInforme = calendarioInforme.Date;
            this.listaTickets = new List<Ticket>();
            if (CargarTickets())
            {
                CalcularEstadisticas();
                PintarTarjetas();
            }
            else
            {
                this.FechaInforme = e.OldDate;
            }
        }

        private void PintarTarjetas()
        {
            this.labelNumeroTickets.Text = NumeroTickets.ToString();
            this.labelTotalDevoluciones.Text = $"{TotalDevoluciones:F2} �";
            this.labelTicketMin.Text = $"{ImporteMasBajo:F2} �";
            this.labelTicketMax.Text = $"{ImporteMasAlto:F2} �";
            this.labelMejorVendedor.Text = MejorVendedor;
            this.labelValorMedioTicket.Text = $"{TicketMedio:F2} �";
            this.labelTotalVenta.Text = $"{TotalVenta:F2} �";
            this.labelValorMedioTodosTicket.Text = $"{MediaTodosTickets:F2} �";
        }

        private bool CargarTickets()
        {

            this.listaTickets = ControladorComun.CurrentBD!.LeerObjetosTipo<Ticket>()
                .Where(ticket => DateOnly.FromDateTime(ticket.FechaHora) == DateOnly.FromDateTime(FechaInforme))
                .ToList();

            if (listaTickets.Count == 0)
            {
                DisplayAlert("Info", "No hay tickets", "OK");
                this.labelNumeroTickets.Text = "0";
                this.labelTotalDevoluciones.Text = $"{0.00} �";
                this.labelTicketMin.Text = $"{0.00:F2} �";
                this.labelTicketMax.Text = $"{0.00:F2} �";
                this.labelMejorVendedor.Text = "N/A";
                this.labelValorMedioTicket.Text = $"{0.00:F2} �";
                this.labelTotalVenta.Text = $"{0.00:F2} �";
                this.labelValorMedioTodosTicket.Text = $"{0.00:F2} �";
                return false;
            }

            this.NumeroTickets = listaTickets.Count;
            return true;
        }
        private void CalcularEstadisticas()
        {
            this.TotalDevoluciones = this.listaTickets
                .Where(ticket => ticket.Tipo == 'D')
                .Sum(ticket => ticket.Lineas.Sum(linea => linea.Precio));
            if (this.TotalDevoluciones >= 0) this.TotalDevoluciones = 0;
            this.TotalVenta = this.listaTickets
                .Where(ticket => ticket.Tipo == 'T')
                .Sum(ticket => ticket.Lineas.Sum(linea => linea.Precio));
            this.ImporteMasBajo = this.listaTickets
                .Where(ticket => ticket.Tipo == 'T')
                .Min(ticket => ticket.Lineas.Sum(linea => linea.Precio));
            this.ImporteMasAlto = this.listaTickets
                .Where(ticket => ticket.Tipo == 'T')
                .Max(ticket => ticket.Lineas.Sum(linea => linea.Precio));
            this.TicketMedio = (this.ImporteMasAlto + this.ImporteMasBajo) / 2;
            this.MediaTodosTickets = listaTickets
                .Sum(ticket => ticket.Lineas.Sum(linea => linea.Precio)) / this.listaTickets.Count;
            this.MejorVendedor = CalcularMejorVendedor();

        }
        private string CalcularMejorVendedor()
        {
            string MejorVendedor = "";
            Dictionary<string, double> vendedores = new Dictionary<string, double>();
            foreach (Ticket ticket in listaTickets)
            {
                if (ticket.Tipo == 'T')
                {
                    if (vendedores.ContainsKey(ticket.Vendedor.Nombre))
                    {
                        vendedores[ticket.Vendedor.Nombre] += ticket.Lineas.Sum(linea => linea.Precio);
                    }
                    else
                    {
                        vendedores.Add(ticket.Vendedor.Nombre, ticket.Lineas.Sum(linea => linea.Precio));
                    }
                }
            }

            for (int i = 0; i < vendedores.Count; i++)
            {
                if (vendedores.ElementAt(i).Value >= vendedores.Values.Max())
                {
                    MejorVendedor = vendedores.ElementAt(i).Key;
                }
            }
            return MejorVendedor;
        }
    }
}
