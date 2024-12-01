using Microsoft.Maui.Controls;
using MongoDB.Bson;
using ProyectoTicketing.Clases;

namespace ProyectoTicketing.Vistas;

public partial class VentanaTecnico_ResolvedorTickets : ContentPage
{
    private AppShell shell;
    private const int LIMITE_DOCUMENTOS = 3;
    private List<Documento> documentosSeleccionados = new List<Documento>();
    private int contadorDocumentos = 0;
    private Ticket ticket;

    /// <summary>
    /// Constructor de la ventana donde el técnico resuelve tickets.
    /// </summary>
    public VentanaTecnico_ResolvedorTickets(AppShell shell)
    {
        InitializeComponent();
        this.shell = shell;
    }

    /// <summary>
    /// Método para seleccionar un archivo. Se invoca cuando el usuario hace clic en el botón de selección.
    /// </summary>
    private async void OnSeleccionarArchivoClicked(object sender, EventArgs e)
    {
        // Verificar si ya se han seleccionado 3 documentos
        if (contadorDocumentos >= LIMITE_DOCUMENTOS)
        {
            await DisplayAlert("Límite de Documentos", "Ya has alcanzado el límite de 3 documentos.", "OK");
            return;
        }

        var result = await FilePicker.Default.PickAsync();
        if (result != null)
        {
            var rutaArchivoSeleccionado = result.FullPath;

            Documento documento = new Documento(
                idDocumento: ObjectId.Empty,
                nombreArchivo: result.FileName,
                tipoArchivo: Path.GetExtension(result.FileName),
                rutaArchivo: rutaArchivoSeleccionado
            );
            documentosSeleccionados.Add(documento);

            ArchivoEntry.Text = result.FileName;
            ArchivoSeleccionadoLayout.IsVisible = true;

            string extension = Path.GetExtension(result.FileName).ToLower();
            if (extension == ".docx" || extension == ".doc")
            {
                ArchivoIcono.Source = "icono_word.png";
            }
            else if (extension == ".pdf")
            {
                ArchivoIcono.Source = "icono_pdf.png";
            }
            else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
            {
                ArchivoIcono.Source = "icono_imagen.png";
            }
            else
            {
                ArchivoIcono.Source = "icono_generico.png";
            }

            CrearControlesDocumento(result, documento);

            contadorDocumentos++;

            if (contadorDocumentos >= LIMITE_DOCUMENTOS)
            {
                SeleccionarArchivoButton.IsEnabled = false;
                await DisplayAlert("Límite de Documentos", "Ya has seleccionado 3 documentos. No puedes agregar más.", "OK");
            }
        }
    }

    /// <summary>
    /// Método para crear dinámicamente los controles visuales para el archivo seleccionado.
    /// </summary>
    private void CrearControlesDocumento(FileResult result, Documento documento)
    {
        var stackLayoutDocumento = new StackLayout { Orientation = StackOrientation.Vertical, Spacing = 10 };

        var imageButton = new ImageButton
        {
            WidthRequest = 50,
            HeightRequest = 50,
            HorizontalOptions = LayoutOptions.Center,
        };

        string extension = Path.GetExtension(result.FileName).ToLower();
        if (extension == ".docx" || extension == ".doc")
        {
            imageButton.Source = "icono_word.png";
        }
        else if (extension == ".pdf")
        {
            imageButton.Source = "icono_pdf.png";
        }
        else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
        {
            imageButton.Source = "icono_imagen.png";
        }
        else
        {
            imageButton.Source = "icono_generico.png";
        }
        imageButton.IsVisible = true;

        var labelDocumento = new Label
        {
            Text = result.FileName,
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Center,
        };
        labelDocumento.IsVisible = true;

        stackLayoutDocumento.Children.Add(imageButton);
        stackLayoutDocumento.Children.Add(labelDocumento);

        DocumentosSeleccionadosLayout.Children.Add(stackLayoutDocumento);

        imageButton.Clicked += (sender, e) => OnDescargarDocumentoClicked(documento);
    }

    /// <summary>
    /// Método para manejar la descarga del documento seleccionado.
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
    /// Establece los datos del ticket y genera dinámicamente los controles de los documentos asociados.
    /// </summary>
    public void SetTicketData(Ticket ticket)
    {
        contadorDocumentos = 0;
        DocumentosSeleccionadosLayout.Clear();
        SeleccionarArchivoButton.IsEnabled = true;
        this.ticket = ticket;
        try
        {
            BindingContext = ticket;
            if (ticket.Documentos != null && ticket.Documentos.Count > 0)
            {
                DocumentosSeleccionadosLayout.Children.Clear();

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
                        HorizontalOptions = LayoutOptions.Center,
                        TextColor = DocumentoNombre.TextColor
                    };

                    documentoLayout.Children.Add(imageButton);
                    documentoLayout.Children.Add(label);

                    DocumentosSeleccionadosLayout.Children.Add(documentoLayout);
                }

                DocumentosSeleccionadosLayout.IsVisible = true;
            }
            else if (documentosSeleccionados.Count == 0)
            {
                DocumentosSeleccionadosLayout.IsVisible = true;
            }
            else
            {
                DocumentosSeleccionadosLayout.Clear();
            }

            if (ticket.Estado == "Cerrado")
            {
                EnviarButton.IsVisible = false;
            }
            else
            {
                EnviarButton.IsVisible = true;
            }
        }
        catch (Exception e)
        {
            // Manejo de excepciones si fuera necesario
        }
    }

    /// <summary>
    /// Método que se invoca al hacer clic en el botón de "Enviar" para actualizar el ticket con la solución y los documentos seleccionados.
    /// </summary>
    private void EnviarButton_Clicked(object sender, EventArgs e)
    {
        if (ticket!=null)
        {
            shell.ActualizarTecnicoTicket(ticket.IdTicket, Solucion.Text, documentosSeleccionados);
        }
        else{
            DisplayAlert("Error","No ha seleccionado Ticket","OK");
        }
        
    }
    /// <summary>
    /// Cambia el tamaño de la fuente de varios elementos de la interfaz de usuario.
    /// </summary>
    /// <param name="factorMultiplicador">El factor por el cual se multiplicará el tamaño original de la fuente.</param>
    public void CambiarTamanoFuente(double factorMultiplicador)
    {
        TituloLabel.FontSize = 24 * factorMultiplicador;
        TipoErrorLabel.FontSize = 16 * factorMultiplicador;
        CategoriaLabel.FontSize = 16 * factorMultiplicador;
        IdTicketLabel.FontSize = 16 * factorMultiplicador;
        NombreTicketLabel.FontSize = 16 * factorMultiplicador;
        DocumentosTicketLabel.FontSize = 18 * factorMultiplicador;
        EspecificacionErrorLabel.FontSize = 18 * factorMultiplicador;
        DescripcionSolucionLabel.FontSize = 18 * factorMultiplicador;
        TipoErrorEntry.FontSize = 16 * factorMultiplicador;
        CategoriaEntry.FontSize = 16 * factorMultiplicador;
        IdTicketEntry.FontSize = 16 * factorMultiplicador;
        NombreTicketEntry.FontSize = 16 * factorMultiplicador;
        EspecificacionErrorEditor.FontSize = 16 * factorMultiplicador;
        Solucion.FontSize = 16 * factorMultiplicador;
        EnviarButton.FontSize = 20 * factorMultiplicador;
        SeleccionarArchivoButton.FontSize = 16 * factorMultiplicador;
    }

    /// <summary>
    /// Limpia todos los campos de entrada, selectores y elementos visuales asociados a los documentos 
    /// en la interfaz de usuario para reiniciar la vista y permitir la entrada de nuevos datos.
    /// </summary>
    public void LimpiarCampos()
    {
        TipoErrorEntry.Text = string.Empty;
        CategoriaEntry.Text = string.Empty;
        IdTicketEntry.Text = string.Empty;
        NombreTicketEntry.Text = string.Empty;
        EspecificacionErrorEditor.Text = string.Empty;
        Solucion.Text = string.Empty;
        ArchivoEntry.Text = string.Empty;


        DocumentosSeleccionadosLayout.Children.Clear();

        DocumentoIcono.Source = null;
        ArchivoIcono.Source = null;

        ArchivoSeleccionadoLayout.IsVisible = false;

        DocumentoNombre.Text = string.Empty;
    }


}
