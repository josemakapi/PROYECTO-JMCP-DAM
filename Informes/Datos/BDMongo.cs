using MongoDB.Bson;
using MongoDB.Driver;
using System.Reflection;

namespace Informes.Datos
{
    /// <summary>
    /// Contiene la lógica de los aspectos específicos del TPV y su uso
    /// </summary>
    public class BDMongo
    {
        private IMongoDatabase? _dbTPV;
        private MongoClient? _clienteDB;
        private string? _directConnectionString;
        private string? _cloudConnectionString;
        public string DirectConnectionString { get => _directConnectionString!; set => _directConnectionString = value; }
        private string? _host;
        public string Host { get => _host!; set => _host = value; }

        private int _port;
        public int Port { get => _port; set => _port = value; }

        public BDMongo(string username, string password)
        {
            this._cloudConnectionString = $"mongodb://{username}:{password}@ac-dykvbnl-shard-00-00.zxd0ycl.mongodb.net:27017,ac-dykvbnl-shard-00-01.zxd0ycl.mongodb.net:27017,ac-dykvbnl-shard-00-02.zxd0ycl.mongodb.net:27017/?ssl=true&replicaSet=atlas-ew8o6j-shard-0&authSource=admin&retryWrites=true&w=majority&appName=Cluster0";
        }

        public bool ConectarBDCloud(string dbName)
        {
            try
            {
                MongoClientSettings settings = MongoClientSettings.FromConnectionString(this._cloudConnectionString);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);
                this._clienteDB = new MongoClient(settings);
                this._dbTPV = this._clienteDB.GetDatabase(dbName);
                return IsBDConnected();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsBDConnected()
        {
            try
            {
                BsonDocument result = _dbTPV!.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                return result.Contains("ok") && result["ok"].ToDouble() == 1.0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool PersistirObjeto<T>(T objeto)
        {
            try
            {
                IMongoCollection<T> collection = this._dbTPV!.GetCollection<T>(typeof(T).Name);
                collection.InsertOne(objeto);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CheckUsuarioInformes(string user, string pass)
        {
            try
            {
                IMongoCollection<BsonDocument> collection = this._dbTPV!.GetCollection<BsonDocument>("UsuarioInformes");
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("Nombre", user),
                    Builders<BsonDocument>.Filter.Eq("Password", pass)
                );
                return collection.Find(filter).ToList().Count > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<T> LeerObjetosTipo<T>()
        {
            try
            {
                IMongoCollection<T> collection = this._dbTPV!.GetCollection<T>(typeof(T).Name);
                return collection.Find(new BsonDocument()).ToList();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        public bool EliminarObjeto<T>(T objeto)
        {
            try
            {
                IMongoCollection<T> collection = this._dbTPV!.GetCollection<T>(typeof(T).Name);
                var idProperty = objeto!.GetType().GetProperty("Id");
                if (idProperty == null)
                {
                    throw new ArgumentException($"El objeto de tipo {typeof(T).Name} no tiene una NombrePropiedad 'Id'.");
                }
                var idValue = idProperty.GetValue(objeto);
                if (idValue == null)
                {
                    throw new ArgumentException($"El valor de la NombrePropiedad 'Id' no puede ser nulo.");
                }
                var filter = Builders<T>.Filter.Eq("Id", idValue);
                var result = collection.DeleteOne(filter);
                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<T> BuscarObjeto<T>(T objeto, string NombreCampoClaveBusqueda)
        {
            try
            {
                IMongoCollection<T> collection = this._dbTPV!.GetCollection<T>(typeof(T).Name);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq(NombreCampoClaveBusqueda, objeto!.GetType().GetProperty(NombreCampoClaveBusqueda)?.GetValue(objeto));
                return collection.Find(filter).ToList();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        public List<T> BuscarObjetosString<T>(string NombrePropiedad, string valor)
        {
            try
            {
                IMongoCollection<T> collection = this._dbTPV!.GetCollection<T>(typeof(T).Name);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq(NombrePropiedad, valor);
                return collection.Find(filter).ToList();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        public List<T> BuscarObjetosDate<T>(string NombrePropiedad, DateOnly valor)
        {
            try
            {
                IMongoCollection<T> collection = this._dbTPV!.GetCollection<T>(typeof(T).Name);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq(NombrePropiedad, valor);
                return collection.Find(filter).ToList();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        public List<T> BuscarObjetosStringAndString<T>(string NombrePropiedad1, string valor1, string NombrePropiedad2, string valor2)
        {
            try
            {
                IMongoCollection<T> collection = this._dbTPV!.GetCollection<T>(typeof(T).Name);
                FilterDefinition<T> filter = Builders<T>.Filter.And(
                    Builders<T>.Filter.Eq(NombrePropiedad1, valor1),
                    Builders<T>.Filter.Eq(NombrePropiedad2, valor2)
                );
                return collection.Find(filter).ToList();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        public List<T> BuscarObjetosStringAndInt<T>(string NombrePropiedadString, string valor1, string NombrePropiedadInt, int valor2)
        {
            try
            {
                IMongoCollection<T> collection = this._dbTPV!.GetCollection<T>(typeof(T).Name);
                FilterDefinition<T> filter = Builders<T>.Filter.And(
                    Builders<T>.Filter.Eq(NombrePropiedadString, valor1),
                    Builders<T>.Filter.Eq(NombrePropiedadInt, valor2)
                );
                return collection.Find(filter).ToList();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        public List<T> BuscarObjetosBool<T>(string NombrePropiedad, bool valor)
        {
            try
            {
                IMongoCollection<T> collection = this._dbTPV!.GetCollection<T>(typeof(T).Name);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq(NombrePropiedad, valor);
                return collection.Find(filter).ToList();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        public List<T> BuscarObjetosInt<T>(string NombrePropiedad, int valor)
        {
            try
            {
                IMongoCollection<T> collection = this._dbTPV!.GetCollection<T>(typeof(T).Name);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq(NombrePropiedad, valor);
                return collection.Find(filter).ToList();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        public List<T> BuscarObjetosIntAndInt<T>(string NombrePropiedad1, int valor1, string NombrePropiedad2, int valor2)
        {
            try
            {
                IMongoCollection<T> collection = this._dbTPV!.GetCollection<T>(typeof(T).Name);
                FilterDefinition<T> filter = Builders<T>.Filter.And(
                    Builders<T>.Filter.Eq(NombrePropiedad1, valor1),
                    Builders<T>.Filter.Eq(NombrePropiedad2, valor2)
                );
                return collection.Find(filter).ToList();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        public List<T> LeerObjetosTest<T>(List<T> objetos, string propiedad)
        {
            try
            {
                IMongoCollection<T> collection = this._dbTPV!.GetCollection<T>(typeof(T).Name);
                List<T> listaObjetos = new List<T>();
                foreach (T objeto in objetos)
                {
                    FilterDefinition<T> filter = Builders<T>.Filter.Eq(propiedad, objeto!.GetType().GetProperty(propiedad)?.GetValue(objeto));
                    listaObjetos.Add(collection.Find(filter).FirstOrDefault());
                }
                return listaObjetos;
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }




        public bool EliminarObjeto<T>(T objeto, string propiedad)
        {
            try
            {
                IMongoCollection<T> collection = this._dbTPV!.GetCollection<T>(typeof(T).Name);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq(propiedad, objeto!.GetType().GetProperty(propiedad)?.GetValue(objeto));
                collection.DeleteOne(filter);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ActualizarObjeto<T>(T objeto)
        {
            try
            {
                IMongoCollection<T> collection = this._dbTPV!.GetCollection<T>(typeof(T).Name);

                // Usa reflexión para obtener la NombrePropiedad Id
                PropertyInfo? idProperty = typeof(T).GetProperty("Id");
                if (idProperty == null)
                {
                    throw new ArgumentException($"El objeto de tipo '{typeof(T).Name}' no tiene una NombrePropiedad 'Id'.");
                }

                // Obtiene el valor de la NombrePropiedad Id
                object? idValue = idProperty.GetValue(objeto);
                if (idValue == null)
                {
                    throw new ArgumentException($"El valor de la NombrePropiedad 'Id' no puede ser nulo.");
                }

                // Crea un filtro para encontrar el documento con el Id especificado
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", idValue);

                // Realiza la actualización del documento
                ReplaceOneResult result = collection.ReplaceOne(filter, objeto);

                // True si ok
                return result.IsAcknowledged && result.ModifiedCount > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int SelectMAXInt(string nombreClase, string nombreMiembroPrivado)
        {
            try
            {
                IMongoCollection<BsonDocument> collection = this._dbTPV!.GetCollection<BsonDocument>(nombreClase);
                var sort = Builders<BsonDocument>.Sort.Descending(nombreMiembroPrivado);
                var document = collection.Find(new BsonDocument()).Project(Builders<BsonDocument>.Projection.Include(nombreMiembroPrivado)).Sort(sort).FirstOrDefault();
                if (document != null)
                {
                    return document.GetValue(nombreMiembroPrivado).AsInt32;
                }
            }
            catch (Exception)
            {
                return -1;
            }
            return 0;
        }
       

        public bool DesconectarBD()
        {
            try
            {
                this._clienteDB = null;
            }
            catch (Exception) { return false; }
            return true;
        }
        public int ContarObjetos<T>()
        {
            try
            {
                IMongoCollection<T> collection = this._dbTPV!.GetCollection<T>(typeof(T).Name);
                return (int)collection.CountDocuments(new BsonDocument());
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
