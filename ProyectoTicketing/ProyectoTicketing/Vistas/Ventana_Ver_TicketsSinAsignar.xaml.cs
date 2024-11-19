using System.Collections.ObjectModel;
using ProyectoTicketing.Clases;
namespace ProyectoTicketing.Vistas;

public partial class Ventana_Ver_TicketsSinAsignar : ContentPage
{
    private AppShell shell;
    public ObservableCollection<Ticket> Tickets { get; set; }
    public Ventana_Ver_TicketsSinAsignar(AppShell shell)
    {
        InitializeComponent();
        this.shell = shell;
        Tickets = new ObservableCollection<Ticket>();
        BindingContext = this;
    }
    private bool _cargandoTickets = false;

    public async Task CargarTicketsAsync()
    {
        if (_cargandoTickets) return; // Evita que se ejecute si ya está cargando

        _cargandoTickets = true;

        try
        {
            Tickets.Clear();
            List<Ticket> tickets = await shell.ObtenerTicketsSinAsignarAsync();
            foreach (Ticket ticket in tickets)
            {
                if (!Tickets.Any(t => t.IdTicket == ticket.IdTicket)) // Verifica por ID u otra propiedad única
                {
                    Tickets.Add(ticket);
                }
                await Task.Yield();
            }

        }
        finally
        {
            _cargandoTickets = false; // Libera el flag al terminar
        }
    }

    private async void OnTicketSelected(object sender, ItemTappedEventArgs e)
    {
        if (e.Item == null)
            return; // Verifica que un item haya sido tocado

        Ticket ticketSeleccionado = e.Item as Ticket;

        if (ticketSeleccionado != null)
        {
            await DisplayAlert("Ticket seleccionado", $"Nombre del ticket: {ticketSeleccionado.NombreTicket}", "OK");


                // Mostrar detalles y redirigir a otra página
                shell.MostrarDetalles(ticketSeleccionado);
                shell.RedirigirPaginaDetalles();
            shell.PaginaDetallesTecnico();
        }
    }
}