using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TPV.Controlador;

namespace TPV.Modelo
{
    public class Usuario
    {
        private int _id;
        public int Id { get => this._id; set => this._id = value; }
        private int _codTienda;
        public int CodTienda { get => this._codTienda; set => this._codTienda = value; }
        private string _clave;
        public string Clave { get => this._clave; set => this._clave = value; }
        private bool _esEncargado;
        public bool EsEncargado { get => this._esEncargado; set => this._esEncargado = value; }
        private string _nombre;
        public string Nombre { get => this._nombre; set => this._nombre = value; }
        private Uri? _uriAvatar;
        public Uri? UriAvatar { get => this._uriAvatar; set => this._uriAvatar = value; }
        private bool _esActivo;
        public bool EsActivo { get => this._esActivo; set => this._esActivo = value; }
        private string? _TemaVisual;
        public string? TemaVisual { get => this._TemaVisual; set => this._TemaVisual = value; }

        /// <summary>
        /// Constructor en caso de BD's vacías
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clave"></param>
        /// <param name="codTienda"></param>
        /// <param name="esEncargado"></param>
        /// <param name="nombre"></param>
        /// <param name="uriAvatar"></param>
        /// <param name="temaVisual"></param>
        public Usuario(int id, string clave, int codTienda, bool esEncargado, string nombre, Uri uriAvatar, string temaVisual)
        {
            this._id = id;
            this._clave = clave;
            this._codTienda = codTienda;
            this._esEncargado = esEncargado;
            this._nombre = nombre;
            this._uriAvatar = uriAvatar;
            this._esActivo = true;
            this._TemaVisual = temaVisual; //Usaremos ControladorComun.CambiarTema(temaVisual) para cambiar el tema visual
        }

        public Usuario(string clave, int codTienda, bool esEncargado, string nombre, Uri uriAvatar, string temaVisual)
        {
            this._id = this._id = ControladorComun.BD!.SelectMAXInt("Usuario", "_id") + 1;
            this._clave = clave;
            this._codTienda = codTienda;
            this._esEncargado = esEncargado;
            this._nombre = nombre;
            this._uriAvatar = uriAvatar;
            this._esActivo = true;
            this._TemaVisual = temaVisual;
        }

        public Usuario(int id, string clave, bool esEncargado, string nombre)
        {
            this._id = id;
            this._clave = clave;
            this._esEncargado = esEncargado;
            this._nombre = nombre;
            this._esActivo = true;
            if (esEncargado)
            {
                this._uriAvatar = new Uri("pack://application:,,,/Recursos/Imagenes/encargado.png", UriKind.Absolute);
            }
            else
            {
                this._uriAvatar = new Uri("pack://application:,,,/Recursos/Imagenes/vendedor.png", UriKind.Absolute);
            }
            this.TemaVisual = "TemaOriginal";
        }

        public Usuario(string clave, bool esEncargado, string nombre)
        {
            this._id = ControladorComun.BD!.SelectMAXInt("Usuario", "_id") + 1;
            this._clave = clave;
            this._esEncargado = esEncargado;
            this._nombre = nombre;
            this._esActivo = true;
            if (esEncargado)
            {
                this._uriAvatar = new Uri("pack://application:,,,/Recursos/Imagenes/encargado.png", UriKind.Absolute);
            }
            else
            {
                this._uriAvatar = new Uri("pack://application:,,,/Recursos/Imagenes/vendedor.png", UriKind.Absolute);
            }
            this.TemaVisual = "TemaOriginal";
        }

        public BitmapImage ObtenerAvatar()
        {
            return new BitmapImage(this._uriAvatar);
        }

        public override string ToString()
        {
            return this._nombre;
        }

        private int maxId ()
        {
            int max = 0;
            foreach (Usuario usuario in ControladorComun.TpvBase!.ListaUsuarios)
            {
                if (usuario.Id > max)
                {
                    max = usuario.Id;
                }
            }
            return max;
        }
    }
}
