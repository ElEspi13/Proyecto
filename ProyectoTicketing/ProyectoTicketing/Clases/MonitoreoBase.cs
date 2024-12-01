using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace ProyectoTicketing.Clases
{
    /// <summary>
    /// Clase base para el monitoreo de cambios en la base de datos.
    /// </summary>
    internal abstract class MonitoreoBase
    {
        protected IMongoDatabase database;
        protected CancellationTokenSource cts;
        protected string filtroId;
        protected string campoFiltro;

        /// <summary>
        /// Constructor que inicializa la base de datos y el filtro de monitoreo.
        /// </summary>
        /// <param name="database">Instancia de la base de datos MongoDB.</param>
        /// <param name="filtroId">ID del técnico o usuario a filtrar.</param>
        /// <param name="campoFiltro">Campo en el que se aplicará el filtro (IDUsuario o IDTecnico).</param>
        protected MonitoreoBase(IMongoDatabase database, string filtroId, string campoFiltro)
        {
            this.database = database;
            this.filtroId = filtroId;
            this.campoFiltro = campoFiltro;
            cts = new CancellationTokenSource();
        }

        /// <summary>
        /// Inicia el monitoreo de cambios.
        /// </summary>
        public void IniciarMonitoreo()
        {
            Task.Run(() => MonitorearCambiosAsync(cts.Token), cts.Token);
        }

        /// <summary>
        /// Detiene el monitoreo de cambios.
        /// </summary>
        public void DetenerMonitoreo()
        {
            cts.Cancel();
        }

        /// <summary>
        /// Método abstracto que debe implementar la clase derivada para cargar los datos.
        /// </summary>
        protected abstract Task CargarDatosAsync();

        /// <summary>
        /// Método abstracto que debe implementar la clase derivada para configurar el filtro de monitoreo.
        /// </summary>
        protected abstract FilterDefinition<ChangeStreamDocument<BsonDocument>> ConfigurarFiltro();

        private async Task MonitorearCambiosAsync(CancellationToken token)
        {
            var collection = database.GetCollection<BsonDocument>("tickets");

            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>()
                .Match(ConfigurarFiltro());

            using var cursor = await collection.WatchAsync(pipeline, cancellationToken: token);

            try
            {
                while (await cursor.MoveNextAsync(token))
                {
                    foreach (var change in cursor.Current)
                    {
                        System.Diagnostics.Debug.WriteLine($"Cambio detectado: {change.FullDocument}");
                        await CargarDatosAsync();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Monitoreo cancelado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error durante el monitoreo: {ex.Message}");
            }
        }
    }
}
