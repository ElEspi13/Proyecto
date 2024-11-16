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
            var result = await FilePicker.Default.PickAsync();
            if (result != null && IsSupportedFileType(result.FileName))
            {
                ArchivoEntry.Text = result.FileName;
                ArchivoSeleccionadoLayout.IsVisible = true;
                ArchivoNombre.Text = result.FileName;
                ArchivoIcono.Source = "archivo_icono.png";
            }
            else
            {
                await DisplayAlert("Error", "File type not supported.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private bool IsSupportedFileType(string fileName)
    {
        string[] supportedExtensions = { ".pdf", ".docx", ".txt", ".xlsx" };
        return supportedExtensions.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }

}