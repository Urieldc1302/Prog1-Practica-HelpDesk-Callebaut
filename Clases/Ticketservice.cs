using HelpDesk.Data;
namespace HelpDesk;

public class TicketService
{
    private readonly TicketRepository _repository;

    public TicketService()
    {
        _repository = new TicketRepository("tickets.json");
    }

    public TicketService(string rutaArchivo)
    {
        _repository = new TicketRepository(rutaArchivo);
    }

    public List<Ticket> ObtenerTodos()
    {
        return _repository.ObtenerTodos();
    }

    public Ticket? ObtenerPorId(int id)
    {
        return ObtenerTodos().FirstOrDefault(t => t.Id == id);
    }

    public Ticket Crear(string titulo, string descripcion, Prioridad prioridad)
    {
        ValidarTitulo(titulo);
        ValidarDescripcion(descripcion);

        var tickets = ObtenerTodos();

        int nuevoId = tickets.Count == 0
            ? 1
            : tickets.Max(t => t.Id) + 1;

        Ticket ticket = new()
        {
            Id = nuevoId,
            Titulo = titulo,
            Descripcion = descripcion,
            Prioridad = prioridad,
            Estado = EstadoTicket.Abierto,
            FechaCreacion = DateTime.Now
        };

        tickets.Add(ticket);

        _repository.GuardarTodos(tickets);

        return ticket;
    }

    public void TomarTicket(int id)
    {
        CambiarEstado(id, EstadoTicket.Abierto, EstadoTicket.EnProceso);
    }

    public void ResolverTicket(int id)
    {
        CambiarEstado(id, EstadoTicket.EnProceso, EstadoTicket.Resuelto);
    }

    public void CerrarTicket(int id)
    {
        CambiarEstado(id, EstadoTicket.Resuelto, EstadoTicket.Cerrado);
    }

    public List<Ticket> ObtenerPorEstado(EstadoTicket estado)
    {
        return ObtenerTodos()
            .Where(t => t.Estado == estado)
            .ToList();
    }

    public List<Ticket> BuscarPorTitulo(string texto)
    {
        return ObtenerTodos()
            .Where(t => t.Titulo.Contains(texto,
                StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private void CambiarEstado(int id,
        EstadoTicket estadoActual,
        EstadoTicket nuevoEstado)
    {
        var tickets = ObtenerTodos();

        var ticket = tickets.FirstOrDefault(t => t.Id == id);

        if (ticket == null)
            return;

        if (ticket.Estado == EstadoTicket.Cerrado)
            throw new InvalidOperationException(
                "No puede modificarse un ticket cerrado.");

        if (ticket.Estado != estadoActual)
            throw new InvalidOperationException(
                "Transición de estado inválida.");

        ticket.Estado = nuevoEstado;

        _repository.GuardarTodos(tickets);
    }

    private void ValidarTitulo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("Título obligatorio.");

        if (titulo.Length > 100)
            throw new ArgumentException("Máximo 100 caracteres.");
    }

    private void ValidarDescripcion(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("Descripción obligatoria.");
    }
}