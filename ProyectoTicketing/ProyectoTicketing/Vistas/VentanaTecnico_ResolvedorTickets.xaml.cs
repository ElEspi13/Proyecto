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
    

    // M�todo para seleccionar el archivo (vinculado al bot�n de selecci�n)
    private async void OnSeleccionarArchivoClicked(object sender, EventArgs e)
    {
        
        // Verificar si ya se han seleccionado 3 documentos
        if (contadorDocumentos >= LIMITE_DOCUMENTOS)
        {
            // Mostrar un mensaje de error o advertencia
            await DisplayAlert("L�mite de Documentos", "Ya has alcanzado el l�mite de 3 documentos.", "OK");
            return; // No permite seleccionar m�s archivos
        }

        var result = await FilePicker.Default.PickAsync();
        if (result != null)
        {
            // Obtener la ruta completa del archivo seleccionado
            var rutaArchivoSeleccionado = result.FullPath;

            // Crear un documento y agregarlo a la lista
            Documento documento = new Documento(
                idDocumento: ObjectId.Empty, // Se actualizar� despu�s de cargar en GridFS
                nombreArchivo: result.FileName,
                tipoArchivo: Path.GetExtension(result.FileName), // Asigna el tipo seg�n la extensi�n
                rutaArchivo: rutaArchivoSeleccionado
            );
            documentosSeleccionados.Add(documento);

            // Mostrar el nombre del archivo en el Entry
            ArchivoEntry.Text = result.FileName;

            // Hacer visible la vista previa del archivo
            ArchivoSeleccionadoLayout.IsVisible = true;


            // Obtener la extensi�n del archivo
            string extension = Path.GetExtension(result.FileName).ToLower();

            // Asignar el �cono correspondiente seg�n la extensi�n del archivo
            if (extension == ".docx" || extension == ".doc")
            {
                ArchivoIcono.Source = "icono_word.png";  // Aseg�rate de tener el �cono en tus recursos
            }
            else if (extension == ".pdf")
            {
                ArchivoIcono.Source = "icono_pdf.png";   // Aseg�rate de tener el �cono en tus recursos
            }
            else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
            {
                ArchivoIcono.Source = "icono_imagen.png"; // Aseg�rate de tener el �cono en tus recursos
            }
            else
            {
                ArchivoIcono.Source = "icono_generico.png"; // Aseg�rate de tener el �cono en tus recursos
            }

            // Crear din�micamente los controles para el archivo seleccionado
            CrearControlesDocumento(result,documento);

            // Incrementar el contador de documentos seleccionados
            contadorDocumentos++;

            // Si se alcanz� el l�mite, deshabilitamos el bot�n de selecci�n de archivo
            if (contadorDocumentos >= LIMITE_DOCUMENTOS)
            {
                SeleccionarArchivoButton.IsEnabled = false;
                await DisplayAlert("L�mite de Documentos", "Ya has seleccionado 3 documentos. No puedes agregar m�s.", "OK");
            }
        }
    }

    // M�todo para crear din�micamente los controles para el documento seleccionado
    private void CrearControlesDocumento(FileResult result,Documento documento)
    {
        // Crear un StackLayout din�mico para cada documento
        var stackLayoutDocumento = new StackLayout { Orientation = StackOrientation.Vertical, Spacing = 10 };

        // Crear el ImageButton para el documento
        var imageButton = new ImageButton
        {
            WidthRequest = 50,
            HeightRequest = 50,
            HorizontalOptions=LayoutOptions.Center,
        };

        // Establecer el �cono basado en la extensi�n del archivo
        string extension = Path.GetExtension(result.FileName).ToLower();
        if (extension == ".docx" || extension == ".doc")
        {
            imageButton.Source = "icono_word.png";  // Asignar �cono de Word
        }
        else if (extension == ".pdf")
        {
            imageButton.Source = "icono_pdf.png";   // Asignar �cono de PDF
        }
        else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
        {
            imageButton.Source = "icono_imagen.png"; // Asignar �cono de imagen
        }
        else
        {
            imageButton.Source = "icono_generico.png"; // Asignar �cono gen�rico
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

    // M�todo para manejar la descarga del documento
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

                // Recorremos cada documento para asignar el �cono correspondiente
                foreach (var documento in ticket.Documentos)
                {
                    // Clonamos el StackLayout b�sico que est� definido en el XAML
                    var documentoLayout = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,  // Cambiado a Vertical para apilar los elementos (imagen y texto)
                        Spacing = 10,
                        HorizontalOptions = LayoutOptions.Center // Centrado para que se vea bonito
                    };

                    // Creamos un ImageButton para el �cono
                    var imageButton = new ImageButton
                    {
                        WidthRequest = 50,
                        HeightRequest = 50
                    };

                    // Determinamos el �cono que corresponde seg�n la extensi�n del archivo
                    string extension = Path.GetExtension(documento.NombreArchivo).ToLower();  // Convertimos a min�sculas

                    if (extension == ".pdf")
                    {
                        // Si es un PDF, usamos el �cono de PDF
                        imageButton.Source = "icono_pdf.png";
                    }
                    else if (extension == ".docx" || extension == ".doc")
                    {
                        // Si es un archivo Word, usamos el �cono de Word
                        imageButton.Source = "icono_word.png";  // Puedes tener un �cono espec�fico para Word
                    }
                    else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                    {
                        // Si es una imagen, usamos la ruta de la imagen directamente
                        imageButton.Source = documento.RutaArchivo;
                    }
                    else
                    {
                        // Para otros tipos de archivo, usamos un �cono gen�rico
                        imageButton.Source = "icono_generico.png";
                    }

                    // Agregamos un evento de clic al ImageButton (si lo deseas, puedes hacer algo cuando se haga clic)
                    imageButton.Clicked += (sender, e) =>
                    {
                        // Llamamos al m�todo OnDescargarDocumentoClicked y pasamos el documento
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

                // Hacemos visible la secci�n de documentos
                DocumentosSeleccionadosLayout.IsVisible = true;
            }
            else if(documentosSeleccionados.Count==0)
            {
                // Si no hay documentos, ocultamos la secci�n de documentos
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