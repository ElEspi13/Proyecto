using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace ProyectoTicketing.Clases
{
    /// <summary>
    /// Monitoreo especializado para usuarios, derivado de la clase base MonitoreoBase.
    /// Esta clase permite monitorear cambios en la base de datos relacionados con un usuario específico.
    /// </summary>
    internal class MonitoreoUsuario : MonitoreoBase
    {
        /// <summary>
        /// Evento que se activa cuando los datos del usuario se actualizan en tiempo real.
        /// </summary>
        public event EventHandler DatosActualizadosTiempoReal;

        /// <summary>
        /// Constructor de la clase MonitoreoUsuario.
        /// </summary>
        /// <param name="database">Base de datos MongoDB.</param>
        /// <param name="idUsuario">ID del usuario cuyo monitoreo se desea realizar.</param>
        public MonitoreoUsuario(IMongoDatabase database, string idUsuario)
            : base(database, idUsuario, "IDUsuario")
        {
        }

        /// <summary>
        /// Carga los datos actualizados del usuario. Este método se ejecuta de manera asincrónica y notifica
        /// a través del evento DatosActualizadosTiempoReal cuando los datos se han actualizado.
        /// </summary>
        protected override async Task CargarDatosAsync()
        {
            System.Diagnostics.Debug.WriteLine("Este es un mensaje de depuración.");
            // Invocar el evento para notificar que los datos han sido actualizados
            DatosActualizadosTiempoReal?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Configura el filtro para monitorear los cambios en los documentos de MongoDB, específicamente los
        /// documentos que han sido actualizados con el ID del usuario o aquellos cuyo estado sea "Cerrado".
        /// </summary>
        /// <returns>Filtro para la transmisión de cambios en MongoDB.</returns>
        protected override FilterDefinition<ChangeStreamDocument<BsonDocument>> ConfigurarFiltro()
        {

            var filtroEstado = Builders<ChangeStreamDocument<BsonDocument>>.Filter.Eq("updateDescription.updatedFields.Estado", "Cerrado");


            var filtroIdUsuario = Builders<ChangeStreamDocument<BsonDocument>>.Filter.Eq($"fullDocument.{campoFiltro}", filtroId);


            return filtroIdUsuario | filtroEstado;
        }
    }
}
