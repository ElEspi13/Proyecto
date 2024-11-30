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
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace ProyectoTicketing.Servicios
{
    /// <summary>
    /// Clase que maneja la conexión y las operaciones en la base de datos MongoDB.
    /// </summary>
    internal class BBDD
    {
        private MonitoreoTecnico monitoreoTecnico;
        private MonitoreoUsuario monitoreoUsuario;
        private MongoClient client;
        private IMongoDatabase database;
        private Usuario usuario = new Usuario();
        private GridFSBucket gridFSBucket;
        private AppShell shell;
        /// <summary>
        /// Cadena de conexión a la base de datos MongoDB.
        /// </summary>
        private string connectionString = "mongodb://alvaroespi13:TQqtYSj3BwZbGOw7@cluster0-shard-00-00.b0c6k.mongodb.net:27017,cluster0-shard-00-01.b0c6k.mongodb.net:27017,cluster0-shard-00-02.b0c6k.mongodb.net:27017/ticketingDB?authSource=admin&ssl=true";

        /// <summary>
        /// Constructor de la clase BBDD.
        /// </summary>
        /// <param name="shell">Referencia al objeto AppShell.</param>
        public BBDD(AppShell shell)
        {
            this.shell = shell;
        }

        /// <summary>
        /// Cierra la sesión del usuario actual.
        /// </summary>
        internal void CerrarSesion()
        {
            usuario = new Usuario();
        }

        /// <summary>
        /// Obtiene el rol del usuario actual.
        /// </summary>
        /// <returns>Un valor entero que representa el rol del usuario.</returns>
        internal int ComprobarRolUsuario()
        {
            return usuario.Rol;
        }

        /// <summary>
        /// Verifica si un usuario con el nombre y contraseña proporcionados existe en la base de datos.
        /// </summary>
        /// <param name="nombre">El nombre de usuario.</param>
        /// <param name="contrasena">La contraseña del usuario.</param>
        /// <returns>True si el usuario existe y las credenciales son correctas, de lo contrario, False.</returns>
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
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Intenta establecer una conexión con la base de datos MongoDB.
        /// </summary>
        /// <returns>True si la conexión es exitosa, de lo contrario, False.</returns>
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
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica si existe una configuración para el usuario actual en la base de datos.
        /// </summary>
        /// <returns>True si la configuración existe, de lo contrario, False.</returns>
        internal bool ExisteConfiguracion()
        {
            if (Conexion())
            {
                var collectionConfiguraciones = database.GetCollection<BsonDocument>("configuracion");
                var filter = Builders<BsonDocument>.Filter.Eq("IDUsuario", usuario.Id);
                var configuracion = collectionConfiguraciones.Find(filter).FirstOrDefault();
                return configuracion != null;
            }
            return false;
        }

        /// <summary>
        /// Guarda o actualiza la configuración del usuario actual (tema, idioma, tamaño de fuente) en la base de datos.
        /// </summary>
        /// <param name="tema">Tema seleccionado por el usuario.</param>
        /// <param name="idioma">Idioma seleccionado por el usuario.</param>
        /// <param name="fuente">Tamaño de la fuente.</param>
        internal void GuardarConfiguracion(int tema, int idioma, double fuente)
        {
            try
            {
                if (Conexion())
                {
                    var collectionConfiguraciones = database.GetCollection<BsonDocument>("configuracion");

                    if (ExisteConfiguracion())
                    {
                        var filter = Builders<BsonDocument>.Filter.Eq("IDUsuario", usuario.Id);
                        var updateDefinition = Builders<BsonDocument>.Update
                            .Set("Tema", tema)
                            .Set("Idioma", idioma)
                            .Set("Fuente", fuente);

                        collectionConfiguraciones.UpdateOne(filter, updateDefinition);
                    }
                    else
                    {
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

        /// <summary>
        /// Registra un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="nombreUsuario">El nombre del nuevo usuario.</param>
        /// <param name="contrasena">La contraseña del nuevo usuario.</param>
        /// <param name="Rol">El rol asignado al nuevo usuario.</param>
        internal async void RegistrarUsuario(string nombreUsuario, string contrasena, int Rol)
        {
            try
            {
                if (Conexion())
                {
                    var collectionUsuarios = database.GetCollection<BsonDocument>("usuarios");
                    var filtro = Builders<BsonDocument>.Filter.Eq("Usuario", nombreUsuario);
                    var usuarioExistente = collectionUsuarios.Find(filtro).FirstOrDefault();

                    if (usuarioExistente != null)
                    {
                        await Shell.Current.DisplayAlert("Error", "El Nombre de Usuario ya está en uso", "OK");
                        return;
                    }

                    var usuarioDoc = new BsonDocument
            {
                { "Usuario", nombreUsuario },
                { "Contrasena", contrasena },
                { "Rol", Rol }
            };
                    collectionUsuarios.InsertOne(usuarioDoc);

                    await Shell.Current.DisplayAlert("Completado", "El Usuario ha sido creado", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar el usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene la configuración del usuario actual desde la base de datos.
        /// </summary>
        /// <returns>Una tupla con los valores de tema, idioma y tamaño de fuente.</returns>
        internal (int tema, int idioma, double fuente) SacarConfiguracion()
        {
            if (Conexion())
            {
                var collectionConfiguraciones = database.GetCollection<BsonDocument>("configuracion");
                var filter = Builders<BsonDocument>.Filter.Eq("IDUsuario", usuario.Id);
                var configuracion = collectionConfiguraciones.Find(filter).FirstOrDefault();

                if (configuracion != null)
                {
                    int tema = configuracion["Tema"].AsInt32;
                    int idioma = configuracion["Idioma"].AsInt32;
                    double fuente = configuracion["Fuente"].AsDouble;

                    return (tema, idioma, fuente);
                }
                return (0, 0, 0);
            }
            return (0, 0, 0);
        }

        /// <summary>
        /// Sube un nuevo ticket a la base de datos junto con sus documentos adjuntos en GridFS.
        /// </summary>
        /// <param name="ticket">El ticket que se subirá.</param>
        public async Task SubirTicketAsync(Ticket ticket)
        {
            var documentosAdjuntos = new List<BsonDocument>();

            foreach (var documento in ticket.Documentos)
            {
                using (var fileStream = File.OpenRead(documento.RutaArchivo))
                {
                    var fileId = await gridFSBucket.UploadFromStreamAsync(documento.NombreArchivo, fileStream);

                    var docMetadata = new Documento(fileId, documento.NombreArchivo, documento.TipoArchivo, documento.RutaArchivo);
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
        { "estado", string.IsNullOrEmpty(ticket.Estado) ? "Abierto" : ticket.Estado },
        { "fechaCreacion", ticket.FechaCreacion },
        { "prioridad", string.IsNullOrEmpty(ticket.Prioridad) ? "Media" : ticket.Prioridad },
        { "nombreTicket", string.IsNullOrEmpty(ticket.NombreTicket) ? "Ticket Nuevo" : ticket.NombreTicket },
        { "IDUsuario", usuario.Id },
        { "IDTecnico", BsonNull.Value },
        { "IDTicketPadre", ticket.IDTicketPadre == null ? BsonNull.Value : ticket.IDTicketPadre },
        { "documentosAdjuntos", new BsonArray(documentosAdjuntos) }
    };

            await database.GetCollection<BsonDocument>("tickets").InsertOneAsync(ticketDoc);
        }


        /// <summary>
        /// Obtiene los tickets asociados al usuario actual, ya sea como creador (IDUsuario) o técnico asignado (IDTecnico).
        /// </summary>
        /// <returns>Lista de tickets asociados al usuario.</returns>
        public async Task<List<Ticket>> ObtenerTicketsDeUsuarioAsync()
        {
            var ticketsCollection = database.GetCollection<Ticket>("tickets");
            var filter = Builders<Ticket>.Filter.Or(
                Builders<Ticket>.Filter.Eq("IDUsuario", usuario.Id),
                Builders<Ticket>.Filter.Eq("IDTecnico", usuario.Id)
            );
            var sort = Builders<Ticket>.Sort.Ascending(t => t.Estado);
            return await ticketsCollection.Find(filter).Sort(sort).ToListAsync();
        }

        /// <summary>
        /// Descarga un documento desde GridFS y lo guarda en la carpeta de descargas del dispositivo.
        /// </summary>
        /// <param name="documento">El documento a descargar.</param>
        public async Task DescargarDocumentoAsync(Documento documento)
        {
            try
            {
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    var statusWrite = await Permissions.RequestAsync<Permissions.StorageWrite>();
                    var statusRead = await Permissions.RequestAsync<Permissions.StorageRead>();

                    if (statusWrite == PermissionStatus.Granted && statusRead == PermissionStatus.Granted)
                    {
                        if (DeviceInfo.Version.Major >= 10)
                        {
                            var downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                            string filePath = Path.Combine(downloadsFolder, documento.NombreArchivo);
                            var fileStream = await gridFSBucket.OpenDownloadStreamAsync(documento.IdDocumento);

                            using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                            {
                                await fileStream.CopyToAsync(file);
                            }

                            await Launcher.OpenAsync(filePath);
                            await Shell.Current.DisplayAlert("Descargado", "Se ha descargado el archivo en su dispositivo", "Aceptar");
                        }
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Dispositivo no compatible por ahora", "Aceptar");
                    }
                }
                else if (DeviceInfo.Platform == DevicePlatform.WinUI)
                {
                    var downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                    var filePath = Path.Combine(downloadsFolder, documento.NombreArchivo);
                    var fileStream = await gridFSBucket.OpenDownloadStreamAsync(documento.IdDocumento);

                    using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await fileStream.CopyToAsync(file);
                    }

                    await Launcher.OpenAsync(filePath);
                    await Shell.Current.DisplayAlert("Descargado", "Se ha descargado el archivo en su dispositivo", "Aceptar");
                }
                else
                {
                    throw new NotSupportedException("Plataforma no soportada.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la descarga: {ex.Message}");
            }
        }

        /// <summary>
        /// Inicia el monitoreo de cambios en tiempo real para un técnico.
        /// </summary>
        internal void IniciarMonitoreoTecnico()
        {
            if (string.IsNullOrEmpty(usuario.Id))
            {
                Console.WriteLine("El usuario no ha iniciado sesión.");
                return;
            }

            monitoreoTecnico = new MonitoreoTecnico(database, usuario.Id);
            monitoreoTecnico.IniciarMonitoreo();
            monitoreoTecnico.DatosActualizadosTiempoRealTecnico -= OnDatosActualizadosTiempoRealTecnico;
            monitoreoTecnico.DatosActualizadosTiempoRealTecnico += OnDatosActualizadosTiempoRealTecnico;
            Console.WriteLine("Monitoreo para técnico iniciado.");
        }

        /// <summary>
        /// Inicia el monitoreo de cambios en tiempo real para un usuario regular.
        /// </summary>
        internal void IniciarMonitoreoUsuario()
        {
            if (string.IsNullOrEmpty(usuario.Id))
            {
                Console.WriteLine("El usuario no ha iniciado sesión.");
                return;
            }

            monitoreoUsuario = new MonitoreoUsuario(database, usuario.Id);
            monitoreoUsuario.DatosActualizadosTiempoReal -= OnDatosActualizadosTiempoReal;
            monitoreoUsuario.DatosActualizadosTiempoReal += OnDatosActualizadosTiempoReal;
            monitoreoUsuario.IniciarMonitoreo();
            System.Diagnostics.Debug.WriteLine("Este es un mensaje de depuración.");
        }

        /// <summary>
        /// Detiene cualquier monitoreo en tiempo real activo.
        /// </summary>
        internal void DetenerMonitoreo()
        {
            monitoreoTecnico?.DetenerMonitoreo();
            monitoreoUsuario?.DetenerMonitoreo();
            Console.WriteLine("Monitoreo detenido.");
        }

        /// <summary>
        /// Evento que maneja las actualizaciones en tiempo real para usuarios regulares.
        /// </summary>
        private void OnDatosActualizadosTiempoReal(object sender, EventArgs e)
        {
            shell.ActualizarTicketsTiempoReal();
        }

        /// <summary>
        /// Evento que maneja las actualizaciones en tiempo real para técnicos.
        /// </summary>
        private void OnDatosActualizadosTiempoRealTecnico(object sender, EventArgs e)
        {
            shell.ActualizarTicketsTiempoRealTecnico();
        }

        /// <summary>
        /// Cierra todos los tickets relacionados con un IDTicketPadre específico.
        /// </summary>
        /// <param name="idTicketPadre">El ID del ticket padre.</param>
        public async Task CerrarTicketsIDTicketPadre(string idTicketPadre)
        {
            var collection = database.GetCollection<Ticket>("tickets");
            var filterPadre = Builders<Ticket>.Filter.Eq(t => t.IDTicketPadre, idTicketPadre);
            var updatePadre = Builders<Ticket>.Update.Set(t => t.Estado, "Cerrado");
            await collection.UpdateManyAsync(filterPadre, updatePadre);

            var filterPadrePrincipal = Builders<Ticket>.Filter.Eq(t => t.IdTicket, ObjectId.Parse(idTicketPadre));
            var updatePadrePrincipal = Builders<Ticket>.Update.Set(t => t.Estado, "Cerrado");
            await collection.UpdateOneAsync(filterPadrePrincipal, updatePadrePrincipal);
        }

        /// <summary>
        /// Cierra un ticket y sus tickets hijos relacionados.
        /// </summary>
        /// <param name="idTicket">El ID del ticket a cerrar.</param>
        public async Task CerrarTicketsIDTicket(ObjectId idTicket)
        {
            var collection = database.GetCollection<Ticket>("tickets");
            var filterHijos = Builders<Ticket>.Filter.Eq(t => t.IDTicketPadre, idTicket.ToString());
            var updateHijos = Builders<Ticket>.Update.Set(t => t.Estado, "Cerrado");
            await collection.UpdateManyAsync(filterHijos, updateHijos);

            var updateTicketPadre = Builders<Ticket>.Update.Set(t => t.Estado, "Cerrado");
            await collection.UpdateOneAsync(t => t.IdTicket == idTicket, updateTicketPadre);
        }

        /// <summary>
        /// Obtiene una lista de tickets que no tienen un técnico asignado.
        /// </summary>
        /// <returns>Lista de tickets sin asignar.</returns>
        internal async Task<List<Ticket>> ObtenerTicketsDeSinAsignarAsync()
        {
            var ticketsCollection = database.GetCollection<Ticket>("tickets");
            string stringnull = null;
            var filter = Builders<Ticket>.Filter.Eq("IDTecnico", stringnull);
            var sort = Builders<Ticket>.Sort.Ascending(t => t.Estado);
            return await ticketsCollection.Find(filter).Sort(sort).ToListAsync();
        }

        /// <summary>
        /// Actualiza el ID del técnico asignado para un ticket específico.
        /// </summary>
        /// <param name="ticketId">El ID del ticket a actualizar.</param>
        /// <returns>True si la actualización fue exitosa, de lo contrario, False.</returns>
        public async Task<bool> ActualizarIDTecnicoAsync(ObjectId ticketId)
        {
            try
            {
                var ticketsCollection = database.GetCollection<Ticket>("tickets");
                var filter = Builders<Ticket>.Filter.Eq("_id", ticketId);
                var update = Builders<Ticket>.Update.Set("IDTecnico", usuario.Id);
                var result = await ticketsCollection.UpdateOneAsync(filter, update);

                return result.MatchedCount > 0;
            }
            catch (COMException ex)
            {
                Console.WriteLine($"COM Exception: {ex.Message}");
                Console.WriteLine($"Código de error: {ex.ErrorCode}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Actualiza un ticket con la solución proporcionada y adjunta documentos seleccionados subiéndolos a GridFS.
        /// </summary>
        /// <param name="idTicket">ID del ticket a actualizar.</param>
        /// <param name="solucion">Texto de la solución proporcionada.</param>
        /// <param name="documentosSeleccionados">Lista de documentos seleccionados para adjuntar.</param>
        internal async void ActualizacionTecnicoAsync(ObjectId idTicket, string solucion, List<Documento> documentosSeleccionados)
        {
            try
            {
                var gridFSBucket = new GridFSBucket(database);
                var documentosAdjuntos = new List<BsonDocument>();

                foreach (var documento in documentosSeleccionados)
                {
                    using (var fileStream = File.OpenRead(documento.RutaArchivo))
                    {
                        var fileId = await gridFSBucket.UploadFromStreamAsync(documento.NombreArchivo, fileStream);

                        var docMetadata = new BsonDocument
                {
                    { "IdDocumento", fileId },
                    { "NombreArchivo", documento.NombreArchivo },
                    { "TipoArchivo", documento.TipoArchivo }
                };

                        documentosAdjuntos.Add(docMetadata);
                    }
                }

                var ticketsCollection = database.GetCollection<Ticket>("tickets");
                var filter = Builders<Ticket>.Filter.Eq("_id", idTicket);

                var update = Builders<Ticket>.Update
                    .Set("solucion", solucion)
                    .Set("documentosAdjuntos", documentosAdjuntos);

                await ticketsCollection.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el ticket: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene una lista de usuarios desde la base de datos, convirtiendo documentos BSON en objetos Usuario.
        /// </summary>
        /// <returns>Lista de usuarios encontrados en la base de datos.</returns>
        internal async Task<List<Usuario>> ListaUsuarios()
        {
            try
            {
                if (Conexion())
                {
                    var collectionUsuarios = database.GetCollection<BsonDocument>("usuarios");

                    var filtro = Builders<BsonDocument>.Filter.In("Rol", new[] { 1, 2 });

                    var usuariosDocs = await collectionUsuarios.FindAsync(filtro);

                    return usuariosDocs.ToList().Select(doc => new Usuario
                    {
                        Id = doc["_id"].ToString(),
                        Nombre = doc["Usuario"].AsString,
                        Rol = doc["Rol"].AsInt32
                    }).ToList();
                }
                else
                {
                    return new List<Usuario>();
                }
            }
            catch (Exception ex)
            {
                return new List<Usuario>();
            }
        }

        /// <summary>
        /// Elimina un usuario específico y sus datos relacionados de la base de datos.
        /// </summary>
        /// <param name="usuario">Usuario a eliminar, con su ID proporcionado.</param>
        internal async Task EliminarDatosUsuario(Usuario usuario)
        {
            try
            {
                if (Conexion())
                {
                    var collectionUsuarios = database.GetCollection<BsonDocument>("usuarios");
                    var filtroUsuario = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(usuario.Id));
                    var resultadoUsuario = await collectionUsuarios.DeleteOneAsync(filtroUsuario);

                    if (resultadoUsuario.DeletedCount == 0)
                    {
                        Console.WriteLine("No se encontró un usuario con ese ID.");
                        return;
                    }

                    var collectionConfiguracion = database.GetCollection<BsonDocument>("configuracion");
                    var filtroConfiguracion = Builders<BsonDocument>.Filter.Eq("IdUsuario", usuario.Id);
                    var resultadoConfiguracion = await collectionConfiguracion.DeleteManyAsync(filtroConfiguracion);

                    Console.WriteLine($"Usuario eliminado: {resultadoUsuario.DeletedCount}");
                    Console.WriteLine($"Datos relacionados eliminados: {resultadoConfiguracion.DeletedCount}");
                }
                else
                {
                    Console.WriteLine("No se pudo conectar a la base de datos.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el usuario y los datos relacionados: {ex.Message}");
            }
        }

    }


}

