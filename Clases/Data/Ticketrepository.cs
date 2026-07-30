using HelpDesk;
using Newtonsoft.Json;

namespace HelpDesk.Data;

public class TicketRepository
{
    private readonly string _rutaArchivo;

    public TicketRepository(string rutaArchivo)
    {
        _rutaArchivo = rutaArchivo;
    }

    public List<Ticket> ObtenerTodos()
    {
        if (!File.Exists(_rutaArchivo))
            return new List<Ticket>();

        var json = File.ReadAllText(_rutaArchivo);

        if (string.IsNullOrWhiteSpace(json))
            return new List<Ticket>();

        return JsonConvert.DeserializeObject<List<Ticket>>(json)
               ?? new List<Ticket>();
    }

    public void GuardarTodos(List<Ticket> tickets)
    {
        var json = JsonConvert.SerializeObject(tickets, Formatting.Indented);

        File.WriteAllText(_rutaArchivo, json);
    }
}