using Informes.Modelo;
using Java.Util.Functions;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using static Android.Provider.MediaStore;

namespace Informes.Controlador
{
    /// <summary>
    /// Clase que contiene métodos y propiedades comunes a toda la aplicación. Es estática, para que los recursos sean comunes y accesibles en todo momento.
    /// </summary>
    public class ControladorComun
    {
        public static Datos.BDMongo? CurrentBD { get; set; }
        public const String NombreBD = "TPVJMCP";
        public const String UsuarioBD = "josemankapi";
        public const String PasswordBD = "nonotiene-20";
        public static UsuarioInformes? UsuarioActual { get; set; }
        public static List<TiendaInformes>? Tiendas { get; set; }
        public static TiendaInformes? TiendaActual { get; set; }
        public static AppShell? ShellInicial { get; set; }

        public static void SalirApp()
        {
            Application.Current!.Quit();
        }

        public static List<UsuarioInformes> CargarUsuariosNoAdminTienda()
        {
            List<UsuarioInformes> lista = CurrentBD!.BuscarObjetosInt<UsuarioInformes>("Perfil", 2);
            lista.RemoveAll(usuario => usuario.Tienda.CodTienda != TiendaActual!.CodTienda); //Un for each y un for no invertido da problemas al borrar mientras se itera
            return lista;
        }

        public static bool BorrarUsuarioInformes(UsuarioInformes usuario)
        {
            CurrentBD!.EliminarObjeto<UsuarioInformes>(usuario);
            if (CurrentBD!.BuscarObjeto<UsuarioInformes>(usuario, "Id").Count > 0)
            {
                return false;
            }
            return true;
        }

        public static bool HacerAdminUsuarioInformes(UsuarioInformes usuario)
        {
            usuario.Perfil = 1;
            CurrentBD!.ActualizarObjeto<UsuarioInformes>(usuario);
            if (CurrentBD!.BuscarObjeto<UsuarioInformes>(usuario, "Id").FirstOrDefault<UsuarioInformes>()!.Perfil != 1)
            {
                return false;
            }
            return true;
        }

        public static List<UsuarioInformes> ListaUsuarios()
        {
            return CurrentBD!.LeerObjetosTipo<UsuarioInformes>();
        }

        public static bool CargarTiendas()
        {
            if (CurrentBD!.ContarObjetos<TiendaInformes>() < 1)
            {
                TiendaInformes nuevaTienda = new TiendaInformes(0, 0, "Tienda de sushi de JMCP", "JMCP Sushi\nNIF:99999990T", "Recursos/Imagenes/ISOLOGOJMCP64.jpg");
                Tiendas = new List<TiendaInformes> { nuevaTienda };
                if (!CurrentBD!.PersistirObjeto<TiendaInformes>(nuevaTienda)) return false;
                if (Tiendas[0] == null) return false;
            }
            else
            {
                Tiendas = CurrentBD!.LeerObjetosTipo<TiendaInformes>();
                if (Tiendas.Count < 1) return false;
            }
            TiendaActual = Tiendas.FirstOrDefault(t => t.CodTienda == 0);
            return true;
        }

        public static List<TiendaInformes> ListaTiendas()
        {
            return CurrentBD!.LeerObjetosTipo<TiendaInformes>();
        }

        public static UsuarioInformes ObtenerUsuario(string Nombre, string Contrasena)
        {
            return CurrentBD!.BuscarObjetosStringAndString<UsuarioInformes>("Nombre", Nombre, "Password", Contrasena).FirstOrDefault<UsuarioInformes>()!;
        }
        public static TiendaInformes ObtenerTienda(int CodigoTienda)
        {
            return CurrentBD!.BuscarObjetosInt<TiendaInformes>("CodTienda", CodigoTienda).FirstOrDefault()!;
        }
    }
}
