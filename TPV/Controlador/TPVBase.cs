using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using TPV.Modelo;
using TPV.Vista;
namespace TPV.Controlador
{
    /// <summary>
    /// Controlador que maneja las operaciones propias del TPV y su uso
    /// </summary>
    public class TPVBase
    {
        private int _numTPV = 1; //Esto se define a mano desde aquí. Determina el número de TPV inicial si no se ha definido
        public int NumTPV { get => _numTPV; }
        private int _numerosTeclado;
        public int NumerosTeclado { get => _numerosTeclado; set => _numerosTeclado = value; }
        private List<Usuario>? _listaUsuarios;
        public List<Usuario> ListaUsuarios { get => _listaUsuarios!; set => _listaUsuarios = value; }
        private Usuario? _usuarioActual;
        public Usuario? UsuarioActual { get => _usuarioActual; set => _usuarioActual = value; }
        private TPVObj _tpvCFG = null!;
        private TPVMaster _procesoMaster = null!;
        private Tarifa? _tarifaActual;
        public Tarifa? TarifaActual { get => _tarifaActual; set => _tarifaActual = value; }
        private PosicionVenta? _posicionVentaActual;
        public PosicionVenta? PosicionVentaActual { get => _posicionVentaActual; set => _posicionVentaActual = value; }
        private VentanaPrincipal? _ventanaPrincipal;
        private List<Seccion>? _secciones;
        public List<Seccion>? Secciones { get => _secciones; set => _secciones = value; }
        private List<Producto>? _productos;
        private List<Tarifa>? _tarifas;
        private List<FormaPago>? _formasPago;
        public List<FormaPago>? FormasPago { get => _formasPago;}
        public List<Tarifa>? Tarifas { get => _tarifas; set => _tarifas = value; }
        private Tienda? _tiendaActual;
        public Tienda? TiendaActual { get => _tiendaActual; set => _tiendaActual = value; }
        private string ticketFormato = string.Empty;

        /// <summary>
        /// Método que coordina las comprobaciones y operaciones de inicio del TPV
        /// </summary>
        public void InicioTPV()
        {
            CargarCfgTPV();
            CargarProductos();
            CargarTarifas();
            CargarFormasPago();
            CargarSecciones();
            CargarUsuariosPredeterminados();
            ControladorComun.BD!.ActualizarObjeto(ControladorComun.TiendaActual);
            this.TiendaActual = ControladorComun.TiendaActual;
            if (this._tpvCFG.IsTPVMaster)
            {
                Task.Run(() =>
                {
                    _procesoMaster = new TPVMaster();
                    _procesoMaster.Iniciar();
                });
            }

            this.PosicionVentaActual = new PosicionVenta(1,this.TarifaActual!.CodTarifa);
            this._ventanaPrincipal = new VentanaPrincipal();
            BloqueaTPV();
            this._ventanaPrincipal.Show();
        }

        private void CargarCfgTPV()
        {
            if (ControladorComun.BD!.ContarObjetos<TPVObj>() < 1)
            {
                this._tpvCFG = new TPVObj(0, 1, true, 0, 0);
                ControladorComun.BD!.PersistirObjeto(_tpvCFG);
            }
            else
            {
                this._tpvCFG = ControladorComun.BD!.BuscarObjetosIntAndInt<TPVObj>("CodTienda", ControladorComun.TiendaActual!.CodTienda, "NumTPV", _numTPV)[0];
            }
        }

        private void CargarUsuariosPredeterminados()
        {
            if (ControladorComun.BD!.ContarObjetos<Usuario>() < 1) //Si no existe ninguno, creamos dos usuarios de ejemplo para la presentación
            {
                ControladorComun.BD!.PersistirObjeto(new Usuario(0, "7777", 0, true, "Encargado", "pack://application:,,,/Recursos/Imagenes/encargado.png", "TemaOriginal"));
                ControladorComun.BD!.PersistirObjeto(new Usuario(1, "1111", 0, false, "Vendedor", "pack://application:,,,/Recursos/Imagenes/vendedor.png", "TemaAC"));
            }
            this._listaUsuarios = ControladorComun.BD!.BuscarObjetosInt<Usuario>("CodTienda", _tpvCFG.CodTienda).ToList();
        }

        private void CargarTarifas()
        {
            if (ControladorComun.BD!.ContarObjetos<Tarifa>() < 1)
            {
                this._tarifaActual = new Tarifa(0, 0, "Tarifa Base PVP", 0, 0.21, "PVP");
                Tarifa tarifaAlternativa = new Tarifa(1, "Tarifa Promo PVP", 0, 0.21, "PVP Oferta");
                //Sushi
                this._tarifaActual.AnadirProducto(0, 6, true);
                tarifaAlternativa.AnadirProducto(0, 4.5, true);
                this._tarifaActual.AnadirProducto(1, 6.1, true);
                tarifaAlternativa.AnadirProducto(1, 4.6, true);
                this._tarifaActual.AnadirProducto(2, 10, true);
                tarifaAlternativa.AnadirProducto(2, 9, true);
                this._tarifaActual.AnadirProducto(3, 5.8, true);
                tarifaAlternativa.AnadirProducto(3, 4.1, true);
                this._tarifaActual.AnadirProducto(4, 5.5, true);
                tarifaAlternativa.AnadirProducto(4, 4.2, true);
                this._tarifaActual.AnadirProducto(5, 6.2, true);
                tarifaAlternativa.AnadirProducto(5, 4.7, true);
                this._tarifaActual.AnadirProducto(6, 6.5, true);
                tarifaAlternativa.AnadirProducto(6, 4.8, true);
                this._tarifaActual.AnadirProducto(7, 6.3, true);
                tarifaAlternativa.AnadirProducto(7, 4.9, true);
                this._tarifaActual.AnadirProducto(8, 5.5, true);
                tarifaAlternativa.AnadirProducto(8, 4.2, true);
                this._tarifaActual.AnadirProducto(9, 6.4, true);
                tarifaAlternativa.AnadirProducto(9, 4.5, true);
                this._tarifaActual.AnadirProducto(10, 6.6, true);
                tarifaAlternativa.AnadirProducto(10, 4.7, true);
                //Bebidas
                this._tarifaActual.AnadirProducto(11, 5.5, true);
                tarifaAlternativa.AnadirProducto(11, 4.2, true);
                this._tarifaActual.AnadirProducto(12, 3.5, true);
                tarifaAlternativa.AnadirProducto(12, 2.5, true);
                this._tarifaActual.AnadirProducto(13, 3.0, true);
                tarifaAlternativa.AnadirProducto(13, 2.2, true);
                this._tarifaActual.AnadirProducto(14, 2.5, true);
                tarifaAlternativa.AnadirProducto(14, 1.8, true);
                //Complementos
                this._tarifaActual.AnadirProducto(15, 1, true);
                tarifaAlternativa.AnadirProducto(15, 0.5, true);
                this._tarifaActual.AnadirProducto(16, 0.7, true);
                tarifaAlternativa.AnadirProducto(16, 0.4, true);
                this._tarifaActual.AnadirProducto(17, 1.5, true);
                tarifaAlternativa.AnadirProducto(17, 1, true);
                this._tarifaActual.AnadirProducto(18, 1.5, true);
                tarifaAlternativa.AnadirProducto(18, 1, true);

                ControladorComun.BD!.ActualizarObjeto(_tarifaActual);
                ControladorComun.BD!.ActualizarObjeto(tarifaAlternativa);
            }
            else
            {
                this._tarifaActual = ControladorComun.BD!.BuscarObjetosIntAndInt<Tarifa>("CodTarifa", _tpvCFG.TarifaDefecto, "CodTienda", _tpvCFG.CodTienda)[0];
                //ControladorComun.TiendaActual!.TarifaDefecto = _tarifaActual;
            }
            ControladorComun.TiendaActual!.TarifaDefecto = this._tarifaActual;
        }

        public List<Tarifa> ListaTarifas()
        {
            List<Tarifa> listarifa = new List<Tarifa>();
            listarifa = ControladorComun.BD!.BuscarObjetosInt<Tarifa>("CodTienda", this._tpvCFG.CodTienda).ToList();
            return listarifa;
        }

        private void CargarSecciones()
        {
            if (ControladorComun.BD!.ContarObjetos<Seccion>() < 1) //Si no hay ninguna, cargamos las de muestra para la presentación
            {
                this._secciones = new List<Seccion>();
                this._secciones.Add(new Seccion(0, 0, "Sushi", 0));
                this._secciones[0].AddProducto(this._productos![0]);
                this._secciones[0].AddProducto(this._productos![1]);
                this._secciones[0].AddProducto(this._productos![2]);
                this._secciones[0].AddProducto(this._productos![3]);
                this._secciones[0].AddProducto(this._productos![4]);
                this._secciones[0].AddProducto(this._productos![5]);
                this._secciones[0].AddProducto(this._productos![6]);
                this._secciones[0].AddProducto(this._productos![7]);
                this._secciones[0].AddProducto(this._productos![8]);
                this._secciones[0].AddProducto(this._productos![9]);
                this._secciones[0].AddProducto(this._productos![10]);

                this._secciones.Add(new Seccion(1, 1, "Bebidas", 0));
                this._secciones[1].AddProducto(this._productos![11]);
                this._secciones[1].AddProducto(this._productos![12]);
                this._secciones[1].AddProducto(this._productos![13]);
                this._secciones[1].AddProducto(this._productos![14]);

                this._secciones.Add(new Seccion(2, 2, "Complementos", 0));
                this._secciones[2].AddProducto(this._productos![15]);
                this._secciones[2].AddProducto(this._productos![16]);
                this._secciones[2].AddProducto(this._productos![17]);
                this._secciones[2].AddProducto(this._productos![18]);

                foreach (Seccion s in _secciones)
                {
                    ControladorComun.BD!.PersistirObjeto(s);
                }
            }
            this._secciones = ControladorComun.BD!.BuscarObjetosInt<Seccion>("CodTienda", this._tpvCFG.CodTienda).ToList();
        }


        private void CargarProductos()
        {
            if (ControladorComun.BD!.ContarObjetos<Producto>() < 1)
            { //Productos de muestra incluidos el proyecto para la presentación
                //Sushi
                ControladorComun.BD!.PersistirObjeto(new Producto(0, 0, "California Maki", "California Maki 4 unidades", this._tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/californiamaki.png"));                
                ControladorComun.BD!.PersistirObjeto(new Producto(1, "Futo Maki", "Futo Maki 4 unidades", this._tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/futomaki.png"));
                ControladorComun.BD!.PersistirObjeto(new Producto(2, "Band. 10 Hosomaki", "Bandeja de 10 Hosomaki", _tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/5hsomaki.png"));
                ControladorComun.BD!.PersistirObjeto(new Producto(3, "Gunkan Maki", "Gunkan Maki 4 unidades", _tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/gunkanmaki.png"));
                ControladorComun.BD!.PersistirObjeto(new Producto(4, "Hoso Maki", "Hoso Maki 4 unidades", _tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/hosomaki.png"));
                ControladorComun.BD!.PersistirObjeto(new Producto(5, "Iniozuki", "Iniozuki 4 unidades", _tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/iniozuki.png"));
                ControladorComun.BD!.PersistirObjeto(new Producto(6, "Maguro Nigiri", "Maguro Nigiri 4 unidades", _tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/maguronigiri.png"));
                ControladorComun.BD!.PersistirObjeto(new Producto(7, "Nigiri", "Nigiri 4 unidades", _tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/nigiri.png"));
                ControladorComun.BD!.PersistirObjeto(new Producto(8, "TriFutoMaki", "Futo Maki Triangular 4 unidades", _tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/triFutomaki.png"));
                ControladorComun.BD!.PersistirObjeto(new Producto(9, "Uramaki", "Uramaki 4 unidades", _tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/uramaki.png"));
                ControladorComun.BD!.PersistirObjeto(new Producto(10, "Philadelphia Roll", "Philadelphia Roll 4 unidades", _tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/philadelphiaroll.png"));
                //Bebidas
                ControladorComun.BD!.PersistirObjeto(new Producto(11, "Sake", "15ml de Sake", _tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/sake.png"));
                ControladorComun.BD!.PersistirObjeto(new Producto(12, "Cerveza Sapporo", "Cerveza Sapporo 33cl", _tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/sapporo.png"));
                ControladorComun.BD!.PersistirObjeto(new Producto(13, "Copa de vino rosado", "Copa de vino rosado de la casa", _tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/copavino.png"));
                ControladorComun.BD!.PersistirObjeto(new Producto(14, "Agua Evian", "Botella de 25cl", _tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/evian.png"));
                //Complementos
                ControladorComun.BD!.PersistirObjeto(new Producto(15, "Palillos", "Palillos chinos", _tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/palillos.png"));
                ControladorComun.BD!.PersistirObjeto(new Producto(16, "Gengibre", "Gengibre 1 ración", _tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/ginger.png"));
                ControladorComun.BD!.PersistirObjeto(new Producto(17, "Wasabi", "Wasabi 1 ración", _tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/wasabi.png"));
                ControladorComun.BD!.PersistirObjeto(new Producto(18, "Soja", "Salsa de soja 1 ración", _tpvCFG.CodTienda, "pack://application:,,,/Recursos/Imagenes/Productos/soja.png"));
            }   
            this._productos = ControladorComun.BD!.BuscarObjetosInt<Producto>("CodTienda", _tpvCFG.CodTienda).ToList();
        }
        private void CargarFormasPago()
        {
            if (ControladorComun.BD!.ContarObjetos<FormaPago>() < 1)
            { //Incluidos en el proyecto para la presentación
                ControladorComun.BD!.PersistirObjeto(new FormaPago("Efectivo", "Efectivo", this._tpvCFG.CodTienda, false));
                ControladorComun.BD!.PersistirObjeto(new FormaPago("Tarjeta", "Tarjeta de crédito o débito", this._tpvCFG.CodTienda, true));
            }
            this._formasPago = ControladorComun.BD!.BuscarObjetosInt<FormaPago>("CodTienda", this._tpvCFG.CodTienda).ToList();
        }

        public Ticket? GeneraNuevoTicket(bool persistirlo)
        {
            Ticket nuevoT;
            try { 
                int SoloNumTicket = Convert.ToInt32(RecibirNumTicketDesdeMaster());
                nuevoT = new Ticket('T', ControladorComun.TpvBase!.NumTPV, ControladorComun.TiendaActual!.CodTienda, ControladorComun.TpvBase.UsuarioActual!, SoloNumTicket);
                if (persistirlo) ControladorComun.BD!.PersistirObjeto(nuevoT);
            }catch (Exception)
            {
                return null;
            }
            return nuevoT;
        }

        public string RecibirNumTicketDesdeMaster()
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(ControladorComun.TiendaActual!.IPTPVMaster, 12700);
                NetworkStream stream = client.GetStream();
                MemoryStream ms = new MemoryStream();
                byte[] buffer = new byte[1024];
                int bytesLeidos;
                while ((bytesLeidos = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, bytesLeidos);
                }
                byte[] msgEncriptado = ms.ToArray();           
                client.Close();
                return Desencriptar(msgEncriptado, ControladorComun.ClaveCompartida, ControladorComun.Iv); ;
            }
            catch (Exception ex)
            {
                if (ControladorComun.debugMode) { MessageBox.Show("Error al conectar con TPVMaster: " + ex.Message, "Error"); }
                return string.Empty;
            }
        }

        private string Desencriptar(byte[] data, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(data))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        public bool FinTPV()
        {
            if (this._tpvCFG.IsTPVMaster && this._procesoMaster != null)
            {
                if (!_procesoMaster!.IsRunning)
                {
                    return false;
                }
                else
                {
                    this._procesoMaster.Parar();
                }
            }
            return false;
        }

        public void BloqueaTPV()
        {
            VentanaTecladoNumericoUsuario ventanaBloqueo = new VentanaTecladoNumericoUsuario(true);
            ControladorComun.AplicarFondoBorroso(ventanaBloqueo);
            ventanaBloqueo.ShowDialog();
            ControladorComun.QuitarFondoBorroso();
            ControladorComun.CambiarTema(ControladorComun.TpvBase!.UsuarioActual!.TemaVisual!);
            this._ventanaPrincipal!.ActualizaInfoUsuario();                        
        }

        public bool CompruebaClave(string clave)
        {
            List<Usuario> usuarios = ControladorComun.BD!.BuscarObjetosStringAndInt<Usuario>("Clave", clave, "CodTienda", this._tpvCFG.CodTienda).ToList();
            if (usuarios.Count > 0)
            {
                return usuarios.Any(x => x.EsActivo);
            }
            return false;
        }

        public void AgregarProductoASeccion(Producto producto, int CodSeccion)
        {
            Seccion seccion = this._secciones?.FirstOrDefault(s => s.CodSeccion == CodSeccion)!;
            if (seccion != null)
            {
                seccion.AddProducto(producto);
                ControladorComun.BD!.PersistirObjeto(seccion);
            }
        }
        public void AgregarProductoASeccion(int CodSeccion, int CodProducto)
        {
            Seccion seccion = this._secciones?.FirstOrDefault(s => s.CodSeccion == CodSeccion)!;
            if (seccion != null)
            {
                seccion.AddProducto(ControladorComun.BD!.BuscarObjetosInt<Producto>("CodProducto", CodProducto)[0]);
                ControladorComun.BD!.PersistirObjeto(seccion);
            }
        }

        public void InsertarLineaVenta(Producto producto)
        {
            if (this._posicionVentaActual == null)
            {
                this._posicionVentaActual = new PosicionVenta(0, 0, this._tarifaActual!.CodTarifa);
            }
            Linea nuevaLinea = new Linea(producto, this._tarifaActual!.ObtenerPrecioProducto(producto.CodProducto));
            this.PosicionVentaActual!.LineasPantalla.Add(nuevaLinea);
        }

        private string GenerarFormatoTicket(Ticket ticket)
        {            
            StringBuilder formato = new StringBuilder();
            formato.AppendLine(ControladorComun.TiendaActual!.Descripcion);
            formato.AppendLine(ControladorComun.TiendaActual!.OtrosDatos);
            formato.AppendLine("----------------------------------------------");
            formato.AppendLine("Factura simplificada #" + ticket.NumTicket);
            formato.AppendLine("----------------------------------------------");
            formato.AppendLine("Fecha: " + ticket.FechaHora);
            formato.AppendLine("Le ha atendido: " + ticket.Vendedor.Nombre);
            formato.AppendLine("----------------------------------------------");
            formato.AppendLine("Producto                      Precio");

            string invisibleChar = " "; //Caracter para rellenar huecos vacíos

            foreach (Linea l in ticket.Lineas)
            {
                //30 caracteres, ocupados por caracteres invisibles
                StringBuilder linea = new StringBuilder(new string(invisibleChar[0], 30));

                //El nombre del producto lo pongo en la posición 0
                linea.Insert(0, l.Producto.Nombre);

                //Ponemos el precio a partir de la posición 30
                string precio = l.Precio.ToString("F2") + "€";
                linea.Insert(30, precio);
                formato.AppendLine(linea.ToString());
            }

            formato.AppendLine("----------------------------------------------");

            foreach (Vencimiento v in ticket.Vencimientos)
            {
                if (v.Devuelto == 0.0)
                {
                    StringBuilder linea = new StringBuilder(new string(invisibleChar[0], 30));
                    linea.Insert(0, v.FormaPago);
                    linea.Insert(30, v.Total.ToString("F2") + "€");
                    formato.AppendLine(linea.ToString());
                }
                else
                {
                    formato.AppendLine(v.FormaPago);
                    formato.AppendLine("Entregado:".PadRight(30) + v.Entregado.ToString("F2") + "€");
                    formato.AppendLine("Devuelto:".PadRight(30) + v.Devuelto.ToString("F2") + "€");
                }
            }

            formato.AppendLine("----------------------------------------------");
            formato.AppendLine("Total (imp. incl.): ".PadRight(30) + Math.Round(ticket.Total, 2).ToString("F2") + "€");
            formato.AppendLine("----------------------------------------------");
            formato.AppendLine("Gracias por su visita");

            return formato.ToString();
        }            

        public string ImprimirTicket(Ticket ticket)
        {
            this.ticketFormato = GenerarFormatoTicket(ticket);
            if (!ControladorComun.debugMode)            
            { //Si no estamos en modo Debug, lo imprimimos de verdad
                try
                {
                    PrintDocument ticketImpreso = new PrintDocument();
                    ticketImpreso.PrintPage += new PrintPageEventHandler(ManejadorImpresion);
                    ticketImpreso.Print();
                }
                catch (Exception)
                {
                    return "Error"; //Si devuelve error, el llamante deberá de la vista deberá mostrar un mensaje de error
                }
            }
            //Si estamos en modo debug, devolvemos el formato para que el llamante de la vista lo imprima
            return this.ticketFormato;
        }

        private void ManejadorImpresion(object sender, PrintPageEventArgs e)
        {
            Font font = new Font("Arial", 12);
            Brush brush = Brushes.Black;
            PointF point = new PointF(10, 10);
            e.Graphics!.DrawString(this.ticketFormato, font, brush, point);
        }

        public void AnularTicket(Ticket ticket, string motivo)
        {
            ticket.Anulado = true;
            ticket.MotivoAnulacion = motivo;
            ControladorComun.BD!.ActualizarObjeto(ticket);
            
            Ticket nuevoTicketAnulacion = ticket;
            nuevoTicketAnulacion.Tipo = 'D';
            nuevoTicketAnulacion.Id = ControladorComun.BD!.SelectMAXInt("Ticket", "_id") + 1;
            nuevoTicketAnulacion.NumTicket = "Da igual lo que escriba porque lo calcula el getter, pero algo tengo que poner para que lo cambie";
            nuevoTicketAnulacion.Anulado = false;
            nuevoTicketAnulacion.Vendedor = ControladorComun.TpvBase!.UsuarioActual!;
            nuevoTicketAnulacion.FechaHora = DateTime.Now;
            foreach (Linea linea in ticket.Lineas)
            {
                linea.Precio = linea.Precio * -1;
            }
            foreach (Vencimiento vencimiento in ticket.Vencimientos)
            {
                vencimiento.Total = vencimiento.Total * -1;
                vencimiento.Devuelto = vencimiento.Devuelto * -1;
                vencimiento.Entregado = vencimiento.Entregado * -1;
            }
            ControladorComun.BD!.PersistirObjeto<Ticket>(nuevoTicketAnulacion);
        }
    }
}
