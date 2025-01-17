using Informes.Controlador;

namespace Informes.Modelo
{
    public class FormaPago
    {
        private int _id;
        public int Id { get => _id; set => _id = value; }
        private string _nombre;
        public string Nombre { get { return _nombre; } set { _nombre = value; } }
        private string _descripcion;
        public string Descripcion { get { return _descripcion; } set { _descripcion = value; } }

        public FormaPago(string nombre, string descripcion)
        {
            _id = ControladorComun.CurrentBD!.SelectMAXInt("FormaPago", "_id") + 1;
            _nombre = nombre;
            _descripcion = descripcion;
        }

        public override string ToString()
        {
            return $"Id: {Id}, Nombre: {Nombre}, Descripcion: {Descripcion}";
        }
    }
}
