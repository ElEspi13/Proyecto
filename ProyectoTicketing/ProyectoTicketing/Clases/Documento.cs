using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoTicketing.Clases
{
    public class Documento
    {
        public ObjectId IdDocumento { get; set; } // ID del archivo en GridFS
        public string NombreArchivo { get; set; }  // Nombre del archivo
        public string TipoArchivo { get; set; }    // Tipo MIME del archivo (por ejemplo, application/pdf)
        public string RutaArchivo {  get; set; }

        // Constructor que recibe un ObjectId (de GridFS)
        public Documento(ObjectId idDocumento, string nombreArchivo, string tipoArchivo, string rutaArchivo)
        {
            IdDocumento = idDocumento;
            NombreArchivo = nombreArchivo;
            TipoArchivo = tipoArchivo;
            RutaArchivo = rutaArchivo;

        }

        // Método para mostrar el documento en formato string
        public override string ToString()
        {
            return $"Documento ID: {IdDocumento}, Nombre: {NombreArchivo}, Tipo: {TipoArchivo}";
        }
    }
}
