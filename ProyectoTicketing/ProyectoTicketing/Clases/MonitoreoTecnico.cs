using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace ProyectoTicketing.Clases
{
    /// <summary>
    /// Monitoreo especializado para técnicos, derivado de la clase base MonitoreoBase.
    /// Esta clase permite monitorear cambios en la base de datos relacionados con un técnico específico.
    /// </summary>
    internal class MonitoreoTecnico : MonitoreoBase
    {
        /// <summary>
        /// Semáforo para garantizar la sincronización del acceso a los datos en situaciones concurrentes.
        /// </summary>
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Evento que se activa cuando se actualizan los datos en tiempo real para el técnico.
        /// </summary>
        public event EventHandler DatosActualizadosTiempoRealTecnico;

        /// <summary>
        /// Constructor de la clase MonitoreoTecnico.
        /// </summary>
        /// <param name="database">Base de datos MongoDB.</param>
        /// <param name="idTecnico">ID del técnico cuyo monitoreo se desea realizar.</param>
        public MonitoreoTecnico(IMongoDatabase database, string idTecnico)
            : base(database, idTecnico, "IDTecnico")
        {
        }

        /// <summary>
        /// Carga los datos actualizados del técnico. Este método se ejecuta de manera asincrónica y notifica
        /// a través del evento DatosActualizadosTiempoRealTecnico cuando los datos se han actualizado.
        /// </summary>
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

        /// <summary>
        /// Configura el filtro para monitorear los cambios en los documentos de MongoDB, específicamente los
        /// documentos que han sido actualizados y contienen el ID del técnico en los campos correspondientes.
        /// </summary>
        /// <returns>Filtro para la transmisión de cambios en MongoDB.</returns>
        protected override FilterDefinition<ChangeStreamDocument<BsonDocument>> ConfigurarFiltro()
        {
            return Builders<ChangeStreamDocument<BsonDocument>>.Filter.And(
                    Builders<ChangeStreamDocument<BsonDocument>>.Filter.Exists($"fullDocument.{campoFiltro}"),
                    Builders<ChangeStreamDocument<BsonDocument>>.Filter.Eq($"fullDocument.{campoFiltro}", BsonNull.Value)
                
    );
        }
    }
}
