
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
        private Ventana_Admin ventana_Admin;
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

            ventana_Admin= new Ventana_Admin(this);
            Admin.Content = ventana_Admin;
            
            

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
                        Admin.IsVisible = true;
                        InicioSesion.IsVisible = true;
                        ventana_iniciodeSesion.VistaAdmin(true);
                        ventana_Admin.MostrarListaUsuarios(await BBDD.ListaUsuarios());
                        await Shell.Current.GoToAsync("//InicioSesionRuta");

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

                    Ventana_Ayuda.IsVisible = true;
                    ventana_Ayuda.MostrarSeccionesSegunRol(BBDD.ComprobarRolUsuario());
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
                await DisplayAlert("Informacion", "Cerrando sesion", "OK" );
                ToolbarItem desconectar = sender as ToolbarItem;
                InicioSesion.IsVisible = true;
                
                ListaTickets.IsVisible = false;
                CreadorTickets.IsVisible = false;
                ventana_iniciodeSesion.LimpiarDatos();
                TecnicoResolver.IsVisible = false;
                ListaTicketsSinAsignar.IsVisible = false;
                Ventana_Ayuda.IsVisible = false;
                ventana_iniciodeSesion.VistaAdmin(false);
                Admin.IsVisible = false;
                Shell.Current.ToolbarItems.Remove(desconectar);
                DetallesTicket.IsVisible = false;
                await Shell.Current.GoToAsync("//InicioSesionRuta");
                usuario.Nombre = "";    
                BBDDsesion = false;
                
                Footer.Text = "";
                BBDD.CerrarSesion();
                configuracion.AsignarConfiguracion(BBDD.SacarConfiguracion());
                Desconectar.IsEnabled = true;
                BBDD.DetenerMonitoreo();
                ventana_Ayuda.MostrarSeccionesSegunRol(-1);
                
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
            ventanaDetallesTicket.CambiarTamanoFuente(value);
            ventana_Ayuda.CambiarTamanoFuente(value);
            ventana_Admin.CambiarTamanoFuente(value);
            ventanaTecnico_Resolvedor.CambiarTamanoFuente(value);
            ventanaUsuario_Creador_Tickets.CambiarTamanoFuente(value);


        }

        /// <summary>
        /// Inserta los datos de un usuario en la base de datos.
        /// </summary>
        /// <param name="Usuario">Nombre del usuario.</param>
        /// <param name="Passwd">Contraseña del usuario.</param>
        /// <param name="selectedIndex">Índice seleccionado para la configuración del usuario.</param>
        internal void InsertarDatos(string Usuario, string Passwd, int selectedIndex)
        {
            BBDD.RegistrarUsuario(Usuario, Passwd, selectedIndex);
        }

        /// <summary>
        /// Guarda la configuración del usuario (tema, idioma y tamaño de fuente).
        /// </summary>
        /// <param name="Tema">Tema seleccionado para la interfaz.</param>
        /// <param name="Idioma">Idioma seleccionado.</param>
        /// <param name="Fuente">Tamaño de la fuente seleccionada.</param>
        internal async void GuardarConfiguracion(int Tema, int Idioma, double Fuente)
        {
            BBDD.GuardarConfiguracion(Tema, Idioma, Fuente);
            await DisplayAlert("Guardado", "Se ha Guardado la configuración", "Ok");
        }

        /// <summary>
        /// Crea un nuevo ticket y lo guarda en la base de datos.
        /// </summary>
        /// <param name="nuevoTicket">Objeto Ticket que contiene los detalles del nuevo ticket.</param>
        internal async void CrearTicket(Ticket nuevoTicket)
        {
            try
            {
                await BBDD.SubirTicketAsync(nuevoTicket);
                await DisplayAlert("Guardado", "Ticket Se ha Creado Correctamente", "Ok");
                await Shell.Current.GoToAsync("//ListaTickets");
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", "Ticket NO Se ha Creado Correctamente", "Ok");
            }
        }

        /// <summary>
        /// Obtiene la lista de tickets asociados a un usuario.
        /// </summary>
        /// <returns>Lista de tickets del usuario.</returns>
        internal async Task<List<Ticket>> ObtenerTicketsDeUsuarioAsync()
        {
            try
            {
                List<Ticket> tickets = await BBDD.ObtenerTicketsDeUsuarioAsync();

                if (tickets == null || tickets.Count == 0)
                {
                    await DisplayAlert("Información", "No se encontraron tickets para este usuario.", "Ok");
                    return new List<Ticket>();
                }

                return tickets;
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", $"Tickets no se pueden cargar: {e.Message}", "Ok");
                return new List<Ticket>();
            }
        }

        /// <summary>
        /// Muestra los detalles de un ticket seleccionado.
        /// </summary>
        /// <param name="ticketSeleccionado">Ticket a mostrar.</param>
        internal void MostrarDetalles(Ticket? ticketSeleccionado)
        {
            ventanaDetallesTicket.SetTicketData(ticketSeleccionado);
        }

        /// <summary>
        /// Descarga un documento asociado a un ticket.
        /// </summary>
        /// <param name="documento">Documento a descargar.</param>
        internal void DescargarDocumento(Documento documento)
        {
            BBDD.DescargarDocumentoAsync(documento);
        }

        /// <summary>
        /// Redirige a la página de detalles del ticket.
        /// </summary>
        internal async void RedirigirPaginaDetalles()
        {
            DetallesTicket.IsVisible = true;
            await Shell.Current.GoToAsync("//DetallesTicket");
        }

        /// <summary>
        /// Actualiza los tickets en tiempo real en la vista general.
        /// </summary>
        internal async void ActualizarTicketsTiempoReal()
        {
            await ventanaGeneral_Ver_Tickets.CargarTicketsAsync();
        }

        /// <summary>
        /// Crea un ticket hijo asociado a un ticket padre.
        /// </summary>
        /// <param name="IDTicketPadre">ID del ticket padre al que se vincula el nuevo ticket hijo.</param>
        internal async void CrearTicketHijo(string IDTicketPadre)
        {
            
            ventanaUsuario_Creador_Tickets.GuardarTicketPadre(IDTicketPadre);
            await Shell.Current.GoToAsync("//CreadorTickets");
        }

        /// <summary>
        /// Cierra los tickets asociados a un ticket padre.
        /// </summary>
        /// <param name="iDTicketPadre">ID del ticket padre para cerrar los tickets asociados.</param>
        internal void CerrarTicketsIDTicketPadre(string iDTicketPadre)
        {
            BBDD.CerrarTicketsIDTicketPadre(iDTicketPadre);
        }

        /// <summary>
        /// Cierra un ticket específico por su ID.
        /// </summary>
        /// <param name="idTicket">ID del ticket a cerrar.</param>
        internal void CerrarTicketsIDTicket(ObjectId idTicket)
        {
            BBDD.CerrarTicketsIDTicket(idTicket);
        }

        /// <summary>
        /// Obtiene la lista de tickets que no han sido asignados a ningún técnico.
        /// </summary>
        /// <returns>Lista de tickets sin asignar.</returns>
        internal async Task<List<Ticket>> ObtenerTicketsSinAsignarAsync()
        {
            try
            {
                List<Ticket> tickets = await BBDD.ObtenerTicketsDeSinAsignarAsync();

                if (tickets == null || tickets.Count == 0)
                {
                    await DisplayAlert("Información", "No se encontraron tickets para asignar a este usuario.", "Ok");
                    return new List<Ticket>();
                }

                return tickets;
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", $"Tickets no se pueden cargar: {e.Message}", "Ok");
                return new List<Ticket>();
            }
        }

        /// <summary>
        /// Cambia la vista a la página de detalles del ticket para técnicos.
        /// </summary>
        internal void PaginaDetallesTecnico()
        {
            ventanaDetallesTicket.VistaTecnico();
        }

        /// <summary>
        /// Actualiza los tickets en tiempo real para los técnicos.
        /// </summary>
        internal async void ActualizarTicketsTiempoRealTecnico()
        {
            try
            {
                await ventana_Ver_TicketsSinAsignar.CargarTicketsAsync();
                await ventanaGeneral_Ver_Tickets.CargarTicketsAsync();
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// Asigna un ticket a un técnico específico.
        /// </summary>
        /// <param name="ticketID">ID del ticket a asignar al técnico.</param>
        internal async void AsignarTicketATecnico(ObjectId ticketID)
        {
            if (await BBDD.ActualizarIDTecnicoAsync(ticketID) == true)
            {
                await Shell.Current.GoToAsync("//ListaTickets");
                await DisplayAlert("Información", "Ticket Asignado.", "Ok");
            }
            else
            {
                await DisplayAlert("Información", "Ticket no se ha asignado", "Ok");
            }
        }

        /// <summary>
        /// Redirige a la página de solución del ticket para un técnico.
        /// </summary>
        /// <param name="ticketSeleccionado">Ticket seleccionado que el técnico resolverá.</param>
        internal async void TecnicoResolvedor(Ticket ticketSeleccionado)
        {
            ventanaTecnico_Resolvedor.SetTicketData(ticketSeleccionado);
            await DisplayAlert("Información", "Redirigiendo a Pagina Resolvedora.", "Ok");
            await Shell.Current.GoToAsync("//TecnicoResolver");
        }

        /// <summary>
        /// Actualiza la solución de un ticket y lo guarda.
        /// </summary>
        /// <param name="idTicket">ID del ticket a actualizar.</param>
        /// <param name="Solucion">Solución proporcionada por el técnico.</param>
        /// <param name="documentosSeleccionados">Lista de documentos asociados al ticket.</param>
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

        /// <summary>
        /// Elimina un usuario de la base de datos.
        /// </summary>
        /// <param name="usuario">Usuario que se va a eliminar.</param>
        internal async void EliminarUsuario(Usuario usuario)
        {
            try
            {
                await BBDD.EliminarDatosUsuario(usuario);
                await MostrarListaUsuarios();
                await DisplayAlert("Completado", "Se Elimino el usuario.", "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "No se pudo eliminar al usuario.", "Ok");
            }
        }

        /// <summary>
        /// Obtiene la lista de usuarios registrados.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        internal async Task<List<Usuario>> MostrarListaUsuarios()
        {
            return await BBDD.ListaUsuarios();
        }

    }
}
