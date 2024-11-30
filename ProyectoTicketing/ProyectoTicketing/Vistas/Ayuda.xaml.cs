

namespace ProyectoTicketing.Vistas;

/// <summary>
/// Representa la página de Ayuda de la aplicación.
/// </summary>
public partial class Ayuda : ContentPage
{
    private AppShell shell;
    private double fuenteOriginal = 30;

    /// <summary>
    /// Constructor de la clase Ayuda.
    /// </summary>
    /// <param name="shell">Instancia de AppShell.</param>
    public Ayuda(AppShell shell)
    {
        InitializeComponent();
        this.shell = shell;
        InicializarFontSizeBase();
    }

    /// <summary>
    /// Muestra las secciones según el rol del usuario.
    /// </summary>
    /// <param name="rol">El rol del usuario (0: Admin, 1: Técnico, 2: Usuario).</param>
    public void MostrarSeccionesSegunRol(int rol)
    {
        LayoutUsuario.IsVisible = rol == 2;
        LayoutTecnico.IsVisible = rol == 1;
        LayoutAdmin.IsVisible = rol == 0;
    }

    private Dictionary<Label, double> fontSizeBase = new Dictionary<Label, double>();

    /// <summary>
    /// Inicializa la fuente base.
    /// </summary>
    private void InicializarFontSizeBase()
    {
        foreach (var elemento in LayoutUsuario.Children.Concat(LayoutTecnico.Children).Concat(LayoutAdmin.Children))
        {
            if (elemento is Label label && !fontSizeBase.ContainsKey(label))
            {
                fontSizeBase[label] = label.FontSize; 
            }
        }
    }



    /// <summary>
    /// Cambia el tamaño de la fuente de varios elementos de la interfaz de usuario.
    /// </summary>
    /// <param name="factorMultiplicador">El factor por el cual se multiplicará el tamaño original de la fuente.</param>
    public void CambiarTamanoFuente(double factorMultiplicador)
    {
        foreach (var elemento in LayoutUsuario.Children.Concat(LayoutTecnico.Children).Concat(LayoutAdmin.Children))
        {
            if (elemento is Label label && fontSizeBase.TryGetValue(label, out double baseFontSize))
            {
                label.FontSize = baseFontSize * factorMultiplicador;
            }
        }
    }
}
