using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPV.Modelo
{
    /// <summary>
    /// Sólamente usaremos cliente genérico porque vamos a trabajar con facturas simplificadas.
    /// En el futuro se permitirá hacer factura nominativa y se crearán más propiedades.
    /// </summary>
    public class Cliente
    {
        private int _id;
        public int Id { get => _id; set => _id = value; }
        private string _nombre;
        public string Nombre { get => _nombre; set => _nombre = value; } 

        public Cliente(int id, string nombre)
        {
            _id = id;
            _nombre = nombre;
        }

    }
}
