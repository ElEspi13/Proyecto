
using Microsoft.Maui.Controls;
using MongoDB.Bson;
using ProyectoTicketing.Clases;
using ProyectoTicketing.Servicios;
using ProyectoTicketing.Vistas;

namespace ProyectoTicketing
{
    public partial class AppShell : Shell
    {
        public bool BBDDEstado = false;
        public bool BBDDsesion = false;
        private BBDD BBDD;
        private Configuracion configuracion;
        private Ventana_IniciodeSesion ventana_iniciodeSesion;
        private VentanaGeneral_Ver_Tickets ventanaGeneral_Ver_Tickets;
        private VentanaUsuario_Creador_Tickets ventanaUsuario_Creador_Tickets;
        private VentanaTecnico_ResolvedorTickets ventanaTecnico_Resolvedor;
        private Ventana_DetallesTicket ventanaDetallesTicket;
        private Ventana_Ver_TicketsSinAsignar ventana_Ver_TicketsSinAsignar;
        private Ayuda ventana_Ayuda;
        private Usuario usuario = new Usuario();
        public bool cargado = false;
        public ToolbarItem Desconectar = new ToolbarItem();

        public AppShell()
        {
            InitializeComponent();

            ventana_iniciodeSesion = new Ventana_IniciodeSesion(this);
            InicioSesion.Content = ventana_iniciodeSesion;

            ventanaGeneral_Ver_Tickets = new VentanaGeneral_Ver_Tickets(this);
            ListaTickets.Content = ventanaGeneral_Ver_Tickets;

            ventanaUsuario_Creador_Tickets=new VentanaUsuario_Creador_Tickets(this);
            CreadorTickets.Content = ventanaUsuario_Creador_Tickets;

            ventanaDetallesTicket = new Ventana_DetallesTicket(this);
            DetallesTicket.Content = ventanaDetallesTicket;

            ventanaTecnico_Resolvedor =new VentanaTecnico_ResolvedorTickets(this);
            TecnicoResolver.Content = ventanaTecnico_Resolvedor;

            ventana_Ayuda = new Ayuda(this);
            Ventana_Ayuda.Content = ventana_Ayuda;
            
            ventana_Ver_TicketsSinAsignar =new Ventana_Ver_TicketsSinAsignar(this);
            ListaTicketsSinAsignar.Content= ventana_Ver_TicketsSinAsignar;

            configuracion = new Configuracion(this);

            BBDD = new BBDD(this);
        }

        /// <summary>
        /// Método para conectar a la base de datos y comprobar si existe el ususario y activa todos los elementos del usuario logueado.
        /// Y si es admin que se active la ventana de admin.
        /// </summary>
        /// <param name="Nombre">Nombre de usuario.</param>
        /// <param name="Contrasena">Contraseña del usuario.</param>
        public async void ConectarBBDD(String Nombre, String Contrasena)
        {
            
            if (BBDDEstado == false)
            {
                BBDDEstado = BBDD.Conexion();
            }
            if (BBDDEstado == true)
            {
                if (BBDD.ComprobarUsuario(Nombre, Contrasena) == true)
                {
                    await DisplayAlert("Conectado", "Se Inicio Sesion Correctamente", "OK");

                    InicioSesion.IsVisible = false;
                    


                    Desconectar = new ToolbarItem();
                    Desconectar.Order = ToolbarItemOrder.Primary;
                    Desconectar.Clicked += Desconectar_Clicked;
                    this.ToolbarItems.Add(Desconectar);
                    
                    

                    usuario.Nombre = Nombre;

                    BBDDsesion = true;

                    

                    Footer.Text = usuario.Nombre;

                    if (BBDD.ExisteConfiguracion())
                    {
                        RecuperarConfiguracion();
                    }
                    Desconectar.Text = (string)App.Current.Resources["Desconectar"];

                    if (BBDD.ComprobarRolUsuario() == 1)
                    {
                        ActualizarTicketsTiempoRealTecnico();
                        ListaTicketsSinAsignar.IsVisible = true;
                        TecnicoResolver.IsVisible = true;
                        DetallesTicket.IsVisible = true;
                        ListaTickets.IsVisible = true;
                        DetallesTicket.IsVisible = false;
                        ventanaGeneral_Ver_Tickets.tecnico = true;
                        BBDD.IniciarMonitoreoTecnico();
                        await Shell.Current.GoToAsync("//ListaTicketsSinAsignar");

                    }
                    if (BBDD.ComprobarRolUsuario() == 0)
                    {
                        //Ventana_Admin.IsVisible = true;
                        //ActualizarDatosAdmin();
                        
                    }
                    if (BBDD.ComprobarRolUsuario() == 2)
                    {
                        ventanaGeneral_Ver_Tickets.tecnico = false;
                        ventanaGeneral_Ver_Tickets.CargarTicketsAsync();
                        ListaTickets.IsVisible = true;
                        CreadorTickets.IsVisible = true;
                        BBDD.IniciarMonitoreoUsuario();
                        await Shell.Current.GoToAsync("//ListaTickets");
                    }
                    

                }

                else
                {
                    await DisplayAlert("Error", "No se Inicio Sesion Correctamente", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "No se Inicio Sesion Correctamente", "OK");
            }


        }

        /// <summary>
        /// Recupera la configuración de la base de datos y la asigna al objeto de configuración.
        /// </summary>
        private void RecuperarConfiguracion()
        {
            configuracion.AsignarConfiguracion(BBDD.SacarConfiguracion());
        }

        /// <summary>
        /// Método que se ejecuta al hacer clic en el botón de configuración y acceder a esa ventana.
        /// </summary>
        /// <param name="sender">El objeto que desencadenó el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void ToolbarItemConfiguracion_Clicked(object sender, EventArgs e)
        {
            IniciarPagina(configuracion);
            FlyoutBehavior = FlyoutBehavior.Disabled;
        }

        /// <summary>
        /// Inicia una nueva página.
        /// </summary>
        /// <param name="pagina">La página a iniciar.</param>
        /// <returns>Tarea asincrónica.</returns>
        public async Task IniciarPagina(ContentPage pagina)
        {
            await Navigation.PushAsync(pagina);
        }

        /// <summary>
        /// Método que se ejecuta al hacer clic en el botón de desconectar(Cierra la sesion) y desabilita los servicios de un usuario logueado.
        /// </summary>
        /// <param name="sender">El objeto que desencadenó el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private async void Desconectar_Clicked(object sender, EventArgs e)
        {
            Desconectar.IsEnabled = false;
            bool confirmacion;
            if (confirmacion = await DisplayAlert("Confirmar", "¿Estás seguro de que deseas cerrar sesión los datos no guardados se eliminaran?", "Sí", "No"))
            {
                ToolbarItem desconectar = sender as ToolbarItem;
                InicioSesion.IsVisible = true;
                ListaTickets.IsVisible = false;
                CreadorTickets.IsVisible = false;
                ventana_iniciodeSesion.LimpiarDatos();
                Shell.Current.ToolbarItems.Remove(desconectar);
                DetallesTicket.IsVisible = false;
                usuario.Nombre = "";    
                BBDDsesion = false;
                
                Footer.Text = "";
                BBDD.CerrarSesion();
                configuracion.AsignarConfiguracion(BBDD.SacarConfiguracion());
                Desconectar.IsEnabled = true;
                BBDD.DetenerMonitoreo();
                await Shell.Current.GoToAsync("//InicioSesionRuta");
            }
            else { Desconectar.IsEnabled = true; }

        }



        /// <summary>
        /// Cambia el tamaño de la fuente en todas las páginas de la aplicación.
        /// </summary>
        /// <param name="value">El valor por el cual se multiplicará el tamaño de la fuente.</param>
        internal void CambiarFuentesPaginas(double value)
        {

            ventana_iniciodeSesion.CambiarTamanoFuente(value);


        }

        internal void InsertarDatos(string Usuario, string Passwd)
        {
            BBDD.RegistrarUsuario(Usuario,Passwd);
        }

        internal async void GuardarConfiguracion(int Tema, int Idioma, double Fuente)
        {
 
            BBDD.GuardarConfiguracion(Tema,Idioma,Fuente);
            await DisplayAlert("Guardado","Se ha Guardado la configuración","Ok");
        }

        internal async void CrearTicket(Ticket nuevoTicket)
        {
            try
            {
                await BBDD.SubirTicketAsync(nuevoTicket);
                await DisplayAlert("Guardado", "Ticket Se ha Creado Correctamente", "Ok");
                await Shell.Current.GoToAsync("//ListaTickets");
            }
            catch (Exception e) {
                await DisplayAlert("Error", "Ticket NO Se ha Creado Correctamente", "Ok");
            }
            
        }

        internal async Task<List<Ticket>> ObtenerTicketsDeUsuarioAsync()
        {
            try
            {
                // Llama al método de la base de datos y espera el resultado
                List<Ticket> tickets = await BBDD.ObtenerTicketsDeUsuarioAsync();

                if (tickets == null || tickets.Count == 0)
                {
                    await DisplayAlert("Información", "No se encontraron tickets para este usuario.", "Ok");
                    return new List<Ticket>(); // Devuelve una lista vacía si no hay tickets
                }

                return tickets; // Devuelve los tickets si los hay
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", $"Tickets no se pueden cargar: {e.Message}", "Ok");
                return new List<Ticket>(); // Devuelve una lista vacía en caso de error
            }
        }

        internal void MostrarDetalles(Ticket? ticketSeleccionado)
        {
            ventanaDetallesTicket.SetTicketData(ticketSeleccionado);
            
        }

        internal void DescargarDocumento(Documento documento)
        {
            BBDD.DescargarDocumentoAsync(documento);
        }

        internal async void RedirigirPaginaDetalles()
        {
            DetallesTicket.IsVisible = true;
            await Shell.Current.GoToAsync("//DetallesTicket");
            
        }

        internal void ActualizarTicketsTiempoReal()
        {
            ventanaGeneral_Ver_Tickets.CargarTicketsAsync();
        }

        internal async void CrearTicketHijo(string IDTicketPadre)
        {
            ventanaUsuario_Creador_Tickets.GuardarTicketPadre(IDTicketPadre);
            await Shell.Current.GoToAsync("//CreadorTickets");

        }

        internal void CerrarTicketsIDTicketPadre(string iDTicketPadre)
        {
            BBDD.CerrarTicketsIDTicketPadre(iDTicketPadre);
        }

        internal void CerrarTicketsIDTicket(ObjectId idTicket)
        {
            BBDD.CerrarTicketsIDTicket(idTicket);
        }

        internal async Task<List<Ticket>> ObtenerTicketsSinAsignarAsync()
        {
            try
            {
                // Llama al método de la base de datos y espera el resultado
                List<Ticket> tickets = await BBDD.ObtenerTicketsDeSinAsignarAsync();

                if (tickets == null || tickets.Count == 0)
                {
                    await DisplayAlert("Información", "No se encontraron tickets para asignar a este usuario.", "Ok");
                    return new List<Ticket>(); // Devuelve una lista vacía si no hay tickets
                }

                return tickets; // Devuelve los tickets si los hay
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", $"Tickets no se pueden cargar: {e.Message}", "Ok");
                return new List<Ticket>(); // Devuelve una lista vacía en caso de error
            }
        }

        internal void PaginaDetallesTecnico()
        {
            ventanaDetallesTicket.VistaTecnico();
        }

        internal async void ActualizarTicketsTiempoRealTecnico()
        {
            try
            {
                ventana_Ver_TicketsSinAsignar.CargarTicketsAsync();
                ventanaGeneral_Ver_Tickets.CargarTicketsAsync();
            }
            catch (Exception e) { 
            
            
            }
            
        }

        internal async void AsignarTicketATecnico(ObjectId ticketID)
        {
            if (await BBDD.ActualizarIDTecnicoAsync(ticketID) == true)
            {
                await Shell.Current.GoToAsync("//ListaTickets");
                await DisplayAlert("Información", "Ticket Asignado.", "Ok");
            }
            else {

                await DisplayAlert("Información", "Ticket no se ha asignado", "Ok");

            }


            

        }

        internal async void TecnicoResolvedor(Ticket ticketSeleccionado)
        {
            ventanaTecnico_Resolvedor.SetTicketData(ticketSeleccionado);
            await DisplayAlert("Información", "Redirigiendo a Pagina Resolvedora.", "Ok");
            await Shell.Current.GoToAsync("//TecnicoResolver");
        }

        internal async void ActualizarTecnicoTicket(ObjectId idTicket, string Solucion, List<Documento> documentosSeleccionados)
        {
            try
            {
                BBDD.ActualizacionTecnicoAsync(idTicket, Solucion, documentosSeleccionados);
                await DisplayAlert("Información", "Se actualizaron los datos.", "Ok");
                await Shell.Current.GoToAsync("//ListaTickets");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "No pudieron actualizar los datos.", "Ok");
            }
            
        }

        internal void EliminarUsuario(Usuario? usuario)
        {
            throw new NotImplementedException();
        }
    }
}
