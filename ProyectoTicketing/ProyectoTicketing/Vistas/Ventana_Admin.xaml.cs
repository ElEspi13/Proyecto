using ProyectoTicketing.Clases;

namespace ProyectoTicketing.Vistas
{
    /// <summary>
    /// Clase parcial que representa la ventana de administraci�n en la aplicaci�n.
    /// </summary>
    public partial class Ventana_Admin : ContentPage
    {
        private AppShell shell;

        /// <summary>
        /// Constructor de la clase Ventana_Admin.
        /// </summary>
        /// <param name="shell">Instancia de AppShell.</param>
        public Ventana_Admin(AppShell shell)
        {
            InitializeComponent();
            this.shell = shell;
        }

        /// <summary>
        /// M�todo para mostrar la lista de usuarios en la interfaz.
        /// </summary>
        /// <param name="usuarios">Lista de usuarios a mostrar.</param>
        public void MostrarListaUsuarios(List<Usuario> usuarios)
        {
            usuariosListView.ItemsSource = usuarios;
        }

        /// <summary>
        /// Evento que se dispara cuando se toca un elemento de la lista de usuarios.
        /// Elimina el usuario seleccionado llamando al m�todo EliminarUsuario del objeto shell.
        /// </summary>
        private void usuariosListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ListView listView = sender as ListView;
            Usuario usuario = listView.SelectedItem as Usuario;
            shell.EliminarUsuario(usuario);
        }

        /// <summary>
        /// Cambia el tama�o de la fuente de varios elementos de la interfaz de usuario.
        /// </summary>
        /// <param name="factorMultiplicador">El factor por el cual se multiplicar� el tama�o original de la fuente.</param>
        public void CambiarTamanoFuente(double factorMultiplicador)
        {
            Titulo.FontSize = 30 * factorMultiplicador;
        }
    }
}