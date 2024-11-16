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

        private async Task MonitorearCambiosAsync(CancellationToken token)
        {
            var collection = database.GetCollection<BsonDocument>("tickets");
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>()
                .Match(Builders<ChangeStreamDocument<BsonDocument>>.Filter
                    .Or(
                        Builders<ChangeStreamDocument<BsonDocument>>.Filter.Eq($"fullDocument.{campoFiltro}", filtroId),
                        Builders<ChangeStreamDocument<BsonDocument>>.Filter.Eq($"fullDocument.{campoFiltro}", BsonNull.Value)
                    ));

            using var cursor = await collection.WatchAsync(pipeline, cancellationToken: token);

            try
            {
                while (await cursor.MoveNextAsync(token))
                {
                    foreach (var change in cursor.Current)
                    {
                        Console.WriteLine($"Cambio detectado: {change.FullDocument}");
                        await CargarDatosAsync(); // Actualiza la interfaz u otra acción necesaria
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Monitoreo cancelado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error durante el monitoreo: {ex.Message}");
            }
        }

    }
}
