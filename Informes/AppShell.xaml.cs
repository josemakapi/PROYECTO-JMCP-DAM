using Informes.Controlador;
using Informes.Datos;
using Informes.Vista;
using Informes.Modelo;
using MongoDB.Driver.Linq;
namespace Informes
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            ControladorComun.ShellInicial = this;
            ControladorComun.CurrentBD = new BDMongo(ControladorComun.UsuarioBD, ControladorComun.PasswordBD);
            if (ControladorComun.CurrentBD.ConectarBDCloud(ControladorComun.NombreBD))
            {

                if (!ControladorComun.CargarTiendas())
                {
                    DisplayAlert("Error", "No existe ninguna tienda", "OK");
                    return;
                }
                if (ControladorComun.CurrentBD.ContarObjetos<UsuarioInformes>() == 0)
                {
                    ControladorComun.CurrentBD.PersistirObjeto<UsuarioInformes>(new UsuarioInformes("admin", 1, "admin", ControladorComun.TiendaActual, "ES", "TemaOriginal", "Medio"));
                }
                loginSC.Icon = "about.png";
                loginSC.Content = new PLogin();
            }
            else
            {
                DisplayAlert("Error", "No se ha podido conectar a la base de datos", "OK");
                ControladorComun.SalirApp();
            }
        }


        public void SacarMenuFlyout()
        {
            this.shellEstructuraNavegacion.FlyoutBehavior = FlyoutBehavior.Flyout;
        }
        public void OcultarMenuFlyout()
        {
            this.shellEstructuraNavegacion.FlyoutBehavior = FlyoutBehavior.Disabled;
        }
        public void SetConectado()
        {
            SacarMenuFlyout();
            CreaLogout();
            loginSC.IsVisible = false;
            lblName.Text = ControladorComun.UsuarioActual!.Nombre;
            if (ControladorComun.UsuarioActual.Perfil < 2)
            {
                TabAdmin.IsVisible = true;
                footAdmin.IsVisible = true;
            }
            else
            {
                TabAdmin.IsVisible = false;
                footAdmin.IsVisible = false;
            }
        }

        public void SetNoConectado()
        {
            OcultarMenuFlyout();
            DestruyeLogout();
            loginSC.IsVisible = true;
            //Establecemos los estilos predeterminados justo antes de desconectar la BD:
            if (ControladorComun.UsuarioActual != null)
            {
                ControladorComun.UsuarioActual.ActualizaEstilos(false);
            }
            ControladorComun.UsuarioActual = null;
            LogoutShell();
        }
        public void LogoutShell()
        {
            Application.Current!.MainPage = new AppShell();
        }

        public void CreaLogout()
        {
            MenuItem logoutSC = new MenuItem();
            logoutSC.Text = "Log Out";
            logoutSC.IconImageSource = "logout_app.png";
            logoutSC.Clicked += LogoutSC_Clicked;
            this.shellEstructuraNavegacion.Items.Add(logoutSC);
            // Hay que ver la de filigranas que hay que hacer en MAUI para que el botón de salir
            // esté en la parte inferior de la pantalla en orden:
            this.shellEstructuraNavegacion.Items.Remove(this.Items.Where(x => x.AutomationId == "SalirYA").FirstOrDefault());
            this.shellEstructuraNavegacion.Items.Add(SalirItemSC);
        }

        private void LogoutSC_Clicked(object? sender, EventArgs e)
        {
            SetNoConectado();
            loginSC.Content = new PLogin();
        }

        private async void SalirItem_Clicked(object? sender, EventArgs e)
        {
            bool salir = await DisplayAlert("Salir", "¿Estás seguro de que deseas salir?", "Sí", "No");
            if (salir)
            {
                ControladorComun.SalirApp();
            }
        }
        public void DestruyeLogout()
        {
            this.shellEstructuraNavegacion.Items.RemoveAt(shellEstructuraNavegacion.Items.Count - 1);
        }

        private void itemSobre_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PSobre());
        }

        private void itemAyuda_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new HelpPage(this));
        }
    }
}
