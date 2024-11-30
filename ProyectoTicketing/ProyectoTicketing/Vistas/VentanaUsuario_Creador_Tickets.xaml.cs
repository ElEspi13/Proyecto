using MongoDB.Bson;
using ProyectoTicketing.Clases;
using SharpCompress.Archives;

namespace ProyectoTicketing.Vistas
{
    public partial class VentanaUsuario_Creador_Tickets : ContentPage
    {
        private AppShell shell;
        private const int LIMITE_DOCUMENTOS = 3;
        private List<Documento> documentosSeleccionados = new List<Documento>();
        private int contadorDocumentos = 0;
        private string ticketPadre = null;
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

        /// <summary>
        /// Maneja el evento de "ENVIAR" para crear un nuevo ticket y enviarlo.
        /// </summary>
        private async void OnEnviarClicked(object sender, EventArgs e)
        {
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

            foreach (var documento in documentosSeleccionados)
            {
                nuevoTicket.AgregarDocumento(documento);
            }

            shell.CrearTicket(nuevoTicket);

            ClearForm();
            ticketPadre = null;
            TicketHijoLabel.IsVisible = false;
        }

        /// <summary>
        /// Permite seleccionar un archivo para agregarlo al ticket.
        /// </summary>
        private async void OnSeleccionarArchivoClicked(object sender, EventArgs e)
        {
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
                ArchivoNombre.Text = result.FileName;

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

                CrearControlesDocumento(result);

                contadorDocumentos++;

                if (contadorDocumentos >= LIMITE_DOCUMENTOS)
                {
                    SeleccionarArchivoButton.IsEnabled = false;
                    await DisplayAlert("Límite de Documentos", "Ya has seleccionado 3 documentos. No puedes agregar más.", "OK");
                }
            }
        }

        /// <summary>
        /// Crea dinámicamente los controles para mostrar la información del documento seleccionado.
        /// </summary>
        private void CrearControlesDocumento(FileResult result)
        {
            var stackLayoutDocumento = new StackLayout { Orientation = StackOrientation.Vertical, Spacing = 10 };

            var imageButton = new ImageButton
            {
                WidthRequest = 50,
                HeightRequest = 50
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

            var labelDocumento = new Label
            {
                Text = result.FileName,
                FontSize = 16,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                TextColor = ArchivoNombre.TextColor
            };

            stackLayoutDocumento.Children.Add(imageButton);
            stackLayoutDocumento.Children.Add(labelDocumento);

            DocumentosSeleccionadosLayout.Children.Add(stackLayoutDocumento);

            imageButton.Clicked += (sender, e) => OnDescargarDocumentoClicked(result);
        }

        /// <summary>
        /// Maneja la descarga del documento seleccionado.
        /// </summary>
        private async void OnDescargarDocumentoClicked(FileResult result)
        {
            // Lógica para descargar el documento
        }

        /// <summary>
        /// Limpia el formulario después de enviar el ticket.
        /// </summary>
        private void ClearForm()
        {
            NombreTicketEntry.Text = "";
            TipoErrorPicker.SelectedItem = null;
            CategoriaPicker.SelectedItem = null;
            Descripcion.Text = string.Empty;
            ArchivoEntry.Text = string.Empty;
            ArchivoSeleccionadoLayout.IsVisible = false;
            documentosSeleccionados.Clear();
            contadorDocumentos = 0;

            SeleccionarArchivoButton.IsEnabled = true;
            DocumentosSeleccionadosLayout.Children.Clear();
        }

        /// <summary>
        /// Maneja la selección del tipo de error para actualizar las categorías disponibles.
        /// </summary>
        private void OnTipoErrorSelected(object sender, EventArgs e)
        {
            CategoriaPicker.Items.Clear();

            string tipoErrorSeleccionado = TipoErrorPicker.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(tipoErrorSeleccionado) && categoriasPorTipoError.ContainsKey(tipoErrorSeleccionado))
            {
                List<string> categorias = categoriasPorTipoError[tipoErrorSeleccionado];

                foreach (string categoria in categorias)
                {
                    CategoriaPicker.Items.Add(categoria);
                }
            }
        }

        /// <summary>
        /// Guarda el identificador de un ticket padre para asociar un ticket hijo.
        /// </summary>
        internal void GuardarTicketPadre(string iDTicketPadre)
        {
            this.ticketPadre = iDTicketPadre;
            TicketHijoLabel.IsVisible = true;
            
        }

        /// <summary>
        /// Limpia el identificador del ticket padre cuando la ventana desaparece.
        /// </summary>
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            ticketPadre = null;
            TicketHijoLabel.IsVisible = false;
        }
        /// <summary>
        /// Cambia el tamaño de la fuente de varios elementos de la interfaz de usuario.
        /// </summary>
        /// <param name="factorMultiplicador">El factor por el cual se multiplicará el tamaño original de la fuente.</param>
        public void CambiarTamanoFuente(double factorMultiplicador)
        {
            TicketHijoLabel.FontSize = 18 * factorMultiplicador;
            NombreTicketLabel.FontSize = 16 * factorMultiplicador;
            TipoErrorLabel.FontSize = 16 * factorMultiplicador;
            CategoriaLabel.FontSize = 16 * factorMultiplicador;
            SeleccionarArchivoLabel.FontSize = 16 * factorMultiplicador;
            NombreTicketEntry.FontSize = 16 * factorMultiplicador;
            ArchivoEntry.FontSize = 16 * factorMultiplicador;
            TipoErrorPicker.FontSize = 16 * factorMultiplicador;
            CategoriaPicker.FontSize = 16 * factorMultiplicador;
            Descripcion.FontSize = 16 * factorMultiplicador;
            EnviarButton.FontSize = 20 * factorMultiplicador;
            SeleccionarArchivoButton.FontSize = 16 * factorMultiplicador;
        
        }
    }
}
