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
    internal class BBDD
    {
        private MonitoreoTecnico monitoreoTecnico;
        private MonitoreoUsuario monitoreoUsuario;
        private MongoClient client;
        private IMongoDatabase database;
        private Usuario usuario = new Usuario();
        private GridFSBucket gridFSBucket;
        private AppShell shell;
        private string connectionString = "mongodb://alvaroespi13:TQqtYSj3BwZbGOw7@cluster0-shard-00-00.b0c6k.mongodb.net:27017,cluster0-shard-00-01.b0c6k.mongodb.net:27017,cluster0-shard-00-02.b0c6k.mongodb.net:27017/ticketingDB?authSource=admin&ssl=true\r\n";
        public BBDD(AppShell shell)
        {
            this.shell = shell;

        }

        internal void CerrarSesion()
        {
            usuario = new Usuario();
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

        internal async void RegistrarUsuario(string nombreUsuario, string contrasena, int Rol)
        {
            try
            {
                // Verificar conexión con la base de datos
                if (Conexion())
                {
                    var collectionUsuarios = database.GetCollection<BsonDocument>("usuarios");

                    // Verificar si ya existe un usuario con el mismo nombre
                    var filtro = Builders<BsonDocument>.Filter.Eq("Usuario", nombreUsuario);
                    var usuarioExistente = collectionUsuarios.Find(filtro).FirstOrDefault();

                    if (usuarioExistente != null)
                    {
                        await Shell.Current.DisplayAlert("Error", "El Nombre de Usuario ya esta siendo usado", "OK");
                        return; // Salir del método si el usuario ya existe
                    }
                    else
                    {
                        // Obtener la colección de usuarios
                        collectionUsuarios = database.GetCollection<BsonDocument>("usuarios");
                        if (Rol == 0)
                        {
                            // Crear un nuevo documento Bson para el usuario
                            var usuarioDoc = new BsonDocument

                    {
                        { "Usuario", nombreUsuario },
                        { "Contrasena", contrasena },
                        { "Rol", 2 } // Asigna un rol por defecto; ajusta según sea necesario
                    };

                            // Insertar el documento en la colección
                            collectionUsuarios.InsertOne(usuarioDoc);
                        }
                        else if (Rol == 1)
                        {

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
                    var docMetadata = new Documento(fileId, documento.NombreArchivo, documento.TipoArchivo, documento.RutaArchivo);

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
                { "IDTicketPadre", ticket.IDTicketPadre == null ? BsonNull.Value : ticket.IDTicketPadre },
                { "documentosAdjuntos", new BsonArray(documentosAdjuntos ?? new List<BsonDocument>()) }
            };



            await database.GetCollection<BsonDocument>("tickets").InsertOneAsync(ticketDoc);
        }


        public async Task<List<Ticket>> ObtenerTicketsDeUsuarioAsync()
        {
            // Obtener la colección de tickets
            var ticketsCollection = database.GetCollection<Ticket>("tickets");

            // Crear el filtro para buscar los tickets del usuario por su ID
            var filter = Builders<Ticket>.Filter.Or(
                Builders<Ticket>.Filter.Eq("IDUsuario", usuario.Id),  // Filtro por IDUsuario
                Builders<Ticket>.Filter.Eq("IDTecnico", usuario.Id)    // Filtro por IDTecnico
            );

            // Crear el criterio de ordenación: "Abierto" primero, "Cerrado" después
            var sort = Builders<Ticket>.Sort.Ascending(t => t.Estado);

            // Realizar la consulta con el filtro y la ordenación
            List<Ticket> tickets = await ticketsCollection.Find(filter).Sort(sort).ToListAsync();

            return tickets;
        }

        public async Task DescargarDocumentoAsync(Documento documento)
        {
            try
            {
                // Verifica la plataforma en la que se ejecuta la aplicación
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    // Solicita permisos de almacenamiento en Android
                    var statusWrite = await Permissions.RequestAsync<Permissions.StorageWrite>();
                    var statusRead = await Permissions.RequestAsync<Permissions.StorageRead>();

                    // Comprobar si los permisos fueron concedidos
                    if (statusWrite == PermissionStatus.Granted && statusRead == PermissionStatus.Granted)
                    {
                        // Verificar si estamos en Android 10 o superior (API 29+)
                        if (DeviceInfo.Version.Major >= 10 ) // Android 10 y superior
                        {

                            var downloadsFolder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");
                            string filePath = Path.Combine(downloadsFolder, documento.NombreArchivo);

                            // Acceder al archivo en GridFS
                            var fileStream = await gridFSBucket.OpenDownloadStreamAsync(documento.IdDocumento);

                            // Guardar el archivo en la carpeta de descargas
                            using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                            {
                                await fileStream.CopyToAsync(file);
                            }

                            // Abrir el archivo usando el Launcher
                            await Launcher.OpenAsync(filePath);
                            await Shell.Current.DisplayAlert("Descargado", "Se ha descargado el archivo en su dispositivo", "Aceptar");
                        }

                    }
                 
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Dispositivo no compatible por ahora", "Aceptar");

                    }
                }
                else if (DeviceInfo.Platform == DevicePlatform.WinUI) // Windows
                {
                    // Para Windows, obtenemos el directorio de "Descargas"
                    var downloadsFolder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");

                    // Ruta completa para guardar el archivo
                    var filePath = Path.Combine(downloadsFolder, documento.NombreArchivo);

                    // Acceder al archivo en GridFS
                    var fileStream = await gridFSBucket.OpenDownloadStreamAsync(documento.IdDocumento);

                    // Guardar el archivo en la carpeta de descargas
                    using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await fileStream.CopyToAsync(file);
                    }

                    // Abrir el archivo usando el Launcher
                    await Launcher.OpenAsync(filePath);
                    await Shell.Current.DisplayAlert("Descargado", "Se ha descargado el archivo en su dispositivo", "Aceptar");
                }
                else
                {
                    // Si la plataforma no es soportada, lanzamos una excepción
                    throw new NotSupportedException("Plataforma no soportada.");
                }

            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error en la descarga: {ex.Message}");

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
            monitoreoTecnico.DatosActualizadosTiempoRealTecnico -= OnDatosActualizadosTiempoRealTecnico;
            monitoreoTecnico.DatosActualizadosTiempoRealTecnico += OnDatosActualizadosTiempoRealTecnico;
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
            monitoreoUsuario.DatosActualizadosTiempoReal -= OnDatosActualizadosTiempoReal;
            monitoreoUsuario.DatosActualizadosTiempoReal += OnDatosActualizadosTiempoReal;
            monitoreoUsuario.IniciarMonitoreo();
            System.Diagnostics.Debug.WriteLine("Este es un mensaje de depuración.");

        }

        internal void DetenerMonitoreo()
        {
            monitoreoTecnico?.DetenerMonitoreo();
            monitoreoUsuario?.DetenerMonitoreo();
            Console.WriteLine("Monitoreo detenido.");
        }
        private void OnDatosActualizadosTiempoReal(object sender, EventArgs e)
        {
            shell.ActualizarTicketsTiempoReal();

        }

        private void OnDatosActualizadosTiempoRealTecnico(object sender, EventArgs e)
        {
            shell.ActualizarTicketsTiempoRealTecnico();

        }


        public async Task CerrarTicketsIDTicketPadre(string idTicketPadre)
        {
            var collection = database.GetCollection<Ticket>("tickets"); // Asegurándonos de que sea la colección "tickets"

            // 1. Buscar todos los tickets relacionados con el mismo IDTicketPadre
            var filterPadre = Builders<Ticket>.Filter.Eq(t => t.IDTicketPadre, idTicketPadre); // Comparar con el IDTicketPadre que es un string
            var updatePadre = Builders<Ticket>.Update.Set(t => t.Estado, "Cerrado");


            // 2. Realizar la actualización masiva de los tickets hijos
            var resultHijos = await collection.UpdateManyAsync(filterPadre, updatePadre);


            // 3. Actualizar el ticket padre
            var filterPadrePrincipal = Builders<Ticket>.Filter.Eq(t => t.IdTicket, ObjectId.Parse(idTicketPadre)); // Convertir el IDTicketPadre a ObjectId para la comparación
            var updatePadrePrincipal = Builders<Ticket>.Update.Set(t => t.Estado, "Cerrado");



            var resultPadre = await collection.UpdateOneAsync(filterPadrePrincipal, updatePadrePrincipal);
        }

        public async Task CerrarTicketsIDTicket(ObjectId idTicket)
        {
            var collection = database.GetCollection<Ticket>("tickets"); // Asegurándonos de que sea la colección "tickets"

            // 1. Buscar todos los tickets hijos que tienen como IDTicketPadre este IDTicket
            var filterHijos = Builders<Ticket>.Filter.Eq(t => t.IDTicketPadre, idTicket.ToString()); // Convertir idTicket a string para la comparación con IDTicketPadre
            var updateHijos = Builders<Ticket>.Update.Set(t => t.Estado, "Cerrado");

            // Mensaje para verificar el filtro de IDTicketPadre


            // 2. Diagnóstico: Verificar cuántos tickets hijos se encuentran
            var hijos = await collection.Find(filterHijos).ToListAsync(); // Obtener los hijos para imprimir información
            
            // 3. Actualizar los tickets hijos si existen
            var resultHijos = await collection.UpdateManyAsync(filterHijos, updateHijos);
            

            // 4. Actualizar el estado del ticket principal a "Cerrado" si tiene hijos
            var updateTicketPadre = Builders<Ticket>.Update.Set(t => t.Estado, "Cerrado");
            var resultPadre = await collection.UpdateOneAsync(t => t.IdTicket == idTicket, updateTicketPadre);
        }

        internal async Task<List<Ticket>> ObtenerTicketsDeSinAsignarAsync()
        {
            // Obtener la colección de tickets
            var ticketsCollection = database.GetCollection<Ticket>("tickets");
            string stringnull = null;

            // Crear el filtro para buscar los tickets del usuario por su ID
            var filter = Builders<Ticket>.Filter.Eq("IDTecnico", stringnull);

            // Crear la ordenación: "Abierto" primero, "Cerrado" después
            var sort = Builders<Ticket>.Sort.Ascending(t => t.Estado);

            // Realizar la consulta con el filtro y la ordenación
            List<Ticket> tickets = await ticketsCollection.Find(filter).Sort(sort).ToListAsync();

            return tickets;
        }

        public async Task<bool> ActualizarIDTecnicoAsync(ObjectId ticketId)
        {

            try {
                // Obtener la colección de tickets
                var ticketsCollection = database.GetCollection<Ticket>("tickets");

                // Crear el filtro para encontrar el ticket con IDTecnico igual a null
                var filter = Builders<Ticket>.Filter.Eq("_id", ticketId);

                // Crear el update para cambiar el IDTecnico al ID del técnico
                var update = Builders<Ticket>.Update.Set("IDTecnico", usuario.Id);

                // Realizar la actualización
                var result = await ticketsCollection.UpdateOneAsync(filter, update);

                if (result.MatchedCount==0)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch (COMException ex)
            {
                // Capturar la excepción COMException
                Console.WriteLine($"COM Exception: {ex.Message}");
                Console.WriteLine($"Código de error: {ex.ErrorCode}");
                // Otras acciones necesarias, como registrar el error en un log
                return false;
            }
            catch (Exception ex)
            {
                // Capturar cualquier otra excepción general
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }

        }

        internal async void ActualizacionTecnicoAsync(ObjectId idTicket, string solucion, List<Documento> documentosSeleccionados)
        {
            
                try
                {
                    // Crear un bucket para manejar GridFS
                    var gridFSBucket = new GridFSBucket(database);

                    // Subir documentos a GridFS y almacenar metadatos
                    var documentosAdjuntos = new List<BsonDocument>();
                    foreach (var documento in documentosSeleccionados)
                    {
                        using (var fileStream = File.OpenRead(documento.RutaArchivo))
                        {
                            // Subir el archivo a GridFS
                            var fileId = await gridFSBucket.UploadFromStreamAsync(documento.NombreArchivo, fileStream);

                            // Crear metadatos del documento
                            var docMetadata = new BsonDocument
                            {
                                { "IdDocumento", fileId },
                                { "NombreArchivo", documento.NombreArchivo },
                                { "TipoArchivo", documento.TipoArchivo }
                            };

                            // Agregar los metadatos a la lista
                            documentosAdjuntos.Add(docMetadata);
                        }
                    }

                    // Obtener la colección de tickets
                    var ticketsCollection = database.GetCollection<Ticket>("tickets");

                    // Crear el filtro para encontrar el ticket por su ID
                    var filter = Builders<Ticket>.Filter.Eq("_id", idTicket);

                    // Crear el documento de actualización
                    var update = Builders<Ticket>.Update
                        .Set("solucion", solucion) // Actualizar la solución
                        .Set("documentosAdjuntos", documentosAdjuntos); // Actualizar documentos adjuntos

                    // Ejecutar la actualización
                    var result = await ticketsCollection.UpdateOneAsync(filter, update);

                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    Console.WriteLine($"Error al actualizar el ticket: {ex.Message}");
                }
            }

        internal async Task<List<Usuario>> ListaUsuarios()
        {
            try
            {
                // Verificar conexión con la base de datos
                if (Conexion())
                {
                    // Obtener la colección de usuarios
                    var collectionUsuarios = database.GetCollection<BsonDocument>("usuarios");

                    // Consultar todos los documentos de la colección
                    var usuariosDocs = await collectionUsuarios.FindAsync(Builders<BsonDocument>.Filter.Empty);

                    // Convertir los documentos Bson en objetos Usuario
                    var usuarios = usuariosDocs.ToList().Select(doc => new Usuario
                    {
                        Id = doc["_id"].ToString(),
                        Nombre = doc["Usuario"].AsString,
                        Rol = doc["Rol"].AsInt32
                    }).ToList();

                    return usuarios;
                }
                else
                {
                    Console.WriteLine("No se pudo conectar a la base de datos.");
                    return new List<Usuario>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la lista de usuarios: {ex.Message}");
                return new List<Usuario>();
            }
        }

        internal async Task EliminarDatosUsuario(Usuario usuario)
        {

            try
            {
                // Verificar conexión con la base de datos
                if (Conexion())
                {
                    // Obtener la colección de usuarios
                    var collectionUsuarios = database.GetCollection<BsonDocument>("usuarios");

                    // Crear un filtro para encontrar el usuario por su ID
                    var filtroUsuario = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(usuario.Id));

                    // Eliminar el usuario de la colección "usuarios"
                    var resultadoUsuario = await collectionUsuarios.DeleteOneAsync(filtroUsuario);

                    if (resultadoUsuario.DeletedCount == 0)
                    {
                        Console.WriteLine("No se encontró un usuario con ese ID.");
                        return;
                    }

                    // Eliminar datos relacionados en la colección "configuracion"
                    var collectionConfiguracion = database.GetCollection<BsonDocument>("configuracion");
                    var filtroConfiguracion = Builders<BsonDocument>.Filter.Eq("IdUsuario", usuario.Id);

                    var resultadoConfiguracion = await collectionConfiguracion.DeleteManyAsync(filtroConfiguracion);

                    // Mostrar en consola los resultados
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

