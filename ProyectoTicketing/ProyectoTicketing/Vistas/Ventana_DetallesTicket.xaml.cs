using ProyectoTicketing.Clases;
using System.Runtime.Intrinsics.X86;

namespace ProyectoTicketing.Vistas;

public partial class Ventana_DetallesTicket : ContentPage
{
    private AppShell shell;
    private Ticket ticket;

    /// <summary>
    /// Constructor de la clase Ventana_DetallesTicket.
    /// </summary>
    /// <param name="shell">Instancia de AppShell que maneja la lógica de la aplicación.</param>
    public Ventana_DetallesTicket(AppShell shell)
    {
        InitializeComponent();
        this.shell = shell;
    }

    /// <summary>
    /// Evento que se dispara cuando el usuario decide cerrar el ticket.
    /// </summary>
    private async void OnCerrarTicketClicked(object sender, EventArgs e)
    {
        bool result = await DisplayAlert("Cerrar Ticket", "Está seguro de que quiere cerrar los Tickets SE CERRARÁN TODOS LOS TICKETS RELACIONADOS si no esta seguro no los cierre", "OK", "Cancelar");

        if (result)
        {
            if (ticket.IDTicketPadre != null)
            {
                // Buscar todos los tickets que tienen este IDTicketPadre
                shell.CerrarTicketsIDTicketPadre(ticket.IDTicketPadre);
                shell.ActualizarTicketsTiempoReal();
            }
            else
            {
                shell.CerrarTicketsIDTicket(ticket.IdTicket);
                shell.ActualizarTicketsTiempoReal();
            }
            await DisplayAlert("Tickets Cerrados", "Todos los tickets relacionados han sido cerrados.", "OK");
        }
        else
        {
            // Si el usuario presiona "Cancelar", no hacer nada o realizar alguna otra acción
        }
    }

    /// <summary>
    /// Evento que se dispara cuando se quiere crear un ticket hijo relacionado con el ticket actual.
    /// </summary>
    private async void OnCrearTicketHijoClicked(object sender, EventArgs e)
    {
        if (ticket.IDTicketPadre != null)
        {
            shell.CrearTicketHijo(ticket.IDTicketPadre);
        }
        else {
            shell.CrearTicketHijo(IdTicketEntry.Text);
        }
        
    }

    /// <summary>
    /// Asigna los datos del ticket a la interfaz de usuario y maneja los documentos asociados.
    /// </summary>
    /// <param name="ticket">El ticket que se desea mostrar en la vista.</param>
    public void SetTicketData(Ticket ticket)
    {
        this.ticket = ticket;
        BindingContext = ticket;

        try
        {
            if (ticket.Documentos != null && ticket.Documentos.Count > 0)
            {
                // Limpiamos cualquier contenido anterior en el StackLayout antes de agregar nuevos elementos
                DocumentosSeleccionadosLayout.Children.Clear();

                // Recorremos cada documento para asignar el ícono correspondiente
                foreach (var documento in ticket.Documentos)
                {
                    var documentoLayout = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Spacing = 10,
                        HorizontalOptions = LayoutOptions.Center
                    };

                    var imageButton = new ImageButton
                    {
                        WidthRequest = 50,
                        HeightRequest = 50
                    };

                    string extension = Path.GetExtension(documento.NombreArchivo).ToLower();

                    if (extension == ".pdf")
                    {
                        imageButton.Source = "icono_pdf.png";
                    }
                    else if (extension == ".docx" || extension == ".doc")
                    {
                        imageButton.Source = "icono_word.png";
                    }
                    else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                    {
                        imageButton.Source = "icono_imagen.png";
                    }
                    else
                    {
                        imageButton.Source = "icono_generico.png";
                    }

                    imageButton.Clicked += (sender, e) =>
                    {
                        OnDescargarDocumentoClicked(documento);
                    };

                    var label = new Label
                    {
                        Text = documento.NombreArchivo,
                        FontSize = 16,
                    };

                    documentoLayout.Children.Add(imageButton);
                    documentoLayout.Children.Add(label);

                    DocumentosSeleccionadosLayout.Children.Add(documentoLayout);
                }

                DocumentosSeleccionadosLayout.IsVisible = true;
            }
            else
            {
                DocumentosSeleccionadosLayout.IsVisible = false;
            }

            if (ticket.Estado == "Cerrado")
            {
                CrearHijo.IsVisible = false;
                CerrarTickets.IsVisible = false;
                AsignarTicket.IsVisible = false;
            }
            else
            {
                CrearHijo.IsVisible = true;
                CerrarTickets.IsVisible = true;
                AsignarTicket.IsVisible=false;
            }
        }
        catch (Exception e)
        {

        }
    }

    /// <summary>
    /// Maneja el evento para descargar un documento asociado al ticket.
    /// </summary>
    private void OnDescargarDocumentoClicked(Documento documento)
    {
        if (documento != null)
        {
            shell.DescargarDocumento(documento);
        }
        else
        {
            DisplayAlert("Error", "No se pudo encontrar el documento asociado.", "OK");
        }
    }

    /// <summary>
    /// Configura la interfaz para que se muestre la vista del técnico.
    /// </summary>
    internal void VistaTecnico()
    {
        CrearHijo.IsVisible = false;
        CerrarTickets.IsVisible = false;

        if (ticket.Estado == "Abierto")
        {
            AsignarTicket.IsVisible = true;
        }
    }

    /// <summary>
    /// Asigna el ticket a un técnico.
    /// </summary>
    private void OnAsignarTicketClicked(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Botón 'Asignar Ticket' clickeado.");
        shell.AsignarTicketATecnico(ticket.IdTicket);
        shell.ActualizarTicketsTiempoRealTecnico();
    }
    /// <summary>
    /// Cambia el tamaño de la fuente de varios elementos de la interfaz de usuario.
    /// </summary>
    /// <param name="factorMultiplicador">El factor por el cual se multiplicará el tamaño original de la fuente.</param>
    public void CambiarTamanoFuente(double factorMultiplicador)
    {
        TituloLabel.FontSize = 24 * factorMultiplicador;
        TipoErrorLabel.FontSize = 16 * factorMultiplicador;
        TipoErrorEntry.FontSize = 16 * factorMultiplicador;
        CategoriaLabel.FontSize = 16 * factorMultiplicador;
        CategoriaEntry.FontSize = 16 * factorMultiplicador;
        IdTicketLabel.FontSize = 16 * factorMultiplicador;
        IdTicketEntry.FontSize = 16 * factorMultiplicador;
        NombreTicketLabel.FontSize = 16 * factorMultiplicador;
        NombreTicketEntry.FontSize = 16 * factorMultiplicador;
        CerrarTickets.FontSize = 20 * factorMultiplicador;
        CrearHijo.FontSize = 20 * factorMultiplicador;
        EspecificacionLabel.FontSize = 16 * factorMultiplicador;
        EspecificacionEditor.FontSize = 16 * factorMultiplicador;
        SolucionEditor.FontSize = 16 * factorMultiplicador;
        SolucionLabel.FontSize = 16 * factorMultiplicador;
        DocumentoNombre.FontSize = 14 * factorMultiplicador;
        DocumentoLabel.FontSize = 16 * factorMultiplicador;
        AsignarTicket.FontSize = 20 * factorMultiplicador;
    }
}
