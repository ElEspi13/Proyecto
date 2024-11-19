using MongoDB.Bson;
using ProyectoTicketing.Clases;
using SharpCompress.Archives;

namespace ProyectoTicketing.Vistas
{
    public partial class VentanaUsuario_Creador_Tickets : ContentPage
    {
        private AppShell shell;
        
        // Limite de documentos seleccionados
        private const int LIMITE_DOCUMENTOS = 3;

        // Variable para almacenar los documentos seleccionados
        private List<Documento> documentosSeleccionados = new List<Documento>();

        // Contador de documentos seleccionados
        private int contadorDocumentos = 0;
        private string ticketPadre = null;

        // Diccionario para las categor�as seg�n el tipo de error
        private Dictionary<string, List<string>> categoriasPorTipoError = new Dictionary<string, List<string>>()
        {
            { "Error de Conexi�n", new List<string> { "Internet", "Red Local", "VPN", "Firewall" } },
            { "Error de Software", new List<string> { "Aplicaci�n", "Sistema Operativo", "Actualizaci�n", "Compatibilidad" } },
            { "Error de Hardware", new List<string> { "Componentes", "Mantenimiento", "Reemplazo", "Instalaci�n" } }
        };

        public VentanaUsuario_Creador_Tickets(AppShell shell)
        {
            InitializeComponent();
            this.shell = shell;
        }

        // M�todo que maneja el evento "ENVIAR"
        private async void OnEnviarClicked(object sender, EventArgs e)
        {
            // Crear instancia del ticket
            Ticket nuevoTicket = new Ticket(
                tipoError: TipoErrorPicker.SelectedItem.ToString(),
                categoria: CategoriaPicker.SelectedItem.ToString(),
                descripcion: Descripcion.Text,
                nombreTicket: NombreTicketEntry.Text)
            {
                FechaCreacion = DateTime.Now,
                Estado = "Abierto",
                Prioridad = "Media",
                IDUsuario = null,
                IDTicketPadre = ticketPadre
            };

            // Agregar los documentos seleccionados al ticket
            foreach (var documento in documentosSeleccionados)
            {
                // Aqu� puedes agregar la l�gica para almacenar el archivo en GridFS o solo agregar el documento como referencia
                nuevoTicket.AgregarDocumento(documento);
            }

            // Enviar el ticket
            shell.CrearTicket(nuevoTicket);

            // Limpiar el formulario despu�s de enviar el ticket
            ClearForm();
            ticketPadre = null;
            TicketHijoLabel.IsVisible =false;
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

                // Mostrar el nombre del archivo
                ArchivoNombre.Text = result.FileName;

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
                CrearControlesDocumento(result);

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
        private void CrearControlesDocumento(FileResult result)
        {
            // Crear un StackLayout din�mico para cada documento
            var stackLayoutDocumento = new StackLayout { Orientation = StackOrientation.Vertical, Spacing = 10 };

            // Crear el ImageButton para el documento
            var imageButton = new ImageButton
            {
                WidthRequest = 50,
                HeightRequest = 50
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

            // Crear el Label para mostrar el nombre del archivo
            var labelDocumento = new Label
            {
                Text = result.FileName,
                FontSize = 16,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            // Agregar el ImageButton y el Label al StackLayout
            stackLayoutDocumento.Children.Add(imageButton);
            stackLayoutDocumento.Children.Add(labelDocumento);

            // Agregar el StackLayout al contenedor de documentos
            DocumentosSeleccionadosLayout.Children.Add(stackLayoutDocumento);

            // Asociar el evento Clicked al ImageButton
            imageButton.Clicked += (sender, e) => OnDescargarDocumentoClicked(result);
        }

        // M�todo para manejar la descarga del documento
        private async void OnDescargarDocumentoClicked(FileResult result)
        {
            
        }

        // M�todo para limpiar el formulario despu�s de enviar el ticket
        private void ClearForm()
        {
            NombreTicketEntry.Text = "";
            TipoErrorPicker.SelectedItem = null;
            CategoriaPicker.SelectedItem = null;
            Descripcion.Text= string.Empty;
            ArchivoEntry.Text = string.Empty;
            ArchivoSeleccionadoLayout.IsVisible = false;
            documentosSeleccionados.Clear(); // Limpiar la lista de documentos
            contadorDocumentos = 0; // Resetear contador de documentos

            // Habilitar el bot�n de selecci�n de archivo si estaba deshabilitado
            SeleccionarArchivoButton.IsEnabled = true;

            // Limpiar la interfaz gr�fica de documentos seleccionados
            DocumentosSeleccionadosLayout.Children.Clear();
        }

        // M�todo que maneja la selecci�n de tipo de error para actualizar las categor�as
        private void OnTipoErrorSelected(object sender, EventArgs e)
        {
            // Limpiar las opciones actuales de CategoriaPicker
            CategoriaPicker.Items.Clear();

            // Obtener el tipo de error seleccionado
            string tipoErrorSeleccionado = TipoErrorPicker.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(tipoErrorSeleccionado) && categoriasPorTipoError.ContainsKey(tipoErrorSeleccionado))
            {
                // Obtener las categor�as correspondientes al tipo de error
                List<string> categorias = categoriasPorTipoError[tipoErrorSeleccionado];

                // Agregar las categor�as al Picker de Categoria
                foreach (string categoria in categorias)
                {
                    CategoriaPicker.Items.Add(categoria);
                }
            }
        }

        internal void GuardarTicketPadre(string iDTicketPadre)
        {
            this.ticketPadre = iDTicketPadre;
            TicketHijoLabel.IsVisible = true;
        }
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            ticketPadre = null;
            TicketHijoLabel.IsVisible = false;
        }
    }
}
