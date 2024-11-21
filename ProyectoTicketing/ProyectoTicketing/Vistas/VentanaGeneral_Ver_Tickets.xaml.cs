
using Microsoft.Maui.Controls;
using ProyectoTicketing.Clases;
using System.Collections.ObjectModel;

namespace ProyectoTicketing.Vistas;

public partial class VentanaGeneral_Ver_Tickets : ContentPage
{
	private AppShell shell;
    public ObservableCollection<Ticket> Tickets { get; set; }
    public bool tecnico {  get; set; }
    
    public VentanaGeneral_Ver_Tickets(AppShell shell)
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
            var tickets = await shell.ObtenerTicketsDeUsuarioAsync() ?? new List<Ticket>();


            MainThread.BeginInvokeOnMainThread(() =>
            {
                Tickets.Clear(); // Asegura que esta operación está en el hilo principal

                foreach (var ticket in tickets)
                {
                    if (!Tickets.Any(t => t.IdTicket == ticket.IdTicket))
                    {
                        Tickets.Add(ticket); // Modificación de la colección también en el hilo principal
                    }
                }
                OnPropertyChanged(nameof(Tickets)); // Notificar cambios si es necesario

            });
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

            if (tecnico == false)
            {
                // Mostrar detalles y redirigir a otra página
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