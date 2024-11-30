using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace ProyectoTicketing.Clases
{
    /// <summary>
    /// Representa un ticket en el sistema, con su descripción, solución, categoría, tipo de error, estado, etc.
    /// </summary>
    public class Ticket
    {
        /// <summary>
        /// ID único del ticket.
        /// </summary>
        [BsonId]
        public ObjectId IdTicket { get; set; }

        /// <summary>
        /// Descripción del ticket.
        /// </summary>
        [BsonElement("descripcion")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Solución aplicada al ticket, si la tiene.
        /// </summary>
        [BsonElement("solucion")]
        public string Solucion { get; set; }

        /// <summary>
        /// Categoría del ticket.
        /// </summary>
        [BsonElement("categoria")]
        public string Categoria { get; set; }

        /// <summary>
        /// Tipo de error relacionado con el ticket.
        /// </summary>
        [BsonElement("tipoError")]
        public string TipoError { get; set; }

        /// <summary>
        /// Estado del ticket (Ej. Abierto, Cerrado, etc.).
        /// </summary>
        [BsonElement("estado")]
        public string Estado { get; set; }

        /// <summary>
        /// Fecha de creación del ticket.
        /// </summary>
        [BsonElement("fechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Prioridad del ticket (Ej. Alta, Media, Baja).
        /// </summary>
        [BsonElement("prioridad")]
        public string Prioridad { get; set; }

        /// <summary>
        /// Nombre del ticket.
        /// </summary>
        [BsonElement("nombreTicket")]
        public string NombreTicket { get; set; }

        /// <summary>
        /// ID del usuario que creó el ticket.
        /// </summary>
        [BsonElement("IDUsuario")]
        public string IDUsuario { get; set; }

        /// <summary>
        /// ID del técnico asignado al ticket (puede ser null si no está asignado).
        /// </summary>
        [BsonElement("IDTecnico")]
        public string? IDTecnico { get; set; }

        /// <summary>
        /// ID del ticket padre (si es un ticket hijo de otro).
        /// </summary>
        public string? IDTicketPadre { get; set; }

        /// <summary>
        /// Lista de documentos adjuntos al ticket.
        /// </summary>
        [BsonElement("documentosAdjuntos")]
        public List<Documento> Documentos { get; set; }

        /// <summary>
        /// Constructor que inicializa un ticket con los valores proporcionados.
        /// </summary>
        /// <param name="tipoError">Tipo de error relacionado con el ticket.</param>
        /// <param name="categoria">Categoría del ticket.</param>
        /// <param name="descripcion">Descripción del ticket.</param>
        /// <param name="nombreTicket">Nombre del ticket.</param>
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

        /// <summary>
        /// Agrega o actualiza la solución del ticket.
        /// </summary>
        /// <param name="solucion">Solución aplicada al ticket.</param>
        public void AgregarSolucion(string solucion)
        {
            Solucion = solucion;
        }

        /// <summary>
        /// Asocia un documento al ticket.
        /// </summary>
        /// <param name="documento">Documento que se desea agregar al ticket.</param>
        public void AgregarDocumento(Documento documento)
        {
            Documentos.Add(documento);
        }

        /// <summary>
        /// Actualiza el estado del ticket.
        /// </summary>
        /// <param name="nuevoEstado">Nuevo estado del ticket.</param>
        public void ActualizarEstado(string nuevoEstado)
        {
            Estado = nuevoEstado;
        }

        /// <summary>
        /// Devuelve una representación en forma de cadena del ticket.
        /// </summary>
        /// <returns>Cadena con los detalles del ticket.</returns>
        public override string ToString()
        {
            return $"Ticket ID: {IdTicket}, Nombre: {NombreTicket}, Estado: {Estado}, Prioridad: {Prioridad}, Fecha de Creación: {FechaCreacion}, Solución: {Solucion}";
        }
    }
}
