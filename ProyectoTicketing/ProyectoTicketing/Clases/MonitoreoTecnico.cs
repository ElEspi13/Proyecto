using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoTicketing.Clases
{
    internal class MonitoreoTecnico : MonitoreoBase
    {
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        public event EventHandler DatosActualizadosTiempoRealTecnico;
        public MonitoreoTecnico(IMongoDatabase database, string idTecnico)
            : base(database, idTecnico, "IDTecnico")
        {
        }

        protected override async Task CargarDatosAsync()
        {
            await semaphore.WaitAsync();
            try
            {
                Console.WriteLine("Cargando datos del técnico...");
                DatosActualizadosTiempoRealTecnico?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                semaphore.Release();
            }
        }
        protected override FilterDefinition<ChangeStreamDocument<BsonDocument>> ConfigurarFiltro()
        {
            return Builders<ChangeStreamDocument<BsonDocument>>.Filter.Eq($"updateDescription.updatedFields.{campoFiltro}", filtroId);
        }
    }
}
