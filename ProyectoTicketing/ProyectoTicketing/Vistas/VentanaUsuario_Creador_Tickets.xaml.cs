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

        // Diccionario para las categorías según el tipo de error
        private Dictionary<string, List<string>> categoriasPorTipoError = new Dictionary<string, List<string>>()
        {
            { "Error de Conexión", new List<string> { "Internet", "Red Local", "VPN", "Firewall" } },
            { "Error de Software", new List<string> { "Aplicación", "Sistema Operativo", "Actualización", "Compatibilidad" } },
            { "Error de Hardware", new List<string> { "Componentes", "Mantenimiento", "Reemplazo", "Instalación" } }
        };

        public VentanaUsuario_Creador_Tickets(AppShell shell)
        {
            InitializeComponent();
            this.shell = shell;
        }

        // Método que maneja el evento "ENVIAR"
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
                // Aquí puedes agregar la lógica para almacenar el archivo en GridFS o solo agregar el documento como referencia
                nuevoTicket.AgregarDocumento(documento);
            }

            // Enviar el ticket
            shell.CrearTicket(nuevoTicket);

            // Limpiar el formulario después de enviar el ticket
            ClearForm();
            ticketPadre = null;
            TicketHijoLabel.IsVisible =false;
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

                // Mostrar el nombre del archivo
                ArchivoNombre.Text = result.FileName;

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
                CrearControlesDocumento(result);

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
        private void CrearControlesDocumento(FileResult result)
        {
            // Crear un StackLayout dinámico para cada documento
            var stackLayoutDocumento = new StackLayout { Orientation = StackOrientation.Vertical, Spacing = 10 };

            // Crear el ImageButton para el documento
            var imageButton = new ImageButton
            {
                WidthRequest = 50,
                HeightRequest = 50
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

        // Método para manejar la descarga del documento
        private async void OnDescargarDocumentoClicked(FileResult result)
        {
            
        }

        // Método para limpiar el formulario después de enviar el ticket
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

            // Habilitar el botón de selección de archivo si estaba deshabilitado
            SeleccionarArchivoButton.IsEnabled = true;

            // Limpiar la interfaz gráfica de documentos seleccionados
            DocumentosSeleccionadosLayout.Children.Clear();
        }

        // Método que maneja la selección de tipo de error para actualizar las categorías
        private void OnTipoErrorSelected(object sender, EventArgs e)
        {
            // Limpiar las opciones actuales de CategoriaPicker
            CategoriaPicker.Items.Clear();

            // Obtener el tipo de error seleccionado
            string tipoErrorSeleccionado = TipoErrorPicker.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(tipoErrorSeleccionado) && categoriasPorTipoError.ContainsKey(tipoErrorSeleccionado))
            {
                // Obtener las categorías correspondientes al tipo de error
                List<string> categorias = categoriasPorTipoError[tipoErrorSeleccionado];

                // Agregar las categorías al Picker de Categoria
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
