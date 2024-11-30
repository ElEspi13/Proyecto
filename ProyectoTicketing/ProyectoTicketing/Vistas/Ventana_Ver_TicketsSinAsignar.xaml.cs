using System.Collections.ObjectModel;
using ProyectoTicketing.Clases;
namespace ProyectoTicketing.Vistas;

public partial class Ventana_Ver_TicketsSinAsignar : ContentPage
{
    private AppShell shell;

    /// <summary>
    /// Colección observable de tickets que se muestra en la interfaz de usuario.
    /// </summary>
    public ObservableCollection<Ticket> Tickets { get; set; }

    /// <summary>
    /// Constructor de la clase que inicializa la vista y la colección de tickets.
    /// </summary>
    /// <param name="shell">Instancia de AppShell que permite la interacción con otras vistas y funcionalidades.</param>
    public Ventana_Ver_TicketsSinAsignar(AppShell shell)
    {
        InitializeComponent();
        this.shell = shell;
        Tickets = new ObservableCollection<Ticket>();
        BindingContext = this;
    }

    private bool _cargandoTickets = false;

    /// <summary>
    /// Carga la lista de tickets sin asignar desde el backend y actualiza la interfaz de usuario.
    /// </summary>
    public async Task CargarTicketsAsync()
    {
        if (_cargandoTickets) return;

        _cargandoTickets = true;

        try
        {
            var tickets = await shell.ObtenerTicketsSinAsignarAsync() ?? new List<Ticket>();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Tickets.Clear();

                foreach (var ticket in tickets)
                {
                    if (!Tickets.Any(t => t.IdTicket == ticket.IdTicket))
                    {
                        Tickets.Add(ticket);
                    }
                }
                OnPropertyChanged(nameof(Tickets));

            });
        }
        finally
        {
            _cargandoTickets = false;
        }
    }

    /// <summary>
    /// Maneja el evento cuando un ticket es seleccionado de la lista.
    /// </summary>
    /// <param name="sender">El objeto que dispara el evento.</param>
    /// <param name="e">Los argumentos del evento.</param>
    private async void OnTicketSelected(object sender, ItemTappedEventArgs e)
    {
        if (e.Item == null)
            return;

        Ticket ticketSeleccionado = e.Item as Ticket;

        if (ticketSeleccionado != null)
        {
            await DisplayAlert("Ticket seleccionado", $"Nombre del ticket: {ticketSeleccionado.NombreTicket}", "OK");

            shell.MostrarDetalles(ticketSeleccionado);
            shell.RedirigirPaginaDetalles();
            shell.PaginaDetallesTecnico();
        }
    }
}
