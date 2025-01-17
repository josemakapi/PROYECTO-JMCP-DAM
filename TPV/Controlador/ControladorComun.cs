﻿using System.Windows;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using TPV.Controlador;
using TPV.Datos;
using TPV.Modelo;
using TPV.Vista;

namespace TPV.Controlador
{
    /// <summary>
    /// Superclase estática que controla el inicio y fin del programa y tiene recursos comunes a todos los elementos 
    /// del programa para evitar crear instancias innecesarias.
    /// </summary>
    public static class ControladorComun
    {
        public static String NombreBD = "TPVJMCP";
        public static TPVBase? TpvBase;
        public static BDMongo? BD;
        public static bool debugMode = true;
        public static Tienda? TiendaActual;
        public static List<Tienda>? Tiendas;
        public static byte[] Iv = new byte[16] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
        public static byte[] ClaveCompartida = new byte[]
        {
            0xA5, 0xB9, 0xC1, 0x3D, 0x4F, 0x2A, 0x1B, 0xE6,
            0x9F, 0x0A, 0x47, 0x81, 0xD3, 0xE5, 0xC4, 0x72,
            0x3B, 0xF8, 0x52, 0xD7, 0xE1, 0x91, 0x98, 0x63,
            0xAC, 0x6B, 0xE3, 0xA9, 0x45, 0xCD, 0xF1, 0x8E
        };

        public static bool IniciarBD(string host, int puerto, string user, string pass)
        {
            BD = new BDMongo(host, puerto, user, pass);
            if (!BD.ConectarBDDirecta(NombreBD))
            {
                return false;
            }
            return true;
        }

        public static void AplicarFondoBorroso(Window ventana)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != ventana)
                {
                    window.Effect = new BlurEffect { Radius = 30 };
                }
            }
        }

        public static void QuitarFondoBorroso()
        {
            foreach (Window window in Application.Current.Windows)
            {
                window.Effect = null;
            }
        }

        public static bool IniciarBD(string user, string pass)
        {
            BD = new BDMongo(user, pass);
            if (!BD.ConectarBDCloud(NombreBD))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Llamado desde el botón Conectar de la ventana de PreInicio para inicializar los parámetros de la tienda
        /// </summary>
        public static void PreInicializaTienda()
        {
            CargarTiendas();
        }

        public static bool CargarPantallaVentas()
        {
            TpvBase = new TPVBase();
            TpvBase.InicioTPV();
            return true;
        }
        public static void CargarTiendas()
        {
            if (BD!.ContarObjetos<Tienda>() < 1)
            {
                Tiendas = new List<Tienda> { new Tienda(0, 0, 1, "192.168.1.200", "Tienda de sushi de JMCP", "JMCP Sushi\nNIF:99999990T", $"pack://application:,,,/Recursos/Imagenes/ISOLOGOJMCP64.jpg") };
                BD!.PersistirObjeto(Tiendas[0]);
            }
            else
            {
                Tiendas = BD!.LeerObjetosTipo<Tienda>();
            }
        }

        public static void CerrarPrograma()
        {
            if (TpvBase != null)
            {
                TpvBase!.FinTPV();
                BD!.DesconectarBD();
                Application.Current.Shutdown();
            }
        }

        public static BitmapImage LeerImagen(string nombreImagenProyecto)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(nombreImagenProyecto);
            bitmap.EndInit();
            return bitmap;
        }
        public static BitmapImage LeerImagen(Uri Uri)
        { 
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = Uri;
            bitmap.EndInit();
            return bitmap;
        }

        public static BitmapImage LeerImagenProducto(string nombreImagenProyecto)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri($"pack://application:,,,/Recursos/Imagenes/Productos/{nombreImagenProyecto}");
            bitmap.EndInit();
            return bitmap;
        }

        public static void CambiarTema(string nombreTema)
        {
            var nuevoTema = new ResourceDictionary();
            nuevoTema.Source = new Uri($"Recursos/Temas/{nombreTema}.xaml", UriKind.Relative);
            nuevoTema = Application.LoadComponent(nuevoTema.Source) as ResourceDictionary;
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(nuevoTema);
            foreach (Window ventana in Application.Current.Windows)
            {
                ventana.Resources.MergedDictionaries.Clear();
                ventana.Resources.MergedDictionaries.Add(nuevoTema);

                // Forzamos la actualización del estilo de los controles

                if (ventana is VentanaPrincipal)
                {
                    var ventPrincipal = ventana as VentanaPrincipal;
                    ventPrincipal!.ActualizaInfoUsuario();
                }
                ventana.InvalidateVisual();
                ventana.UpdateLayout();
            }
        }

    }
}
