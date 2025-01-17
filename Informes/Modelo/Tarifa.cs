using Informes.Controlador;

namespace Informes.Modelo
{
    public class Tarifa
    {
        private int _id;
        public int Id { get => _id; set => _id = value; }
        private int _codTarifa;
        public int CodTarifa { get => _codTarifa; set => _codTarifa = value; }
        private double _iva;
        /// <summary>
        /// IVA, expresado en decimales. Por ejemplo, 0.21 para un 21%
        /// </summary>
        public double Iva { get => _iva; set => _iva = value; }
        private int _codTienda;
        public int CodTienda { get => _codTienda; set => _codTienda = value; }
        private string _nombre;
        public string Nombre { get => _nombre; set => _nombre = value; }
        private string _descripcion;
        public string Descripcion { get => _descripcion; set => _descripcion = value; }

        private List<ObjProdPrecio> _listaProductos; //CodProducto, Precio, CodTarifa
        public List<ObjProdPrecio> ListaProductos { get => _listaProductos; set => _listaProductos = value; }

        public Tarifa(int codTarifa, string nombre, int codTienda,  double iva, string descripcion)
        {
            this._id = ControladorComun.CurrentBD!.SelectMAXInt("Tarifa", "_id") + 1;
            this._codTarifa = codTarifa;
            this._iva = iva;
            this._codTienda = codTienda;
            this._nombre = nombre;
            this._descripcion = descripcion;
            this._listaProductos = new List<ObjProdPrecio>();
        }
        public Tarifa(int id, int codTarifa, string nombre, int codTienda, double iva, string descripcion)
        {
            this._id = id;
            this._codTarifa = codTarifa;
            this._iva = iva;
            this._codTienda = codTienda;
            this._nombre = nombre;
            this._descripcion = descripcion;
            this._listaProductos = new List<ObjProdPrecio>();
        }

        public void AnadirProducto(Producto producto, double precioConIva, bool persistencia)
        {
            ObjProdPrecio obj = new ObjProdPrecio(producto.CodProducto, this.CodTarifa, precioConIva);
            _listaProductos!.Add(obj);
            if (persistencia)
            {
                if (ControladorComun.CurrentBD!.BuscarObjetosIntAndInt<ObjProdPrecio>("CodProducto", producto.CodProducto, "CodTarifa", this._codTarifa).Count < 1) //Si no existía el producto en la tarifa   
                {
                    ControladorComun.CurrentBD!.PersistirObjeto(obj); //Lo persistimos
                }
                ControladorComun.CurrentBD!.PersistirObjeto(this); //Persistimos la tarifa 
            }
        }
        public void AnadirProducto(int codProducto, double precioConIva, bool persistencia)
        {
            ObjProdPrecio obj = new ObjProdPrecio(codProducto, this.CodTarifa, precioConIva);
            _listaProductos!.Add(obj);
            if (persistencia)
            {
                if (ControladorComun.CurrentBD!.BuscarObjetosIntAndInt<ObjProdPrecio>("CodProducto", codProducto, "CodTarifa", this._codTarifa).Count < 1)
                {
                    ControladorComun.CurrentBD!.PersistirObjeto(obj);
                }
                ControladorComun.CurrentBD!.PersistirObjeto(this);
            }
        }

        public bool EliminarProducto(int codProducto, bool persistencia)
        {

            if (this._listaProductos!.Remove(_listaProductos.Find(x => x.CodProducto == codProducto)!))
            {
                if (persistencia)
                {
                    ControladorComun.CurrentBD!.EliminarObjeto<ObjProdPrecio>(_listaProductos.Find(x => x.CodProducto == codProducto)!);
                    ControladorComun.CurrentBD!.ActualizarObjeto(this);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool EliminarPrecioProducto(Producto producto, bool persistencia)
        {
            if (this._listaProductos!.Remove(_listaProductos.Find(x => x.CodProducto == producto.CodProducto)!))
            {
                if (persistencia)
                {
                    ControladorComun.CurrentBD!.EliminarObjeto<ObjProdPrecio>(_listaProductos.Find(x => x.CodProducto == producto.CodProducto)!);
                    ControladorComun.CurrentBD!.ActualizarObjeto(this);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public double ObtenerPrecioProducto(int codProducto)
        {
            return _listaProductos!.Find(x => x.CodProducto == codProducto)!.Precio;
        }

        public double ObtenerPrecioSinIvaProducto(int codProducto)
        {
            return _listaProductos!.Find(x => x.CodProducto == codProducto)!.Precio/(this._iva+1);
        }

        public void ModificarPrecioProducto(int codProducto, double precio, bool persistencia)
        {
            ObjProdPrecio obj = _listaProductos!.Find(x => x.CodProducto == codProducto)!;
            obj.Precio = precio;
            if (persistencia)
            {
                ControladorComun.CurrentBD!.ActualizarObjeto(obj);
                ControladorComun.CurrentBD!.ActualizarObjeto(this);
            }
        }

        public override string ToString()
        {
            return this.Nombre;
        }
    }
}
