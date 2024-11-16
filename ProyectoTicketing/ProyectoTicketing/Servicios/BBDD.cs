using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
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
        private MonitoreoTecnico monitoreoTecnico;
        private MonitoreoUsuario monitoreoUsuario;
        private MongoClient client;
        private IMongoDatabase database;
        private Usuario usuario = new Usuario();
        private GridFSBucket gridFSBucket;
        private string connectionString = "mongodb://CuidaoConLosHackersn:ContraseñaSegura123@79.116.92.21:27017/ticketingDB?connectTimeoutMS=2000&socketTimeoutMS=2000";
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
                gridFSBucket = new GridFSBucket(database);

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

        public async Task SubirTicketAsync(Ticket ticket)
        {
            var documentosAdjuntos = new List<BsonDocument>();

            // Subir documentos a GridFS y almacenar metadatos
            foreach (var documento in ticket.Documentos)
            {
                using (var fileStream = File.OpenRead(documento.RutaArchivo))
                {
                    var fileId = await gridFSBucket.UploadFromStreamAsync(documento.NombreArchivo, fileStream);

                    // Crear instancia de Documento con el ObjectId de GridFS
                    var docMetadata = new Documento(fileId, documento.NombreArchivo, documento.TipoArchivo,documento.RutaArchivo);

                    // Agregar la referencia del documento al ticket
                    documentosAdjuntos.Add(new BsonDocument
            {
                { "IdDocumento", docMetadata.IdDocumento },
                { "NombreArchivo", docMetadata.NombreArchivo },
                { "TipoArchivo", docMetadata.TipoArchivo }
            });
                }
            }

            var ticketDoc = new BsonDocument
            {
                { "descripcion", string.IsNullOrEmpty(ticket.Descripcion) ? BsonNull.Value : ticket.Descripcion },
                { "solucion", string.IsNullOrEmpty(ticket.Solucion) ? BsonNull.Value : ticket.Solucion },
                { "categoria", string.IsNullOrEmpty(ticket.Categoria) ? BsonNull.Value : ticket.Categoria },
                { "tipoError", string.IsNullOrEmpty(ticket.TipoError) ? BsonNull.Value : ticket.TipoError },
                { "estado", string.IsNullOrEmpty(ticket.Estado) ? "Abierto" : ticket.Estado },  // Puedes asignar un valor por defecto como "Abierto"
                { "fechaCreacion", ticket.FechaCreacion },
                { "prioridad", string.IsNullOrEmpty(ticket.Prioridad) ? "Media" : ticket.Prioridad },  // Valor predeterminado si es null o vacío
                { "nombreTicket", string.IsNullOrEmpty(ticket.NombreTicket) ? "Ticket Nuevo" : ticket.NombreTicket },  // Valor predeterminado si es null o vacío
                { "IDUsuario", usuario.Id },
                { "IDTecnico", BsonNull.Value },  // Usamos BsonNull si IDTecnico es null
                { "documentosAdjuntos", new BsonArray(documentosAdjuntos ?? new List<BsonDocument>()) }
            };



            await database.GetCollection<BsonDocument>("tickets").InsertOneAsync(ticketDoc);
        }


        public async Task<List<Ticket>> ObtenerTicketsDeUsuarioAsync()
        {
            // Obtener la colección de tickets
            var ticketsCollection = database.GetCollection<Ticket>("tickets");

            // Crear el filtro para buscar los tickets del usuario por su ID
            var filter = Builders<Ticket>.Filter.Eq("IDUsuario",usuario.Id);

            // Realizar la consulta y devolver los tickets encontrados
            List<Ticket> tickets = await ticketsCollection.Find(filter).ToListAsync();

            return tickets;
        }
        public async Task DescargarDocumentoAsync(Documento documento)
        {
            try
            {
                // Obtener el directorio de "Descargas" de Windows
                var downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

                // Ruta completa para guardar el archivo en la carpeta de descargas
                var filePath = Path.Combine(downloadsFolder, documento.NombreArchivo);

                // Acceder al archivo en GridFS
                var fileStream = await gridFSBucket.OpenDownloadStreamAsync(documento.IdDocumento);

                // Guardar el archivo en la carpeta de descargas
                using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await fileStream.CopyToAsync(file);
                }

                // Si deseas abrir el archivo, puedes hacerlo con la función Launcher
                await Launcher.OpenAsync(filePath);
            }
            catch (Exception ex)
            {

            }
        }
        internal void IniciarMonitoreoTecnico()
        {
            if (string.IsNullOrEmpty(usuario.Id))
            {
                Console.WriteLine("El usuario no ha iniciado sesión.");
                return;
            }

            monitoreoTecnico = new MonitoreoTecnico(database, usuario.Id);
            monitoreoTecnico.IniciarMonitoreo();
            Console.WriteLine("Monitoreo para técnico iniciado.");
        }

        internal void IniciarMonitoreoUsuario()
        {
            if (string.IsNullOrEmpty(usuario.Id))
            {
                Console.WriteLine("El usuario no ha iniciado sesión.");
                return;
            }

            monitoreoUsuario = new MonitoreoUsuario(database, usuario.Id);
            monitoreoUsuario.IniciarMonitoreo();
            Console.WriteLine("Monitoreo para usuario iniciado.");
        }

        internal void DetenerMonitoreo()
        {
            monitoreoTecnico?.DetenerMonitoreo();
            monitoreoUsuario?.DetenerMonitoreo();
            Console.WriteLine("Monitoreo detenido.");
        }
    }


}
