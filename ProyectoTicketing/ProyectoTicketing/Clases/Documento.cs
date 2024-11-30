using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoTicketing.Clases
{
    /// <summary>
    /// Representa un documento en el sistema.
    /// </summary>
    public class Documento
    {
        /// <summary>
        /// ID del archivo en GridFS.
        /// </summary>
        public ObjectId IdDocumento { get; set; }

        /// <summary>
        /// Nombre del archivo.
        /// </summary>
        public string NombreArchivo { get; set; }

        /// <summary>
        /// Tipo MIME del archivo (por ejemplo, application/pdf).
        /// </summary>
        public string TipoArchivo { get; set; }

        /// <summary>
        /// Ruta del archivo en el sistema.
        /// </summary>
        public string RutaArchivo { get; set; }

        /// <summary>
        /// Constructor que inicializa un objeto Documento con los parámetros especificados.
        /// </summary>
        /// <param name="idDocumento">ID del documento en GridFS.</param>
        /// <param name="nombreArchivo">Nombre del archivo.</param>
        /// <param name="tipoArchivo">Tipo MIME del archivo.</param>
        /// <param name="rutaArchivo">Ruta del archivo.</param>
        public Documento(ObjectId idDocumento, string nombreArchivo, string tipoArchivo, string rutaArchivo)
        {
            IdDocumento = idDocumento;
            NombreArchivo = nombreArchivo;
            TipoArchivo = tipoArchivo;
            RutaArchivo = rutaArchivo;
        }

        /// <summary>
        /// Devuelve una representación en formato string del documento.
        /// </summary>
        /// <returns>Una cadena que representa el documento.</returns>
        public override string ToString()
        {
            return $"Documento ID: {IdDocumento}, Nombre: {NombreArchivo}, Tipo: {TipoArchivo}";
        }
    }
}
