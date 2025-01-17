namespace Informes
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            UserAppTheme = AppTheme.Dark; //Pongo el tema oscuro de MAUI y genero los temas visuales combinándolos con él

            MainPage = new AppShell();
        }
    }
}
