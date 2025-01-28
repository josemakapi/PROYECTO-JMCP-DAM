namespace Informes.Vista;

using Informes.Controlador;
using Informes.Datos;
using Informes.Modelo;

/// <summary>
/// Página desde la que podemos registrar nuevos usuarios
/// </summary>
public partial class RegisterPage : ContentPage
{
    private List<TiendaInformes>? listaTiendas;
    public RegisterPage() 
    {
        InitializeComponent();
        BindingContext = this;
        IniciaListas();
    }

    private void IniciaListas()
    {
        listaTiendas = new List<TiendaInformes>();
        listaTiendas = ControladorComun.CurrentBD!.LeerObjetosTipo<TiendaInformes>();
        pkTienda.ItemsSource = listaTiendas;
        pkTienda.SelectedIndex = 0;
        pkIdioma.SelectedIndex = 0;
        pkLetra.SelectedIndex = 1;
        pkTema.SelectedIndex = 0;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        ControladorComun.SalirApp();
    }

    private void btnMostrarReg_Pressed(object sender, EventArgs e)
    {
        entPassword.IsPassword = false;
        btnMostrarReg.ImageSource = "ojocerrado32.png";
        ToolTipProperties.SetText(btnMostrarReg, "Ocultar contraseña");
    }

    private void btnMostrarReg_Released(object sender, EventArgs e)
    {
        entPassword.IsPassword = true;
        btnMostrarReg.ImageSource = "ojoabierto32.png";
        ToolTipProperties.SetText(btnMostrarReg, "Mostrar contraseña");
    }

    /// <summary>
    /// Al hacer click en el botón registrar el método comprueba que se haya escrito algo en los entrys de Usuario y Contraseña y que ese usuario no existiese previamente.
    /// De ser así, se da de alta en la BD y lleva a la página de Login
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRegister_Clicked(object sender, EventArgs e)
    {
        if (pkTienda.SelectedItem == null)
        {
            DisplayAlert("Error", "Debes seleccionar una tienda", "OK");
            return;
        }
        if (entUserName.Text == null || entPassword.Text == null || entUserName.Text.Length < 1 || entPassword.Text.Length < 1)
        {
            DisplayAlert("Error", "Debes escribir usuario y contraseña", "OK");
            return;
        }
        else
        {
            if (ControladorComun.CurrentBD!.BuscarObjetosString<UsuarioInformes>("Nombre", (string)entUserName.Text).Count < 1)
            {

                string visualTheme = (string)pkTema.SelectedItem;
                if (visualTheme == "Original (recomendado)")
                {
                    visualTheme = "TemaOriginal";
                }
                else if (visualTheme == "Alto contraste")
                {
                    visualTheme = "TemaAltoContraste";
                }

                string idioma = (string)pkIdioma.SelectedItem;
                if (idioma == "Castellano")
                {
                    idioma = "ES";
                }
                else if (idioma == "English")
                {
                    idioma = "EN";
                }

                string tamFuente = (string)pkLetra.SelectedItem;
                if (tamFuente == "Grande")
                {
                    tamFuente = "big";
                }
                else if (tamFuente == "Pequeña")
                {
                    tamFuente = "little";
                }

                try
                {
                    UsuarioInformes nuevoUsuario = new UsuarioInformes(entUserName.Text, 2 ,entPassword.Text, (TiendaInformes)pkTienda.SelectedItem, idioma, visualTheme, tamFuente);
                    ControladorComun.CurrentBD.PersistirObjeto<UsuarioInformes>(nuevoUsuario);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    DisplayAlert("Error", "Error al dar de alta el usuario en la BD", "OK");
                }
                DisplayAlert("Bienvenido", "¡Bienvenid@, "+entUserName.Text+"!\nEsperamos que la app cumpla tus espectativas. Te hemos llevado a la página de Login para que puedas comenzar desde ya", "OK");
                Navigation.PopAsync();
            }
            else
            {
                DisplayAlert("Error", "Ese usuario ya existe, dime otro nuevo", "OK");
            }
        }
        entUserName.Text = string.Empty;
        entPassword.Text = string.Empty;
    }
}