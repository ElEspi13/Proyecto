using ProyectoTicketing.Clases;
using ProyectoTicketing.Servicios;

namespace ProyectoTicketing.Vistas
{
    /// <summary>
    /// Clase parcial que representa la ventana de administraci�n en la aplicaci�n.
    /// Permite ver y gestionar usuarios.
    /// </summary>
    public partial class Ventana_Admin : ContentPage
    {
        private AppShell shell;

        /// <summary>
        /// Constructor de la clase Ventana_Admin.
        /// </summary>
        /// <param name="shell">Instancia de AppShell que gestiona la l�gica de navegaci�n y acciones principales.</param>
        public Ventana_Admin(AppShell shell)
        {
            InitializeComponent();
            this.shell = shell;
        }

        /// <summary>
        /// Muestra la lista de usuarios en la interfaz.
        /// </summary>
        /// <param name="usuarios">Lista de usuarios a mostrar en la vista.</param>
        public void MostrarListaUsuarios(List<Usuario> usuarios)
        {
            usuariosListView.ItemsSource = usuarios;
        }

        /// <summary>
        /// Evento que se dispara cuando se toca un elemento de la lista de usuarios.
        /// Elimina el usuario seleccionado llamando al m�todo EliminarUsuario del objeto shell.
        /// </summary>
        private async void usuariosListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ListView listView = sender as ListView;
            Usuario usuario = listView.SelectedItem as Usuario;
            shell.EliminarUsuario(usuario);
            List<Usuario> lista = await shell.MostrarListaUsuarios();
        }

        /// <summary>
        /// Cambia el tama�o de la fuente de varios elementos de la interfaz de usuario.
        /// </summary>
        /// <param name="factorMultiplicador">El factor por el cual se multiplicar� el tama�o original de la fuente.</param>
        public void CambiarTamanoFuente(double factorMultiplicador)
        {
            Titulo.FontSize = 30 * factorMultiplicador;
        }

        /// <summary>
        /// Evento que se dispara cuando la p�gina est� a punto de aparecer.
        /// Carga la lista de usuarios desde el servicio correspondiente.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                List<Usuario> lista = await shell.MostrarListaUsuarios();
                MostrarListaUsuarios(lista);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Ocurri� un problema al cargar la p�gina.", "OK");
            }
        }
        
    }
}
