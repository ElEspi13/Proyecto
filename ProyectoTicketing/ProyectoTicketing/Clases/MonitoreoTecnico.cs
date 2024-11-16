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
        public MonitoreoTecnico(IMongoDatabase database, string idTecnico)
            : base(database, idTecnico, "IDTecnico")
        {
        }

        protected override async Task CargarDatosAsync()
        {
            Console.WriteLine("Actualizando datos del técnico...");
            // Aquí puedes implementar la lógica específica para cargar los datos del técnico.
        }
    }
}
