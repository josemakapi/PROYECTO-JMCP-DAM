using Informes.Controlador;
using Informes.Recursos.Strings;
using Informes.Recursos.Styles;
using Informes.Recursos.Themes;
namespace Informes.Modelo
{
    public class UsuarioInformes
    {
        private int _id;
        public int Id { get => _id; set => _id = value; }
        private string _nombre;
        public string Nombre { get => _nombre; set => _nombre = value; }
        private string _password;
        public string Password { get => _password; set => _password = value; }
        private string _idioma;
        public string Idioma { get => _idioma; set => _idioma = value; }
        private int _perfil;
        public int Perfil { get => _perfil; set => _perfil = value; }
        private TiendaInformes _tienda;
        public TiendaInformes Tienda { get => _tienda; set => _tienda = value; }
        private string _visualTheme;
        public string VisualTheme { get => _visualTheme; set => _visualTheme = value; }
        private string _tamFuente;
        public string TamFuente { get => _tamFuente; set => _tamFuente = value; }

        public UsuarioInformes(string nombre, int perfil, string password, TiendaInformes tienda, string idioma, string visualTheme, string tamFuente)
        {
            this._id = ControladorComun.CurrentBD!.SelectMAXInt("UsuarioInformes", "_id") + 1; ;
            this._nombre = nombre;
            this._perfil = perfil;
            this._password = password;
            this._tienda = tienda;
            this._idioma = idioma;
            this._visualTheme = visualTheme;
            this._tamFuente = tamFuente;
        }

        public void ActualizaEstilos(bool modoOnline)
        {
            ICollection<ResourceDictionary> misDiccionarios = Application.Current!.Resources.MergedDictionaries;
            misDiccionarios.Clear();

            if (modoOnline)
            {
                switch (VisualTheme)
                {
                    case "TemaOriginal":
                        misDiccionarios.Add(new TemaOriginal());
                        break;
                    case "TemaAltoContraste":
                        misDiccionarios.Add(new TemaAltoContraste());
                        break;
                }
                switch (Idioma)
                {
                    case "ES":
                        misDiccionarios.Add(new Castellano());
                        break;
                    case "EN":
                        misDiccionarios.Add(new English());
                        break;
                }
                switch (TamFuente)
                {
                    case "big":
                        misDiccionarios.Add(new BigFontsMode());
                        break;
                    case "little":
                        misDiccionarios.Add(new SmallFontsMode());
                        break;
                }
            }
            else
            {
                misDiccionarios.Add(new Informes.Recursos.Themes.TemaOriginal());
                misDiccionarios.Add(new Informes.Recursos.Strings.Castellano());
                misDiccionarios.Add(new Informes.Recursos.Styles.SmallFontsMode());
            }
        }
        public override string ToString()
        {
            return this.Nombre;
        }
    }
}
