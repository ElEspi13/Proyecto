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
        TituloAyuda.FontSize = fuenteOriginal * factorMultiplicador;
        AccionLabel.FontSize = fuenteOriginal * factorMultiplicador;
        NoUsuarioLabel.FontSize = fuenteOriginal * factorMultiplicador;
        UsuarioLabel.FontSize = fuenteOriginal * factorMultiplicador;
        CreadorPersonajeLabel.FontSize = fuenteOriginal * factorMultiplicador;
        SiLabel1.FontSize = fuenteOriginal * factorMultiplicador;
        SiLabel2.FontSize = fuenteOriginal * factorMultiplicador;
        GuardarPersonajeLabel.FontSize = fuenteOriginal * factorMultiplicador;
        NoLabel.FontSize = fuenteOriginal * factorMultiplicador;
        SiLabel3.FontSize = fuenteOriginal * factorMultiplicador;
        GuardarConfiguracionLabel.FontSize = fuenteOriginal * factorMultiplicador;
        NoLabel2.FontSize = fuenteOriginal * factorMultiplicador;
        SiLabel4.FontSize = fuenteOriginal * factorMultiplicador;

    }
}