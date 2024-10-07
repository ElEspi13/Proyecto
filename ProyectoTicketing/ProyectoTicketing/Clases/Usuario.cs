using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoTicketing.Clases
{
    /// <summary>
    /// Representa un usuario en la aplicación.
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Rol del usuario.
        /// </summary>
        public int Rol { get; set; }

        /// <summary>
        /// Representación en cadena del usuario (su nombre).
        /// </summary>
        /// <returns>Nombre del usuario.</returns>
        public override string ToString()
        {
            return Nombre;
        }
    }
}
