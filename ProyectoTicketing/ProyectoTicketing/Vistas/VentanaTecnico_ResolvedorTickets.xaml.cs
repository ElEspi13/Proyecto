using Microsoft.Maui.Controls;

namespace ProyectoTicketing.Vistas;

public partial class VentanaTecnico_ResolvedorTickets : ContentPage
{
    private AppShell shell;
    public VentanaTecnico_ResolvedorTickets(AppShell shell)
    {
		InitializeComponent();
        this.shell = shell;
    }
    private async void OnSeleccionarArchivoClicked(object sender, EventArgs e)
    {
        try
        {
            // Usamos el FilePicker para seleccionar un archivo
            var result = await FilePicker.PickAsync();

            if (result != null)
            {
                // Mostrar el nombre del archivo en el Entry
                ArchivoEntry.Text = result.FileName;

                // Establecemos el nombre del archivo en el Label debajo de la imagen
                ArchivoNombre.Text = result.FileName;

                // Determinamos el icono basado en la extensión del archivo
                string extension = Path.GetExtension(result.FileName).ToLower();
                if (extension == ".doc" || extension == ".docx")
                {
                    // Usa un icono de Word si el archivo es .doc o .docx
                    ArchivoIcono.Source = "icono_word.png"; // Asegúrate de que 'icono_word.png' esté en tus recursos
                }
                else
                {
                    // Usa un icono genérico o según el tipo de archivo
                    ArchivoIcono.Source = "icono_generico.png";
                }

                // Mostrar el layout de archivo seleccionado
                ArchivoSeleccionadoLayout.IsVisible = true;

                // Lógica adicional para guardar el archivo en la base de datos...
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo abrir el archivo: {ex.Message}", "OK");
        }
    }
}