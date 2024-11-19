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
            return Builders<ChangeStreamDocument<BsonDocument>>.Filter.Eq($"fullDocument.{campoFiltro}", filtroId);
        }
    }
}
