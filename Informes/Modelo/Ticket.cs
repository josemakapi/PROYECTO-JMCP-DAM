using Informes.Controlador;


namespace Informes.Modelo
{
    public class Ticket
    {
        private int _id;
        public int Id { get => _id; set => _id = value; }
        private DateTime _fechaHora;
        public DateTime FechaHora { get { return _fechaHora; } set { _fechaHora = value; } }
        private string _numTicket; //Lo mantenemos como miembro privado porque si no, MongoDB no lo persiste
        //Ojo a a como se construye el NumTicket final: T-001-0000001 >> T = Tipo, 001 = CodTienda, 0000001 = Serie
        public string NumTicket { get { return char.ToUpper(this._tipo).ToString() + "-" + this._codTienda.ToString().PadLeft(3, '0') + "-" + this._soloNumero.ToString().PadLeft(7, '0'); } set { _numTicket = value; } }
        private long _soloNumero;
        public long SoloNumero { get { return _soloNumero; } set { _soloNumero = value; } }
        private char _tipo;
        public char Tipo { get { return _tipo; } set { _tipo = value; } } //T = Ticket normal, D = Devolucion
        private int _numTPV;
        public int NumTPV { get { return _numTPV; } set { _numTPV = value; } }
        private Usuario _vendedor;
        public Usuario Vendedor { get { return _vendedor; } set { _vendedor = value; } }
        private int _codTienda;
        public int CodTienda { get { return _codTienda; } set { _codTienda = value; } }
        private string _tienda;
        public string Tienda { get { return _tienda; } set { _tienda = value; } }
        private List<Linea> _lineas;
        public List<Linea> Lineas { get { return _lineas; } set { _lineas = value; } }
        private bool _cerrado;
        public bool Cerrado { get { return _cerrado; } set { _cerrado = value; } }
        private DateTime? _fechaHoraCierre;
        public DateTime? FechaHoraCierre { get { return _fechaHoraCierre; } set { _fechaHoraCierre = value; } }
        private List<Vencimiento> _vencimientos; //Vencimiento: Cantidad, FormaPago
        public List<Vencimiento> Vencimientos { get { return _vencimientos; } set { _vencimientos = value; } }
        public double Total { get { return CalcularTotal(); } }
        private bool _anulado;
        public bool Anulado { get { return _anulado; } set { _anulado = value; } }
        private string? _motivoAnulacion;
        public string? MotivoAnulacion { get { return _motivoAnulacion; } set { _motivoAnulacion = value; } }
        private bool _impreso;
        public bool Impreso { get { return _impreso; } set { _impreso = value; } }
        private DateTime _fechaHoraImpresion;
        public DateTime FechaHoraImpresion { get { return _fechaHoraImpresion; } set { _fechaHoraImpresion = value; } }


        public double CalcularTotal()
        {
            double total = 0;
            foreach (Linea linea in _lineas)
            {
                total += linea.Precio;
            }
            return total;
        }

        public override string ToString()
        {
            return this.NumTicket;
        }
    }
}
