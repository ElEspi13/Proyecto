using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using ProyectoTicketing.Clases;
using Microsoft.Maui.Animations;


namespace ProyectoTicketing.Servicios
{
    internal class BBDD
    {
        private MongoClient client;
        private IMongoDatabase database;
        private Usuario usuario = new Usuario();
        private string connectionString = "mongodb://localhost:27017";
        public BBDD()
        {


        }

        internal void CerrarSesion()
        {
            usuario=new Usuario();
        }

        internal int ComprobarRolUsuario()
        {
            return usuario.Rol;
        }

        internal bool ComprobarUsuario(string nombre, string contrasena)
        {
            try
            {
                IMongoCollection<BsonDocument> collectionUsuarios = database.GetCollection<BsonDocument>("usuarios");

                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.And(
                 Builders<BsonDocument>.Filter.Eq("Usuario", nombre),
                 Builders<BsonDocument>.Filter.Eq("Contrasena", contrasena)
             );

                BsonDocument usuario = collectionUsuarios.Find(filter).FirstOrDefault();
                if (usuario != null)
                {
                    this.usuario.Id = usuario["_id"].ToString();
                    this.usuario.Rol = int.Parse(usuario["Rol"].ToString());
                    this.usuario.Nombre = usuario["Usuario"].ToString();
                    return usuario != null;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        internal bool Conexion()
        {
            try
            {

                client = new MongoClient(connectionString);

                database = client.GetDatabase("ticketingDB");

                var collections = database.ListCollections().ToList();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        internal bool ExisteConfiguracion()
        {
            if (Conexion())
            {

                var collectionConfiguraciones = database.GetCollection<BsonDocument>("configuracion");


                var filter = Builders<BsonDocument>.Filter.Eq("IDUsuario", usuario.Id);


                var configuracion = collectionConfiguraciones.Find(filter).FirstOrDefault();


                return configuracion != null;
            }
            else
            {

                return false;
            }


        }

        internal void GuardarConfiguracion(int tema, int idioma, double fuente)
        {
            try
            {
                if (Conexion())
                {
                    var collectionConfiguraciones = database.GetCollection<BsonDocument>("configuracion");

                    // Verifica si ya existe la configuración
                    if (ExisteConfiguracion())
                    {
                        // Actualiza la configuración existente
                        var filter = Builders<BsonDocument>.Filter.Eq("IDUsuario", usuario.Id);
                        var updateDefinition = Builders<BsonDocument>.Update
                            .Set("Tema", tema)
                            .Set("Idioma", idioma)
                            .Set("Fuente", fuente);

                        collectionConfiguraciones.UpdateOne(filter, updateDefinition);
                    }
                    else
                    {
                        // Crea una nueva configuración
                        var configuracionDoc = new BsonDocument
                {
                    { "IDUsuario", usuario.Id },
                    { "Tema", tema },
                    { "Idioma", idioma },
                    { "Fuente", fuente }
                };

                        collectionConfiguraciones.InsertOne(configuracionDoc);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar la configuración: {ex.Message}");
            }
        }

        internal void RegistrarUsuario(string nombreUsuario, string contrasena)
        {
            try
            {
                // Verificar conexión con la base de datos
                if (Conexion())
                {
                    // Obtener la colección de usuarios
                    var collectionUsuarios = database.GetCollection<BsonDocument>("usuarios");

                    // Crear un nuevo documento Bson para el usuario
                    var usuarioDoc = new BsonDocument
            {
                { "Usuario", nombreUsuario },
                { "Contrasena", contrasena },
                { "Rol", 1 } // Asigna un rol por defecto; ajusta según sea necesario
            };

                    // Insertar el documento en la colección
                    collectionUsuarios.InsertOne(usuarioDoc);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar el usuario: {ex.Message}");
            }
        }


        internal (int tema, int idioma, double fuente) SacarConfiguracion()
        {
            if (Conexion())
            {
                // Obtener la colección de configuraciones
                var collectionConfiguraciones = database.GetCollection<BsonDocument>("configuracion");

                // Crear un filtro para buscar la configuración por el ID del usuario
                var filter = Builders<BsonDocument>.Filter.Eq("IDUsuario", usuario.Id);

                // Encontrar la configuración del usuario
                var configuracion = collectionConfiguraciones.Find(filter).FirstOrDefault();

                // Verificar si se encontró la configuración
                if (configuracion != null)
                {
                    // Obtener los valores de la configuración
                    int tema = configuracion["Tema"].AsInt32;
                    int idioma = configuracion["Idioma"].AsInt32;
                    double fuente = configuracion["Fuente"].AsDouble;

                    return (tema, idioma, fuente);
                }
                else
                {
                    return (0, 0, 0);
                }
            }
            else
            {
                return (0, 0, 0);
            }
        }
    }
}
