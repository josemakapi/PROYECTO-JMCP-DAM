using Informes.Controlador;

namespace Informes.Modelo
{
    public class TiendaInformes
    {
        private int _id;
        public int Id { get => _id; set => _id = value; }
        private int _codTienda;
        public int CodTienda { get => _codTienda; set => _codTienda = value; }
        private string? _otrosDatos;
        public string? OtrosDatos { get => _otrosDatos; set => _otrosDatos = value; }
        private string? _descripcion;
        public string? Descripcion { get => _descripcion; set => _descripcion = value; }
        private string? _uriImagen; //En vez de guardar la imagen y sobrecargar la BD, se guarda la URI. La imagen se genera cuando se necesite leyendo del disco mediante un método
        public string? UriImagen { get => _uriImagen; set => _uriImagen = value; }

        public TiendaInformes(int codTienda, string descripcion)
        {
            this._id = ControladorComun.CurrentBD!.SelectMAXInt("TiendaInformes", "_id") + 1;
            this._codTienda = codTienda;
            this._descripcion = descripcion;
        }
        public TiendaInformes(int id, int codTienda, string descripcion)
        {
            this._id = id;
            this._codTienda = codTienda;
            this._descripcion = descripcion;
        }
        public TiendaInformes(int id, int codTienda, string descripcion, string otrosDatos, string logo)
        {
            this._id = id;
            this._codTienda = codTienda;
            this._descripcion = descripcion;
            this._otrosDatos = otrosDatos;
            this._uriImagen = logo;
        }

        public override string ToString()
        {
            return this.Descripcion!;
        }
    }
}
