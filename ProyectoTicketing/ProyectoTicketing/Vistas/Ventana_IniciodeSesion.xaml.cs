
namespace ProyectoTicketing.Vistas;

/// <summary>
/// Clase parcial que representa la ventana de inicio de sesi�n en la aplicaci�n.
/// </summary>
public partial class Ventana_IniciodeSesion : ContentPage
{
    private AppShell Shell;
    private double FuenteOriginal=30;
    /// <summary>
    /// Constructor de la clase Ventana_IniciodeSesion.
    /// </summary>
    /// <param name="Shell">Instancia de AppShell.</param>
    public Ventana_IniciodeSesion(AppShell Shell)
	{
		InitializeComponent();
        this.Shell=Shell;
		
	}

    /// <summary>
    /// M�todo interno para limpiar los datos de entrada en la interfaz.
    /// </summary>
    internal void LimpiarDatos()
    {
        Usuario.Text = "";
        Passw.Text = "";
    }

    /// <summary>
    /// Evento que se dispara cuando se hace clic en el bot�n de inicio de sesi�n.
    /// Llama al m�todo ConectarBBDD del objeto Shell, pasando los datos ingresados por el usuario.
    /// </summary>
    private void Login_Clicked(object sender, EventArgs e)
    {
        Login.IsEnabled = false;
        
        Shell.ConectarBBDD(Usuario.Text,Passw.Text);
        Login.IsEnabled = true;
    }

    /// <summary>
    /// Evento que se dispara cuando se hace clic en el bot�n de registro.
    /// Llama al m�todo InsertarDatos del objeto Shell, pasando los datos ingresados por el usuario.
    /// </summary>
    private void Registrar_Clicked(object sender, EventArgs e)
    {
        Shell.InsertarDatos(Usuario.Text,Passw.Text);
    }

    /// <summary>
    /// Evento que se dispara cuando se necesita limpiar los datos de entrada en la interfaz.
    /// </summary>
    private void LimpiarDatos(object sender,EventArgs e)
    {
        LimpiarDatos();
    }

    /// <summary>
    /// Cambia el tama�o de la fuente de varios elementos de la interfaz de usuario.
    /// </summary>
    /// <param name="factorMultiplicador">El factor por el cual se multiplicar� el tama�o original de la fuente.</param>
    public void CambiarTamanoFuente(double factorMultiplicador)
    {
        
    }
}