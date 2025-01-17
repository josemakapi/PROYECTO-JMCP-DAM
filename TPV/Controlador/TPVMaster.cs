using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.IO;
using System.Windows;

namespace TPV.Controlador
{
    /// <summary>
    /// Clase que se carga si este terminal está configurado como master. Se encarga de enviar 
    /// el último número de ticket a los TPV's esclavos que se conecten a él de forma concurrente. 
    /// Se ejecuta en un hilo aparte y los nº de ticket se envían cifrados. TPVBase se encarga
    /// de recibir y descifrar los nº de ticket.
    /// </summary>
    public class TPVMaster
    {
        private bool _isRunning;
        //private Thread? _hiloCliente; //Como explicaremos más abajo, no es recomendable usar Thread en C#
        private TcpListener? _tcpListener;
        //Semáforo para controlar la zona crítica. Sólo un hilo podrá acceder a la BD a la vez
        //para evitar que se lea el mismo nº de ticket por distintos clientes
        private SemaphoreSlim _bloqueo = new SemaphoreSlim(1, 1);
        public bool IsRunning { get { return this._isRunning; } }

        public TPVMaster()
        {
            this._isRunning = false;
            Iniciar();
        }

        /// <summary>
        /// Inicia el hilo de escucha
        /// </summary>
        /// <returns> false = error; true = OK</returns>
        public async Task <bool> Iniciar()
        {
            if (!this._isRunning)
            {
                try
                {
                    await Listener();                    
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Detiene el hilo de escucha
        /// </summary>
        public void Parar()
        {
            if (this._isRunning)
            {
                this._isRunning = false;
                if (ControladorComun.debugMode) MessageBox.Show("Cerrando proceso TPV Master");
            }
        }

        /// <summary>
        /// Listener que escucha las conexiones entrantes de los TPV's esclavos, instancia un hilo por cada conexión 
        /// y les envía el último número de ticket
        /// </summary>
        /// <returns>Código de terminación -1, -2 = error. Código 0 = OK </returns>
        private async Task <int> Listener()
        {
            this._isRunning = true;
            int estado = 1;
            
            this._tcpListener = new TcpListener(IPAddress.Any, 12700);
            this._tcpListener.Start();

            while (this._isRunning)
            {              
                try
                {
                    TcpClient cliente = await this._tcpListener.AcceptTcpClientAsync();
                    if (ControladorComun.debugMode) MessageBox.Show("Cliente " + cliente.Client.RemoteEndPoint!.ToString() + " conectado");

                    //Procesamos el cliente
                    _ = Task.Run(() => ProcesarCliente(cliente));
                    //this._hiloCliente = new Thread(async () => await ProcesarCliente(cliente)); //Vetusto, C# no funciona bien usando Thread. En su lugar, uso Task.Run
                    //this._hiloCliente.Start();
                }
                catch (InvalidOperationException) { return -2; }
                catch (Exception)
                {
                    return -1;
                }
            }

            this._tcpListener!.Stop();
            estado = 0;
            return estado;
        }

        /// <summary>
        /// Procesa el cliente recibido por el método Listener. Busca en la BD el último número de ticket y lo envía al cliente cifrado
        /// </summary>
        /// <param name="cliente">Cliente que ha recibido del método Listener para ser procesado</param>
        /// <returns></returns>
        private async Task<bool> ProcesarCliente(TcpClient cliente)
        {
            try
            {
                NetworkStream stream = cliente.GetStream();
                await this._bloqueo.WaitAsync();
                try
                {
                    //Ciframos el número de ticket más alto antes de enviarlo. Sólo se hará con los tickets tipo 'T'

                    long lastTicketNum = ControladorComun.BD!.BuscarUltimoTicket(ControladorComun.TiendaActual!.CodTienda, 'T');
                    byte[] iv = new byte[16] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                    byte[] encryptedData = EncryptData(lastTicketNum.ToString(), ControladorComun.ClaveCompartida, ControladorComun.Iv); //Aquí encriptamos los datos

                    //Enviamos los datos cifrados al cliente
                    await stream.WriteAsync(encryptedData.ToArray().AsMemory(0, encryptedData.Length));                  
                }
                finally
                {
                    this._bloqueo.Release(); //Liberamos el semáforo una vez terminamos
                }
                stream.Close();
                cliente.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Método que cifra los datos a enviar. Usaremos AES con modo CBC y Padding PKCS7, con una clave compartida entre el master y los esclavos
        /// </summary>
        /// <param name="numTicket"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        private byte[] EncryptData(string numTicket, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(numTicket);
                        }
                    }
                    byte[] msgCryptEnviar = msEncrypt.ToArray();
                    return msgCryptEnviar;
                }
            }
        }
    }
}
