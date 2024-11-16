using ProyectoTicketing.Clases;
using System.Runtime.Intrinsics.X86;
namespace ProyectoTicketing.Vistas;

public partial class Ventana_DetallesTicket : ContentPage
{
	private AppShell shell;
	public Ventana_DetallesTicket(AppShell shell)
	{
		InitializeComponent();
		this.shell = shell;
	}
    private async void OnCerrarTicketClicked(object sender, EventArgs e)
    {
        // Lógica para cerrar el ticket
        await DisplayAlert("Cerrar Ticket", "El ticket se ha cerrado correctamente.", "OK");
    }

    // Maneja el evento de Crear Ticket Hijo
    private async void OnCrearTicketHijoClicked(object sender, EventArgs e)
    {
        // Lógica para crear un ticket hijo
        await DisplayAlert("Crear Ticket Hijo", "Se ha creado un ticket hijo.", "OK");
    }
    public void SetTicketData(Ticket ticket)
    {
        // Asignamos el ticket al BindingContext de la página
        BindingContext = ticket;
        try {
            if(ticket.Documentos != null && ticket.Documentos.Count > 0)
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
        else
            {
                // Si no hay documentos, ocultamos la sección de documentos
                DocumentosSeleccionadosLayout.IsVisible = false;
            }
        } catch (Exception e)
        {

        }
        
    }





    private void OnDescargarDocumentoClicked(Documento documento)
    {

        if (documento != null)
        {
            shell.DescargarDocumento(documento);
            DisplayAlert("Descargado", "Se ha descargado el archivo en su sistema.", "OK");
        }
        else
        {
            DisplayAlert("Error", "No se pudo encontrar el documento asociado.", "OK");
        }
       
    }
}