using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TPV.Controlador;
using TPV.Modelo;

namespace TPV.Vista
{
    /// <summary>
    /// Lógica de interacción para VentanaPrincipal.xaml
    /// </summary>
    public partial class VentanaPrincipal : Window
    {
        private double _totalGrid;
        public VentanaPrincipal()
        {
            InitializeComponent();
            this.dGridPosVentaMain.CellEditEnding += dGridPosVentaMain_CellEditEnding;
            this.dGridPosVentaMain.CurrentCellChanged += dGridPosVentaMain_CurrentCellChanged;
            try
            {
                this.lblSumaGrid.DataContext = this;
                this._totalGrid = 0;
                this.GenerarTabItems(ControladorComun.TpvBase!.Secciones!);
                tab1M.Visibility = Visibility.Collapsed;
                gridProductos.Visibility = Visibility.Collapsed;
                this.DataContext = ControladorComun.TpvBase;
                tabControl.SelectedItem = tabControl.Items[1]; //Porque el idx0 es la muestra estática
                actualizaGridPrecios();
            }catch(IOException e)
            {
                MessageBox.Show("Error al cargar las imágenes de los productos: " + e.Message);
            }
        }
        private void dGridPosVentaMain_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            this.sincronizaLineasPantallaConGrid();
        }      

        private void dGridPosVentaMain_CurrentCellChanged(object sender, EventArgs e)
        {
            this.sincronizaLineasPantallaConGrid();
        }

        private void sincronizaLineasPantallaConGrid()
        {
            this._totalGrid = 0;
            ControladorComun.TpvBase!.PosicionVentaActual!.LineasPantalla = new List<Linea>();
            foreach (Linea linea in this.dGridPosVentaMain.Items)
            {
                ControladorComun.TpvBase!.PosicionVentaActual!.LineasPantalla.Add(linea);
                this._totalGrid += linea.Precio;
            }
            this.lblSumaGrid.Content = $"{this._totalGrid.ToString("0.00")} €";
        }

        private void actualizaGridPrecios()
        {
            this.dGridPosVentaMain.ItemsSource = ControladorComun.TpvBase!.PosicionVentaActual!.LineasPantalla;
            this.dGridPosVentaMain.Items.Refresh();
            this._totalGrid = 0;
            foreach (Linea linea in ControladorComun.TpvBase!.PosicionVentaActual!.LineasPantalla)
            {
                this._totalGrid += linea.Precio;
            }
            this.lblSumaGrid.Content = $"{this._totalGrid.ToString("0.00")} €";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ControladorComun.CerrarPrograma();
        }

        private void btnCobrar_Click(object sender, RoutedEventArgs e)
        {
            if ((ControladorComun.TpvBase!.PosicionVentaActual == null) || ControladorComun.TpvBase!.PosicionVentaActual.LineasPantalla.Count < 1)
            {
                MessageBox.Show("No hay productos en la venta", "Error");
            }
            else 
            { 
                new VentanaTicketPantalla().ShowDialog();
                this.ActualizaInfoUsuario();
                this.actualizaGridPrecios();
            }
        }

        private void btnFuncEnc_Click(object sender, RoutedEventArgs e)
        {
            new VentanaEncargado().ShowDialog();
            this.ActualizaInfoUsuario();
        }

        public void ActualizaInfoUsuario()
        {
            this.lblUsuario.Content = ControladorComun.TpvBase!.UsuarioActual!.Nombre;
            this.imgAvatar.Source = ControladorComun.TpvBase!.UsuarioActual!.ObtenerAvatar();
            this.lblTarifa.Content = ControladorComun.TpvBase!.TarifaActual!.Nombre;

            if (ControladorComun.TpvBase!.UsuarioActual!.EsEncargado)
            {
                Grid.SetColumnSpan(btnTarifa, 1);
                this.btnTarifa.Width = 128;
                this.btnFuncEnc.Visibility = Visibility.Visible;
                this.lblUsuario.Foreground = Brushes.Red;
                this.dGridPosVentaMain.IsReadOnly = false; //El encargado puede modificar productos y precios
            }
            else
            {
                this.btnFuncEnc.Visibility = Visibility.Collapsed;
                this.btnTarifa.Width = 290;
                Grid.SetColumnSpan(btnTarifa, 2); //Cambio en el ancho del botón si no es encargado
                this.dGridPosVentaMain.IsReadOnly = true; //El vendedor no puede modificar productos ni precios
                if (ControladorComun.TpvBase!.UsuarioActual!.TemaVisual == "TemaOriginal")
                {
                    this.lblUsuario.Foreground = Brushes.Black;
                }
                else if (ControladorComun.TpvBase!.UsuarioActual!.TemaVisual == "TemaAC")
                {
                    this.lblUsuario.Foreground = Brushes.White;
                }
            }
        }

        private void imgExit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ControladorComun.TpvBase!.PosicionVentaActual!.LineasPantalla.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Tiene productos sin vender que se perderán como lágrimas en la lluvia ¿Realmente quiere salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    ControladorComun.CerrarPrograma();
                }
            }
            else
            {
                ControladorComun.CerrarPrograma();
            }
        }

        private void imgAvatar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ControladorComun.TpvBase!.BloqueaTPV();
            ActualizaInfoUsuario();
        }

        private void btnTarifa_Click(object sender, RoutedEventArgs e)
        {
            new VentanaTarifa().ShowDialog();
            ActualizaInfoUsuario();
        }

        private void btnAcciones_Click(object sender, RoutedEventArgs e)
        {
            VentanaAcciones ventanaAcciones = new VentanaAcciones();
            this.Show();
            ventanaAcciones.ShowDialog();
        }
        
        public void GenerarTabItems(List<Seccion> secciones)
        {
            foreach (Seccion seccion in secciones)
            {
                TabItem tabItem = new TabItem();
                tabItem.Header = seccion.Nombre;

                Grid gridProductos = new Grid();
                gridProductos.Background = Brushes.White;
                gridProductos.VerticalAlignment = VerticalAlignment.Center;
                gridProductos.HorizontalAlignment = HorizontalAlignment.Center;
                gridProductos.Width = 693;
                gridProductos.Height = 692;

                int rowCount = (seccion.Productos!.Count + 6) / 7; // Hallar el nº de filas
                for (int i = 0; i < rowCount; i++)
                {
                    gridProductos.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(98) });
                }

                int columnCount = Math.Min(seccion.Productos.Count, 7); // Máximo de 7 columnas
                for (int i = 0; i < columnCount; i++)
                {
                    gridProductos.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(98) });
                }

                int productIndex = 0;
                for (int row = 0; row < rowCount; row++)
                {
                    for (int col = 0; col < columnCount; col++)
                    {
                        //if (productIndex < seccion.Productos.Count)
                        if (productIndex < seccion.Productos.Count)
                        {

                            Producto producto = seccion.Productos[productIndex];

                            Image image = new Image();
                            image.Source = producto.Imagen();
                            image.Width = 98;
                            image.Height = 98;
                            Grid.SetRow(image, row);
                            Grid.SetColumn(image, col);
                            image.ToolTip = producto.Descripcion; 
                            AutomationProperties.SetName(image, producto.Nombre); //Para el lector en pantalla de accesibilidad
                            image.MouseLeftButtonDown += (sender, e) =>
                            { // Evento al hacer click en la imagen. Se define aquí porque se generan dinámicamente 
                                ControladorComun.TpvBase!.InsertarLineaVenta(producto);
                                //this._totalGrid += ControladorComun.TpvBase!.TarifaActual!.ListaProductos.Find(x => x.CodProducto == producto.CodProducto)!.Precio;
                                //this.lblSumaGrid.Content = this._totalGrid.ToString("0.00") + " €";
                                this.actualizaGridPrecios();
                        };
                        gridProductos.Children.Add(image);
                        productIndex++;
                        }
                    }
                }
                tabItem.Content = gridProductos;
                tabControl.Items.Add(tabItem);
            }
        }

        private void btnBorrar_Click(object sender, RoutedEventArgs e)
        {
            if (this.dGridPosVentaMain.Items.Count > 0)
            {
                var productosGrid = (IList<Linea>)dGridPosVentaMain.ItemsSource;
                if (this.dGridPosVentaMain.SelectedItems.Count < 1)
                {
                    productosGrid.RemoveAt(this.dGridPosVentaMain.Items.Count - 1);
                }
                else
                {
                    foreach (Linea linea in this.dGridPosVentaMain.SelectedItems)
                    {
                        productosGrid.Remove(linea);
                    }
                }
                this.actualizaGridPrecios();
            }
        }

        private void M_Item0_Click(object sender, RoutedEventArgs e)
        {            
            Process.Start(new ProcessStartInfo("calc.exe") { UseShellExecute = true });
        }

        private void M_Item1_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("notepad.exe") { UseShellExecute = true });
        }

        private void M_Item2_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("osk.exe") { UseShellExecute = true });
        }

        private void dGridPosVentaMain_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            this.sincronizaLineasPantallaConGrid();
        }
    }
}
