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
        private VentanaUsuario_Ver_Tickets ventanaUsuario_Ver_Tickets;
        private VentanaUsuario_Creador_Tickets ventanaUsuario_Creador_Tickets;
        private Ayuda ventana_Ayuda;
        private Usuario usuario = new Usuario();
        public bool cargado = false;
        public ToolbarItem Desconectar = new ToolbarItem();

        public AppShell()
        {
            InitializeComponent();

            ventana_iniciodeSesion = new Ventana_IniciodeSesion(this);
            InicioSesion.Content = ventana_iniciodeSesion;

            ventanaUsuario_Ver_Tickets = new VentanaUsuario_Ver_Tickets(this);
            ListaTickets.Content = ventanaUsuario_Ver_Tickets;

            ventanaUsuario_Creador_Tickets=new VentanaUsuario_Creador_Tickets(this);
            CrearTicket.Content = ventanaUsuario_Creador_Tickets;

            ventana_Ayuda = new Ayuda(this);
            Ventana_Ayuda.Content = ventana_Ayuda;

            configuracion = new Configuracion(this);

            BBDD = new BBDD();
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
            if (BBDD.ComprobarUsuario(Nombre, Contrasena) == true)
            {
                DisplayAlert("Conectado", "Se Inicio Sesion Correctamente", "OK");

                InicioSesion.IsVisible = false;
                ListaTickets.IsVisible = true;
                CrearTicket.IsVisible = true;



                Desconectar.Order = ToolbarItemOrder.Primary;
                Desconectar.Clicked += Desconectar_Clicked;
                this.ToolbarItems.Add(Desconectar);

                await Shell.Current.GoToAsync("//ListaTickets");

                usuario.Nombre = Nombre;

                BBDDsesion = true;



                Footer.Text = usuario.Nombre;

                if (BBDD.ExisteConfiguracion())
                {
                    RecuperarConfiguracion();
                }
                Desconectar.Text = (string)App.Current.Resources["Desconectar"];

                if (BBDD.ComprobarRolUsuario() == 0)
                {
                    //Ventana_Admin.IsVisible = true;
                    //ActualizarDatosAdmin();
                }

            }

            else
            {
                DisplayAlert("Error", "No se Inicio Sesion Correctamente", "OK");
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
                CrearTicket.IsVisible = false;
                ventana_iniciodeSesion.LimpiarDatos();
                
                Shell.Current.ToolbarItems.Remove(desconectar);
                
                usuario.Nombre = "";
                BBDDsesion = false;
                
                Footer.Text = "";
                BBDD.CerrarSesion();
                configuracion.AsignarConfiguracion(BBDD.SacarConfiguracion());
                Desconectar.IsEnabled = true;
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
            throw new NotImplementedException();
        }

        internal void GuardarConfiguracion(int Tema, int Idioma, double Fuente)
        {
 
            BBDD.GuardarConfiguracion(Tema,Idioma,Fuente);
            DisplayAlert("Guardado","Se ha Guardado la configuración","Ok");
        }
    }
}
