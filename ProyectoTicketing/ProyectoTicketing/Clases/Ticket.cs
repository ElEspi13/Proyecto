using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoTicketing.Clases
{
    public class Ticket
    {
        [BsonId]
        public ObjectId IdTicket { get; set; }
        [BsonElement("descripcion")]
        public string Descripcion { get; set; }

        [BsonElement("solucion")]
        public string Solucion { get; set; }

        [BsonElement("categoria")]
        public string Categoria { get; set; }

        [BsonElement("tipoError")]
        public string TipoError { get; set; }

        [BsonElement("estado")]
        public string Estado { get; set; }

        [BsonElement("fechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [BsonElement("prioridad")]
        public string Prioridad { get; set; }

        [BsonElement("nombreTicket")]
        public string NombreTicket { get; set; }

        [BsonElement("IDUsuario")]
        public string IDUsuario { get; set; }

        [BsonElement("IDTecnico")]
        public string? IDTecnico { get; set; }

        public string? IDTicketPadre { get; set; }

        [BsonElement("documentosAdjuntos")]
        public List<Documento> Documentos { get; set; }

        public Ticket(string tipoError, string categoria, string descripcion, string nombreTicket)
        {
            TipoError = tipoError;
            Categoria = categoria;
            Descripcion = descripcion;
            NombreTicket = nombreTicket;
            Estado = "Abierto";
            FechaCreacion = DateTime.Now;
            Prioridad = "Media";
            Solucion = null;
            IDTecnico = null;
            Documentos = new List<Documento>();
        }
    

    // Método para agregar o actualizar la solución
    public void AgregarSolucion(string solucion)
        {
            Solucion = solucion;
        }

        // Método para asociar un documento al ticket
        public void AgregarDocumento(Documento documento)
        {
            Documentos.Add(documento);
        }

        // Método para actualizar el estado del ticket
        public void ActualizarEstado(string nuevoEstado)
        {
            Estado = nuevoEstado;
        }

        // Método para obtener una representación en forma de cadena
        public override string ToString()
        {
            return $"Ticket ID: {IdTicket}, Nombre: {NombreTicket}, Estado: {Estado}, Prioridad: {Prioridad}, Fecha de Creación: {FechaCreacion}, Solución: {Solucion}";
        }
    }
}