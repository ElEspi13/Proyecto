
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoTicketing.Clases
{
    internal class MonitoreoUsuario : MonitoreoBase
    {
        public event EventHandler DatosActualizadosTiempoReal;
        public MonitoreoUsuario(IMongoDatabase database, string idUsuario)
            : base(database, idUsuario, "IDUsuario")
        {
        }

        protected override async Task CargarDatosAsync()
        {
            System.Diagnostics.Debug.WriteLine("Este es un mensaje de depuración.");
            DatosActualizadosTiempoReal?.Invoke(this, EventArgs.Empty);
        }
        protected override FilterDefinition<ChangeStreamDocument<BsonDocument>> ConfigurarFiltro()
        {
            var filtroEstado = Builders<ChangeStreamDocument<BsonDocument>>.Filter.Eq("updateDescription.updatedFields.Estado", "Cerrado");

            // Filtro para verificar si el "IDUsuario" coincide con el filtroId
            var filtroIdUsuario = Builders<ChangeStreamDocument<BsonDocument>>.Filter.Eq($"fullDocument.{campoFiltro}", filtroId);
            return filtroIdUsuario | filtroEstado;
        }
    }
}
