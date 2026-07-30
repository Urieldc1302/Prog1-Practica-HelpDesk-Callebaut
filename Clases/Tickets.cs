namespace HelpDesk;

public class Ticket
{
    public int Id { get; set; }

    public string Titulo { get; set; } = "";

    public string Descripcion { get; set; } = "";

    public Prioridad Prioridad { get; set; }

    public EstadoTicket Estado { get; set; }

    public DateTime FechaCreacion { get; set; }
}