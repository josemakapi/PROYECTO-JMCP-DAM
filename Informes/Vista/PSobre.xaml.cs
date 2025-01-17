using Informes.Controlador;
using Informes.Modelo;

namespace Informes.Vista;

/// <summary>
/// Página para el about
/// </summary>
public partial class PSobre : ContentPage
{
    public PSobre()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        ControladorComun.SalirApp();
    }

    private void ImagenJMCP_Clicked(object sender, EventArgs e)
    {
        string urlMail = "mailto:josemankapi@hotmail.com";
        Launcher.OpenAsync(new Uri(urlMail));
    }
}