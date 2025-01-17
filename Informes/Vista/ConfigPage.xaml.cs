namespace Informes.Vista;

using Informes.Controlador;
using Informes.Modelo;

/// <summary>
/// Página para gestionar los ajustes del usuario
/// </summary>
public partial class ConfigPage : ContentPage
{
    private List<string> listaIdiomas;
    private List<string> listaVisuales;
    private List<string> listaTamLetra;

    /// <summary>
    /// Inicializamos las listas de los elementos visuales y provincias, así como los métodos de los picker asociados
    /// </summary>
    public ConfigPage()
	{
        InitializeComponent();
        BindingContext = this;

        IniciaListaIdiomas();
        IniciaListaVisuales();
        IniciaListaTamLetra();

        pkIdiomaCfg.SelectedIndexChanged += PkIdioma_SelectedIndexChanged;
        pkTemaCfg.SelectedIndexChanged += PkTemaCfg_SelectedIndexChanged;
        pkTamLetra.SelectedIndexChanged += PkTamLetra_SelectedIndexChanged;
    }

    /// <summary>
    /// Rellenamos los pickers con las listas teniendo en cuenta los diccionarios cargados y establecemos la nueva configuración seleccionada por el usuario, haciendo persistencia en la BD.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PkTamLetra_SelectedIndexChanged(object? sender, EventArgs e)
    {
        string selectedElement = (string)pkTamLetra.SelectedItem;
        if (selectedElement.Equals("Grande") || selectedElement.Equals("Big"))
        {
            ControladorComun.UsuarioActual!.TamFuente = "big";
        }
        else if (selectedElement.Equals("Pequeña") || selectedElement.Equals("Small"))
        {
            ControladorComun.UsuarioActual!.TamFuente = "little";
        }
        ControladorComun.CurrentBD!.ActualizarObjeto<UsuarioInformes>(ControladorComun.UsuarioActual!);
        ControladorComun.UsuarioActual!.ActualizaEstilos(true);
    }

    /// <summary>
    /// Rellenamos los pickers con las listas teniendo en cuenta los diccionarios cargados y establecemos la nueva configuración seleccionada por el usuario, haciendo persistencia en la BD.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PkTemaCfg_SelectedIndexChanged(object? sender, EventArgs e)
    {
        string selectedElement = (string)pkTemaCfg.SelectedItem;
        if (selectedElement.Equals("Original (recomendado)") || selectedElement.Equals("Original (recommended)"))
        {
            ControladorComun.UsuarioActual!.VisualTheme = "TemaOriginal";
        }
        else if (selectedElement.Equals("Alto contraste") || selectedElement.Equals("High contrast"))
        {
            ControladorComun.UsuarioActual!.VisualTheme = "TemaAltoContraste";
        }
        ControladorComun.CurrentBD!.ActualizarObjeto<UsuarioInformes>(ControladorComun.UsuarioActual!);
        ControladorComun.UsuarioActual!.ActualizaEstilos(true);
    }

    /// <summary>
    /// Rellenamos los pickers con las listas teniendo en cuenta los diccionarios cargados y establecemos la nueva configuración seleccionada por el usuario, haciendo persistencia en la BD.
    /// Para que se traduzca el propio picker ha sido necesario desapilar la página de ajustes y reabrirla, 
    /// pues no accedía al diccionario de idiomas a tiempo para rellenar el picker y daba error de objeto nulo
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PkIdioma_SelectedIndexChanged(object? sender, EventArgs e)
    {
        string selectedElement = (string)pkIdiomaCfg.SelectedItem;
        if (selectedElement.Equals("English") || selectedElement.Equals("Inglés"))
        {
            DisplayAlert("Advertencia", "Tu nuevo idioma es " + selectedElement + ". Ahora el contenido se mostrará en " + selectedElement, "Vale");
            ControladorComun.UsuarioActual!.Idioma = "EN";
        }
        else if (selectedElement.Equals("Castellano") || selectedElement.Equals("Castillian"))
        {
            DisplayAlert("Warning", "Your current language will be " + selectedElement + ". The whole content will be in " + selectedElement + " language", "OK");
            ControladorComun.UsuarioActual!.Idioma = "ES";
        }
        ControladorComun.CurrentBD!.ActualizarObjeto<UsuarioInformes>(ControladorComun.UsuarioActual!);
        ControladorComun.UsuarioActual!.ActualizaEstilos(true);

        //Como los picker no se actualizan automáticamewnte al cambiar el idioma y tampoco recargando las listas, recargamos la ContentPage
        ConfigPage currentPage = this;
        ConfigPage newPage = new ConfigPage();
        Navigation.RemovePage(currentPage);
        Navigation.PushAsync(newPage);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        ControladorComun.SalirApp();
    }

    /// <summary>
    /// Inicia la lista de idiomas teniendo en cuenta los diccionarios que tengamos cargados en el momento
    /// </summary>
    private void IniciaListaIdiomas()
    {
        listaIdiomas = new List<string>();
        listaIdiomas.Add((string)App.Current!.Resources["idiomaCastellano"]);
        listaIdiomas.Add((string)App.Current!.Resources["idiomaEnglish"]);
        pkIdiomaCfg.ItemsSource = listaIdiomas;
        if (ControladorComun.UsuarioActual!.Idioma.Equals("ES")) pkIdiomaCfg.SelectedItem = (string)App.Current!.Resources["idiomaCastellano"];
        if (ControladorComun.UsuarioActual!.Idioma.Equals("EN")) pkIdiomaCfg.SelectedItem = (string)App.Current!.Resources["idiomaEnglish"];
    }

    /// <summary>
    /// Inicia la lista de temas visuales teniendo en cuenta los diccionarios que tengamos cargados en el momento
    /// </summary>
    private void IniciaListaVisuales()
    {
        listaVisuales = new List<string>();
        listaVisuales.Add((string)App.Current!.Resources["temaOriginalName"]);
        listaVisuales.Add((string)App.Current.Resources["temaACName"]);
        pkTemaCfg.ItemsSource = listaVisuales;
        if (ControladorComun.UsuarioActual!.VisualTheme.Equals("TemaOriginal")) pkTemaCfg.SelectedIndex = 0;
        if (ControladorComun.UsuarioActual!.VisualTheme.Equals("TemaAltoContraste")) pkTemaCfg.SelectedIndex = 1;
    }

    /// <summary>
    /// Inicia la lista de fuentes teniendo en cuenta los diccionarios que tengamos cargados en el momento
    /// </summary>
    private void IniciaListaTamLetra()
    {
        listaTamLetra = new List<string>();
        listaTamLetra.Add((string)App.Current!.Resources["tamLetraBig"]);
        listaTamLetra.Add((string)App.Current!.Resources["tamLetraSmall"]);
        pkTamLetra.ItemsSource = listaTamLetra;
        if (ControladorComun.UsuarioActual!.TamFuente.Equals("big")) pkTamLetra.SelectedIndex = 0;
        if (ControladorComun.UsuarioActual!.TamFuente.Equals("little")) pkTamLetra.SelectedIndex = 1;
    }

}