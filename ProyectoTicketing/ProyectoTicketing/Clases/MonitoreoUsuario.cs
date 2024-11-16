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
        public MonitoreoUsuario(IMongoDatabase database, string idUsuario)
            : base(database, idUsuario, "IDUsuario")
        {
        }

        protected override async Task CargarDatosAsync()
        {
            Console.WriteLine("Actualizando datos del usuario...");
            // Aquí puedes implementar la lógica específica para cargar los datos del usuario.
        }
    }
}
