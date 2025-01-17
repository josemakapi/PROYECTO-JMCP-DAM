namespace Informes.Vista;
using Informes.Modelo;
using Informes.Datos;
using System.Diagnostics;
using Informes.Controlador;
using Android.App;
using AndroidX.AppCompat.App;

public partial class AdminPage : ContentPage
{
	public AdminPage()
	{
		InitializeComponent();
        CargaListaUsuariosPicker();
	}

    /// <summary>
    /// Como indica su nombre, carga y recarga la lista de usuarios para el picker que lo muestra en pantalla.
    /// </summary>
    private void CargaListaUsuariosPicker()
    {
        pkUser.SelectedItem = null;
        pkUser.ItemsSource = ControladorComun.CargarUsuariosNoAdmin();
        if (pkUser.ItemsSource.Count == 0)
        {
            pkUser.IsEnabled = false;
        }
        else
        {
            pkUser.SelectedIndex = 0;
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        ControladorComun.SalirApp();
    }

    /// <summary>
    /// Controla el comportamiento del botón de borrado de usuarios no administradores. Usa un control asíncrono para preguntar al usuario y no bloquear la aplicación
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnBorraUser_Clicked(object sender, EventArgs e)
    {
        if (pkUser.SelectedItem!=null) {
            bool respuesta;
            respuesta = await DisplayAlert("Advertencia", "¿Confirmas su borrado?", "Si", "No");
            //Debug.WriteLine("Respuesta: ");
            if (respuesta)
            {

                if (ControladorComun.BorrarUsuarioInformes((UsuarioInformes)pkUser.SelectedItem))
                {
                    await DisplayAlert("Info", "Usuario borrado correctamente", "OK");
                    this.CargaListaUsuariosPicker();
                }
                else 
                { 
                    await DisplayAlert("Advertencia", "Error al borrar usuario", "OK");
                }
            }
            this.CargaListaUsuariosPicker();
        }
        else
        {
            await DisplayAlert("Advertencia","Debes tener seleccionado algún usuario","OK");
        }
    }

    /// <summary>
    /// Controla el comportamiento del botón de elevación de usuarios no administradores. Usa un control asíncrono para preguntar al usuario y no bloquear la aplicación
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnElevaUser_Clicked(object sender, EventArgs e)
    {
        if (pkUser.SelectedItem != null)
        {
            bool respuesta;
            respuesta = await DisplayAlert("Advertencia", "Al hacer administrador a un usuario ya no podrá ser eliminado.\n¿Confirmas hacer administrador a "+pkUser.SelectedItem+"?", "Si", "No");
            //Debug.WriteLine("Respuesta: ");
            if (respuesta)
            {

                if (ControladorComun.HacerAdminUsuarioInformes((UsuarioInformes)pkUser.SelectedItem))
                {
                    await DisplayAlert("Info", pkUser.SelectedItem+" ya es administrador", "OK");
                    this.CargaListaUsuariosPicker();
                }
                else
                {
                    await DisplayAlert("Advertencia", "Error al intentar hacer administrador al usuario", "OK");
                }
                this.CargaListaUsuariosPicker();
            }
        }
        else
        {
            await DisplayAlert("Advertencia", "Debes tener seleccionado algún usuario", "OK");
        }
    }

    private void pkUser_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (pkUser.ItemsSource.Count == 0)
        {
            DisplayAlert("Advertencia", "No hay ningún usuario limitado en esta tienda", "OK");
            pkUser.IsEnabled = false;
        }
    }
}