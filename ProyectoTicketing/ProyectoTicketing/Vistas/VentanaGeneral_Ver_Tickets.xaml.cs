using Microsoft.Maui.Controls;
using ProyectoTicketing.Clases;
using System.Collections.ObjectModel;

namespace ProyectoTicketing.Vistas;

public partial class VentanaGeneral_Ver_Tickets : ContentPage
{
    private AppShell shell;

    /// <summary>
    /// Colección observable de tickets para enlazar con la interfaz de usuario.
    /// </summary>
    public ObservableCollection<Ticket> Tickets { get; set; }

    /// <summary>
    /// Indica si el usuario es técnico.
    /// </summary>
    public bool tecnico { get; set; }

    /// <summary>
    /// Constructor de la ventana de visualización de tickets.
    /// </summary>
    public VentanaGeneral_Ver_Tickets(AppShell shell)
    {
        InitializeComponent();
        this.shell = shell;
        Tickets = new ObservableCollection<Ticket>();
        BindingContext = this;
    }

    private bool _cargandoTickets = false;

    /// <summary>
    /// Método asincrónico para cargar los tickets desde la fuente de datos.
    /// </summary>
    public async Task CargarTicketsAsync()
    {
        if (_cargandoTickets) return; 

        _cargandoTickets = true;

        try
        {
            var tickets = await shell.ObtenerTicketsDeUsuarioAsync() ?? new List<Ticket>();

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
    /// Método que se invoca cuando se selecciona un ticket en la lista.
    /// </summary>
    private async void OnTicketSelected(object sender, ItemTappedEventArgs e)
    {
        if (e.Item == null)
            return; 

        Ticket ticketSeleccionado = e.Item as Ticket;

        if (ticketSeleccionado != null)
        {
            await DisplayAlert("Ticket seleccionado", $"Nombre del ticket: {ticketSeleccionado.NombreTicket}", "OK");

            if (tecnico == false)
            {
                shell.MostrarDetalles(ticketSeleccionado);
                shell.RedirigirPaginaDetalles();
            }
            else
            {
                shell.TecnicoResolvedor(ticketSeleccionado);
            }
        }
    }
}
