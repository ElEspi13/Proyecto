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
    public VentanaTecnico_ResolvedorTickets(AppShell shell)
    {
		InitializeComponent();
        this.shell = shell;
    }
    

    // Método para seleccionar el archivo (vinculado al botón de selección)
    private async void OnSeleccionarArchivoClicked(object sender, EventArgs e)
    {
        
        // Verificar si ya se han seleccionado 3 documentos
        if (contadorDocumentos >= LIMITE_DOCUMENTOS)
        {
            // Mostrar un mensaje de error o advertencia
            await DisplayAlert("Límite de Documentos", "Ya has alcanzado el límite de 3 documentos.", "OK");
            return; // No permite seleccionar más archivos
        }

        var result = await FilePicker.Default.PickAsync();
        if (result != null)
        {
            // Obtener la ruta completa del archivo seleccionado
            var rutaArchivoSeleccionado = result.FullPath;

            // Crear un documento y agregarlo a la lista
            Documento documento = new Documento(
                idDocumento: ObjectId.Empty, // Se actualizará después de cargar en GridFS
                nombreArchivo: result.FileName,
                tipoArchivo: Path.GetExtension(result.FileName), // Asigna el tipo según la extensión
                rutaArchivo: rutaArchivoSeleccionado
            );
            documentosSeleccionados.Add(documento);

            // Mostrar el nombre del archivo en el Entry
            ArchivoEntry.Text = result.FileName;

            // Hacer visible la vista previa del archivo
            ArchivoSeleccionadoLayout.IsVisible = true;


            // Obtener la extensión del archivo
            string extension = Path.GetExtension(result.FileName).ToLower();

            // Asignar el ícono correspondiente según la extensión del archivo
            if (extension == ".docx" || extension == ".doc")
            {
                ArchivoIcono.Source = "icono_word.png";  // Asegúrate de tener el ícono en tus recursos
            }
            else if (extension == ".pdf")
            {
                ArchivoIcono.Source = "icono_pdf.png";   // Asegúrate de tener el ícono en tus recursos
            }
            else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
            {
                ArchivoIcono.Source = "icono_imagen.png"; // Asegúrate de tener el ícono en tus recursos
            }
            else
            {
                ArchivoIcono.Source = "icono_generico.png"; // Asegúrate de tener el ícono en tus recursos
            }

            // Crear dinámicamente los controles para el archivo seleccionado
            CrearControlesDocumento(result,documento);

            // Incrementar el contador de documentos seleccionados
            contadorDocumentos++;

            // Si se alcanzó el límite, deshabilitamos el botón de selección de archivo
            if (contadorDocumentos >= LIMITE_DOCUMENTOS)
            {
                SeleccionarArchivoButton.IsEnabled = false;
                await DisplayAlert("Límite de Documentos", "Ya has seleccionado 3 documentos. No puedes agregar más.", "OK");
            }
        }
    }

    // Método para crear dinámicamente los controles para el documento seleccionado
    private void CrearControlesDocumento(FileResult result,Documento documento)
    {
        // Crear un StackLayout dinámico para cada documento
        var stackLayoutDocumento = new StackLayout { Orientation = StackOrientation.Vertical, Spacing = 10 };

        // Crear el ImageButton para el documento
        var imageButton = new ImageButton
        {
            WidthRequest = 50,
            HeightRequest = 50,
            HorizontalOptions=LayoutOptions.Center,
        };

        // Establecer el ícono basado en la extensión del archivo
        string extension = Path.GetExtension(result.FileName).ToLower();
        if (extension == ".docx" || extension == ".doc")
        {
            imageButton.Source = "icono_word.png";  // Asignar ícono de Word
        }
        else if (extension == ".pdf")
        {
            imageButton.Source = "icono_pdf.png";   // Asignar ícono de PDF
        }
        else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
        {
            imageButton.Source = "icono_imagen.png"; // Asignar ícono de imagen
        }
        else
        {
            imageButton.Source = "icono_generico.png"; // Asignar ícono genérico
        }
        imageButton.IsVisible = true;

        // Crear el Label para mostrar el nombre del archivo
        var labelDocumento = new Label
        {
            Text = result.FileName,
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Center,
        };
        labelDocumento.IsVisible = true;
        // Agregar el ImageButton y el Label al StackLayout
        stackLayoutDocumento.Children.Add(imageButton);
        stackLayoutDocumento.Children.Add(labelDocumento);

        // Agregar el StackLayout al contenedor de documentos
        DocumentosSeleccionadosLayout.Children.Add(stackLayoutDocumento);

        // Asociar el evento Clicked al ImageButton
        imageButton.Clicked += (sender, e) => OnDescargarDocumentoClicked(documento);
    }

    // Método para manejar la descarga del documento
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
    public void SetTicketData(Ticket ticket)
    {
        this.ticket = ticket;
        try
        {

            BindingContext = ticket;
            if (ticket.Documentos != null && ticket.Documentos.Count > 0)
            {
                // Limpiamos cualquier contenido anterior en el StackLayout antes de agregar nuevos elementos
                DocumentosSeleccionadosLayout.Children.Clear();

                // Recorremos cada documento para asignar el ícono correspondiente
                foreach (var documento in ticket.Documentos)
                {
                    // Clonamos el StackLayout básico que está definido en el XAML
                    var documentoLayout = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,  // Cambiado a Vertical para apilar los elementos (imagen y texto)
                        Spacing = 10,
                        HorizontalOptions = LayoutOptions.Center // Centrado para que se vea bonito
                    };

                    // Creamos un ImageButton para el ícono
                    var imageButton = new ImageButton
                    {
                        WidthRequest = 50,
                        HeightRequest = 50
                    };

                    // Determinamos el ícono que corresponde según la extensión del archivo
                    string extension = Path.GetExtension(documento.NombreArchivo).ToLower();  // Convertimos a minúsculas

                    if (extension == ".pdf")
                    {
                        // Si es un PDF, usamos el ícono de PDF
                        imageButton.Source = "icono_pdf.png";
                    }
                    else if (extension == ".docx" || extension == ".doc")
                    {
                        // Si es un archivo Word, usamos el ícono de Word
                        imageButton.Source = "icono_word.png";  // Puedes tener un ícono específico para Word
                    }
                    else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                    {
                        // Si es una imagen, usamos la ruta de la imagen directamente
                        imageButton.Source = documento.RutaArchivo;
                    }
                    else
                    {
                        // Para otros tipos de archivo, usamos un ícono genérico
                        imageButton.Source = "icono_generico.png";
                    }

                    // Agregamos un evento de clic al ImageButton (si lo deseas, puedes hacer algo cuando se haga clic)
                    imageButton.Clicked += (sender, e) =>
                    {
                        // Llamamos al método OnDescargarDocumentoClicked y pasamos el documento
                        OnDescargarDocumentoClicked(documento);
                    };


                    // Creamos el Label para mostrar el nombre del archivo
                    var label = new Label
                    {
                        Text = documento.NombreArchivo,
                        FontSize = 16,
                        HorizontalOptions = LayoutOptions.Center,
                    };

                    // Agregamos el ImageButton y el Label al StackLayout de cada documento
                    documentoLayout.Children.Add(imageButton);
                    documentoLayout.Children.Add(label);

                    // Finalmente, agregamos el StackLayout al StackLayout principal donde se muestran los documentos
                    DocumentosSeleccionadosLayout.Children.Add(documentoLayout);
                }

                // Hacemos visible la sección de documentos
                DocumentosSeleccionadosLayout.IsVisible = true;
            }
            else if(documentosSeleccionados.Count==0)
            {
                // Si no hay documentos, ocultamos la sección de documentos
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

        }



    }

    private void EnviarButton_Clicked(object sender, EventArgs e)
    {
        shell.ActualizarTecnicoTicket(ticket.IdTicket,Solucion.Text,documentosSeleccionados);
    }

}