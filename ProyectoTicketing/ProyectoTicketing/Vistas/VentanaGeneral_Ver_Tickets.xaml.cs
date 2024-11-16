using Microsoft.Maui.Controls;
using ProyectoTicketing.Clases;
using System.Collections.ObjectModel;

namespace ProyectoTicketing.Vistas;

public partial class VentanaGeneral_Ver_Tickets : ContentPage
{
	private AppShell shell;
    public ObservableCollection<Ticket> Tickets { get; set; }
    public VentanaGeneral_Ver_Tickets(AppShell shell)
	{
		InitializeComponent();
		this.shell = shell;
        Tickets = new ObservableCollection<Ticket>();
        BindingContext = this;
    }
    public async Task CargarTicketsAsync()
    {
        // Suponiendo que tienes un método para obtener los tickets desde la base de datos
        List<Ticket> tickets = await shell.ObtenerTicketsDeUsuarioAsync();  // Método para obtener los tickets de la base de datos

        // Limpiar cualquier dato previo en la UI
        Tickets.Clear();

        // Añadir cada ticket a la colección
        foreach (Ticket ticket in tickets)
        {
            Tickets.Add(ticket);

            // Este paso asegura que la UI se actualice de manera fluida mientras cargamos los tickets
            await Task.Yield();  // Liberamos el hilo de UI para que pueda renderizar los tickets uno por uno
        }
    }
    private async void OnTicketSelected(object sender, SelectionChangedEventArgs e)
    {
        // Obtiene el ticket seleccionado
        Ticket ticketSeleccionado = e.CurrentSelection.FirstOrDefault() as Ticket;

        if (ticketSeleccionado != null)
        {
            // Aquí puedes hacer lo que desees con el ticket seleccionado
            // Por ejemplo, mostrar los detalles del ticket o navegar a otra página con la información del ticket

            // Ejemplo: Mostrar un mensaje con el nombre del ticket
            await DisplayAlert("Ticket seleccionado", $"Nombre del ticket: {ticketSeleccionado.NombreTicket}", "OK");

            // O podrías navegar a una página de detalles de ese ticket
            // Ejemplo de navegación (si tienes una página de detalles de ticket):
            // await Navigation.PushAsync(new TicketDetailPage(ticketSeleccionado));
        }
        shell.MostrarDetalles(ticketSeleccionado);
        shell.RedirigirPaginaDetalles();
    }

}