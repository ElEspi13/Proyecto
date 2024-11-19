using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace ProyectoTicketing.Clases
{
    internal abstract class MonitoreoBase
    {
        protected IMongoDatabase database;
        protected CancellationTokenSource cts;
        protected string filtroId; // ID específico del técnico o usuario
        protected string campoFiltro; // Campo a filtrar (IDUsuario o IDTecnico)

        protected MonitoreoBase(IMongoDatabase database, string filtroId, string campoFiltro)
        {
            this.database = database;
            this.filtroId = filtroId;
            this.campoFiltro = campoFiltro;
            cts = new CancellationTokenSource();
        }

        public void IniciarMonitoreo()
        {
            Task.Run(() => MonitorearCambiosAsync(cts.Token), cts.Token);
        }

        public void DetenerMonitoreo()
        {
            cts.Cancel();
        }

        protected abstract Task CargarDatosAsync();

        protected abstract FilterDefinition<ChangeStreamDocument<BsonDocument>> ConfigurarFiltro();
        private async Task MonitorearCambiosAsync(CancellationToken token)
        {
            var collection = database.GetCollection<BsonDocument>("tickets");

            // Usa el filtro configurado por la clase derivada
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>()
                .Match(ConfigurarFiltro());

            using var cursor = await collection.WatchAsync(pipeline, cancellationToken: token);

            try
            {
                while (await cursor.MoveNextAsync(token))
                {
                    foreach (var change in cursor.Current)
                    {
                        Console.WriteLine($"Cambio detectado: {change.FullDocument}");
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
