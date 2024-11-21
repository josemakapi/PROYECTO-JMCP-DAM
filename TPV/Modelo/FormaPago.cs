using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPV.Controlador;

namespace TPV.Modelo
{
    /// <summary>
    /// Tal como está definida sólamente tiene fines descriptivos, pero
    /// een el futuro permitirá añadir otras funcionalidades
    /// </summary>
    public class FormaPago
    {
        private int _id;
        public int Id { get { return _id; } set { _id = value; } }
        private string _nombre;
        private int _codTienda;
        public int CodTienda { get => _codTienda; set => _codTienda = value; }
        public string Nombre { get { return _nombre; } set { _nombre = value; } }
        private string _descripcion;
        public string Descripcion { get { return _descripcion; } set { _descripcion = value; } }
        private bool _sinDevolucion;
        public bool SinDevolucion { get => _sinDevolucion; set => _sinDevolucion = value; }

        public FormaPago(string nombre, string descripcion, int codTienda, bool sinDev)
        {
            this._id = ControladorComun.BD!.SelectMAXInt("FormaPago", "_id") + 1;
            this._nombre = nombre;
            this._descripcion = descripcion;
            this._codTienda = codTienda;
            this._sinDevolucion = sinDev;
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
