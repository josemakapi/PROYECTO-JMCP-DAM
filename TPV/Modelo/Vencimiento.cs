using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPV.Controlador;

namespace TPV.Modelo
{
    public class Vencimiento //No es necesario persistir directamente en la BD. Pero sí que sea volcado al ticket. Se persistirá dentro del ticket cuando éste se persista  
    {
        private int _id;
        public int Id { get { return _id; } set { _id = value; } }
        private string _ticketNum;
        public string TicketNum { get { return _ticketNum; } set { _ticketNum = value; } }
        private string _formaPago;
        public string FormaPago { get { return _formaPago; } set { _formaPago = value; } }
        private double _total;
        public double Total { get { return _total; } set { _total = value; } }
        private double _entregado;
        public double Entregado { get { return _entregado; } set { _entregado = value; } }
        private double _devuelto;
        public double Devuelto { get { return _devuelto; } set { _devuelto = value; } }

        /// <summary>  
        /// Constructor para formas de pago exactas, como tarjetas. Total = entregado  
        /// </summary>  
        /// <param name="ticketNum"></param>  
        /// <param name="formaPago"></param>  
        /// <param name="entregado"></param>  
        public Vencimiento(string ticketNum, string formaPago, double entregado)
        {
            this._id = ControladorComun.BD!.SelectMAXInt("Vencimiento", "_id") + 1;
            this._ticketNum = ticketNum;
            this._formaPago = formaPago;
            this._entregado = entregado;
            this._total = entregado;
        }

        /// <summary>  
        /// Constructor para formas de pago con devolución, como efectivo. Total = entregado - devuelto  
        /// </summary>  
        /// <param name="ticketNum"></param>  
        /// <param name="formaPago"></param>  
        /// <param name="entregado"></param>  
        /// <param name="devuelto"></param>  
        public Vencimiento(string ticketNum, string formaPago, double entregado, double devuelto) : this(ticketNum, formaPago, entregado)
        {
            this._devuelto = devuelto;
            this._total = entregado - devuelto;
        }
    }
}
