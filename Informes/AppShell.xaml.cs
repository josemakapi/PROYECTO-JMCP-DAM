using Informes.Controlador;
using Informes.Datos;
using Informes.Vista;
using Informes.Modelo;
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

            //ControladorComun.CurrentBD.DesconectaBD();
            //Desinstanciamos el objeto que contiene los datos comunes del usuario logado:
            ControladorComun.UsuarioActual = null;
            //Ahora cerramos las contentPage que hubiera abiertas:

            //while (Navigation.ModalStack.Count > 0) { Navigation.PopModalAsync(); }
            //Navigation.PopToRootAsync();

            //foreach (Page page in Application.Current!.MainPage!.Navigation.NavigationStack)
            //{
            //    Application.Current.MainPage.Navigation.RemovePage(page);
            //}

            //var navigation = Application.Current.MainPage.Navigation;
            //foreach (var page in navigation.NavigationStack.ToList()) { navigation.RemovePage(page); }
            //Shell.Current.GoToAsync("//login");

            /*
            int numProg = Application.Current!.MainPage!.Navigation.ModalStack.Count;

            while (Application.Current!.MainPage!.Navigation.ModalStack.Count > 0)
            {
                Application.Current.MainPage.Navigation.PopModalAsync();
            }
            while (Application.Current.MainPage.Navigation.NavigationStack.Count > 1)
            {
                Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[0]);
            }
            */
            Logout();
            
        }

        public async void Logout()
        {
            /*
            var nuevo = Shell.Current.Items.ToArray();
            for (int i = nuevo.Length - 1; i > 0; i--)
            {
                Shell.Current.Items.Remove(nuevo[i]);
                //Shell.Current.Items.;
            }
            await Navigation.PopToRootAsync();
            */

            
            // Obtiene todas las páginas de la pila de navegación
            var navigationStack = Navigation.NavigationStack.ToList();

            // Almacena las páginas en una variable (opcional)
            //var pages = navigationStack;

            // Cierra todas las páginas excepto la raíz
            for (int i = navigationStack.Count - 1; i > 0; i--)
            {
                //Navigation.RemovePage(navigationStack[i]);
                Navigation.NavigationStack[i].RemoveLogicalChild(navigationStack[i]);
            }

            // Si deseas garantizar que la navegación quede en la raíz
            await Navigation.PopToRootAsync();

            
        }

        public void CreaLogout()
        {
            MenuItem logoutSC = new MenuItem();
            logoutSC.Text = "Log Out";
            logoutSC.IconImageSource = "logout_app.png";
            logoutSC.Clicked += LogoutSC_Clicked;
            this.shellEstructuraNavegacion.Items.Add(logoutSC);
        }

        private void LogoutSC_Clicked(object? sender, EventArgs e)
        {
            SetNoConectado();
            loginSC.Content = new PLogin();
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
