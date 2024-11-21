namespace ProyectoTicketing.Vistas;

/// <summary>
/// Representa la página de Ayuda de la aplicación.
/// </summary>
public partial class Ayuda : ContentPage
{
    private AppShell shell;
    private double fuenteOriginal=30;
    /// <summary>
    /// Constructor de la clase Ayuda.
    /// </summary>
    public Ayuda(AppShell shell)
	{
		InitializeComponent();
        this.shell = shell;
    }

    /// <summary>
    /// Cambia el tamaño de la fuente de varios elementos de la interfaz de usuario.
    /// </summary>
    /// <param name="factorMultiplicador">El factor por el cual se multiplicará el tamaño original de la fuente.</param>
    public void CambiarTamanoFuente(double factorMultiplicador)
    {
        

    }
    public void MostrarSeccionesSegunRol(int rol)
    {
        // Por ejemplo, si el rol es 'Usuario'
        LayoutUsuario.IsVisible = rol == 2;
        LayoutTecnico.IsVisible = rol == 1;
        LayoutAdmin.IsVisible = rol == 0;
    }

}