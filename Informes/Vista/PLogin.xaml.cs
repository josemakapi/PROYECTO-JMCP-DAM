using Informes.Controlador;
using Informes.Modelo;
namespace Informes.Vista;

public partial class PLogin : ContentPage
{
    private bool isAcTheme = false;

    private AppShell receivedAppShell;
    public PLogin() //Ojo con los contructores, si no se pone el de por defecto, en Android no funciona 
    {
        InitializeComponent();
        this.receivedAppShell = ControladorComun.ShellInicial!;
    }
    public PLogin(AppShell miAppShell)
    {
        InitializeComponent();
        this.receivedAppShell = miAppShell;
    }

    private void btnMostrar_Pressed(object sender, EventArgs e)
    {
        txtPass.IsPassword = false;
        btnMostrar.ImageSource = "ojocerrado32.png";
        ToolTipProperties.SetText(btnMostrar, "Ocultar contrase�a");
    }

    private void btnMostrar_Released(object sender, EventArgs e)
    {
        txtPass.IsPassword = true;
        btnMostrar.ImageSource = "ojoabierto32.png";
        ToolTipProperties.SetText(btnMostrar, "Mostrar contrase�a");
    }

    /// <summary>
    /// Desencadena la conexi�n a la BD y la inicializaci�n de los objetos est�ticos: 
    /// la BD y las propiedades del usuario, para ser utilizados de forma centralizada 
    /// en todo el programa. Este objeto usuario est�tico ser� accesible a trav�s de SharedObjects.CurrentBD.LoggedUserProps.
    /// Se cargan y aplican los estilos y configuraciones que tuviera configurados en la BD.
    /// Tambi�n se comprueba si hay usuarios en la BD. Si no hay, se lanza la bienvenida.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnConectar_Clicked(object sender, EventArgs e)
    {
        //ControladorComun.CurrentBD!.ConectarBDCloud(ControladorComun.NombreBD);

        if (ControladorComun.TiendaActual == null)
        {
            DisplayAlert("Error", "No existe ninguna tienda", "OK");
            return;
        }

        if (ControladorComun.CurrentBD!.ContarObjetos<UsuarioInformes>() < 1)
        {
            ControladorComun.CurrentBD.PersistirObjeto<UsuarioInformes>(new UsuarioInformes("admin", 1, "admin", ControladorComun.TiendaActual, "ES", "TemaOriginal", "Medio"));
            DisplayAlert("Error", "No existe ning�n usuario", "OK");
        }
        else
        {
            if (ControladorComun.CurrentBD.CheckUsuarioInformes(txtUser.Text, txtPass.Text))
            {
                ControladorComun.UsuarioActual = ControladorComun.ObtenerUsuario(txtUser.Text, txtPass.Text);
                this.receivedAppShell.SetConectado();
                ControladorComun.UsuarioActual!.ActualizaEstilos(true);
            }
            else
            {
                DisplayAlert("Error", "Usuario/contrase�a inv�lidos", "OK");
            }
        }
    }

    private void btnCancelar_Clicked(object sender, EventArgs e)
    {
        ControladorComun.SalirApp();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        ICollection<ResourceDictionary> misDiccionarios = Application.Current!.Resources.MergedDictionaries;
        if (!this.isAcTheme)
        {
            string action = await DisplayActionSheet("Opciones", "Cancelar", null, "Cambiar a tema Alto Contraste");
            if (action == "Cambiar a tema Alto Contraste")
            {
                misDiccionarios.Clear();
                misDiccionarios.Add(new Informes.Recursos.Themes.TemaAltoContraste());
                misDiccionarios.Add(new Informes.Recursos.Strings.Castellano());
                misDiccionarios.Add(new Informes.Recursos.Styles.SmallFontsMode());
                //this.Contextual.Text = "Cambiar a tema Original";
                isAcTheme = true;
            }
        }
        else
        {
            string action = await DisplayActionSheet(
                "Opciones",               // T�tulo
                "Cancelar",               // Bot�n de cancelar
                null,                     // Bot�n de destrucci�n (opcional)
                "Cambiar a tema Original");  // Opciones del men�

            if (action == "Cambiar a tema Original")
            {
                misDiccionarios.Clear();
                misDiccionarios.Add(new Informes.Recursos.Themes.TemaOriginal());
                misDiccionarios.Add(new Informes.Recursos.Strings.Castellano());
                misDiccionarios.Add(new Informes.Recursos.Styles.SmallFontsMode());
                //this.Contextual.Text = "Cambiar a tema Alto Contraste";
                isAcTheme = false;
            }
        }
    }

    private void btnAltaUsuario_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new RegisterPage());
    }
}