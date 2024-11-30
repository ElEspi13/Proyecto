


namespace ProyectoTicketing.Vistas;
/// <summary>
/// Clase parcial que representa la ventana de inicio de sesión en la aplicación.
/// </summary>
public partial class Ventana_IniciodeSesion : ContentPage
{
    private AppShell Shell;
    private double FuenteOriginal = 30;

    /// <summary>
    /// Constructor de la clase Ventana_IniciodeSesion.
    /// </summary>
    /// <param name="Shell">Instancia de AppShell.</param>
    public Ventana_IniciodeSesion(AppShell Shell)
    {
        InitializeComponent();
        this.Shell = Shell;
    }

    /// <summary>
    /// Método interno para limpiar los datos de entrada en la interfaz.
    /// </summary>
    internal void LimpiarDatos()
    {
        Usuario.Text = "";
        Passw.Text = "";
        Rol.SelectedIndex = -1;
    }

    /// <summary>
    /// Evento que se dispara cuando se hace clic en el botón de inicio de sesión.
    /// Llama al método ConectarBBDD del objeto Shell, pasando los datos ingresados por el usuario.
    /// </summary>
    private void Login_Clicked(object sender, EventArgs e)
    {
        Login.IsEnabled = false;

        Shell.ConectarBBDD(Usuario.Text, Passw.Text);
        Login.IsEnabled = true;
        LimpiarDatos();
    }

    /// <summary>
    /// Evento que se dispara cuando se hace clic en el botón de registro.
    /// Llama al método InsertarDatos del objeto Shell, pasando los datos ingresados por el usuario.
    /// </summary>
    private async void Registrar_Clicked(object sender, EventArgs e)
    {
        if (Usuario.Text == "" && Passw.Text == "" && Rol.SelectedIndex == -1)
        {
            await DisplayAlert("Error", "Error al crear la cuenta", "OK");
        }
        else
        {
            Shell.InsertarDatos(Usuario.Text, Passw.Text, Rol.SelectedIndex);
            LimpiarDatos();
            
        }
    }

    /// <summary>
    /// Evento que se dispara cuando se necesita limpiar los datos de entrada en la interfaz.
    /// </summary>
    private void LimpiarDatos(object sender, EventArgs e)
    {
        LimpiarDatos();
    }

    /// <summary>
    /// Cambia el tamaño de la fuente de varios elementos de la interfaz de usuario.
    /// </summary>
    /// <param name="factorMultiplicador">El factor por el cual se multiplicará el tamaño original de la fuente.</param>
    public void CambiarTamanoFuente(double factorMultiplicador)
    {
        UsuarioLabel.FontSize= 30*factorMultiplicador;
        Usuario.FontSize= 30*factorMultiplicador;
        ContrasenaLabel.FontSize = 30 * factorMultiplicador;
        Passw.FontSize= 30*factorMultiplicador;
        Login.FontSize = 30*factorMultiplicador;
        Registrar.FontSize = 30*factorMultiplicador;
        Rol.FontSize = 30*factorMultiplicador;

    }

    /// <summary>
    /// Ajusta la visibilidad de los elementos de la interfaz según si el usuario es administrador.
    /// </summary>
    /// <param name="Admin">Indica si el usuario es administrador.</param>
    public void VistaAdmin(bool Admin)
    {
        if (Admin == true)
        {
            Registrar.IsVisible = true;
            Login.IsVisible = false;
            Rol.IsVisible = true;
        }
        else
        {
            Registrar.IsVisible = false;
            Login.IsVisible = true;
            Rol.IsVisible = false;
        }
    }

}
