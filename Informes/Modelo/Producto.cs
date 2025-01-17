using Informes.Controlador;
//using Windows.UI.Xaml.Media.Imaging;

namespace Informes.Modelo
{
    public class Producto
    {
        private int _id;
        public int Id { get => _id; set => _id = value; }
        private string _nombre;
        public string Nombre { get => _nombre; set => _nombre = value; }
        private int _codProducto;
        public int CodProducto { get => _codProducto; set => _codProducto = value; }
        private string? _descripcion;
        private int _codTienda;
        public int CodTienda { get => _codTienda; set => _codTienda = value; }
        public string? Descripcion { get => _descripcion; set => _descripcion = value; }
        private string? _uriImagen; //En vez de guardar la imagen y sobrecargar la BD, se guarda la URI. La imagen se genera cuando se necesite leyendo del disco mediante un método
        public string? UriImagen { get => _uriImagen; set => _uriImagen = value; }

        public Producto(int codProd, string nombre, int codTienda)
        {
            _id = ControladorComun.CurrentBD!.SelectMAXInt("Producto", "_id") + 1;
            _codProducto = codProd;
            _nombre = nombre;
            _codTienda = codTienda;
        }
        public Producto(int id, int codProd, string nombre, int codTienda)
        {
            _id = id;
            _codProducto = codProd;
            _nombre = nombre;
            _codTienda = codTienda;
        }

        public Producto(int codProd, string nombre, string descripcion, int codTienda)
        {
            _id = ControladorComun.CurrentBD!.SelectMAXInt("Producto", "_id") + 1;
            _codProducto = codProd;
            _nombre = nombre;
            _descripcion = descripcion;
            _codTienda = codTienda;
        }

        public Producto(int codProd, string nombre, string descripcion, int codTienda, string uriImagen)
        {
            _id = ControladorComun.CurrentBD!.SelectMAXInt("Producto", "_id") + 1;
            _codProducto = codProd;
            _nombre = nombre;
            _descripcion = descripcion;
            _codTienda = codTienda;
            _uriImagen = uriImagen;
        }
        public Producto(int id, int codProd, string nombre, string descripcion, int codTienda, string uriImagen)
        {
            _id = id;
            _codProducto = codProd;
            _nombre = nombre;
            _descripcion = descripcion;
            _codTienda = codTienda;
            _uriImagen = uriImagen;
        }

    }
}
