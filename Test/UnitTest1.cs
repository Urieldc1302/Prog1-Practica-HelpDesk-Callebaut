using HelpDesk;
namespace HelpDesk.Tests;

public class TicketServiceTests
{
    private string _archivo = "";

    [SetUp]
    public void Setup()
    {
        _archivo = Path.GetTempFileName();

        if (File.Exists(_archivo))
            File.Delete(_archivo);
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_archivo))
            File.Delete(_archivo);
    }

    [Test]
    public void CrearTicket_Valido_QuedaAbierto()
    {
        var service = new TicketService(_archivo);

        var ticket = service.Crear(
            "Error",
            "No inicia",
            Prioridad.Alta);

        Assert.That(ticket.Estado, Is.EqualTo(EstadoTicket.Abierto));
    }

    [Test]
    public void CrearTicket_TituloVacio_LanzaExcepcion()
    {
        var service = new TicketService(_archivo);

        Assert.Throws<ArgumentException>(() =>
            service.Crear("", "Desc", Prioridad.Baja));
    }

    [Test]
    public void CrearTicket_TituloMuyLargo_LanzaExcepcion()
    {
        var service = new TicketService(_archivo);

        string titulo = new string('A', 101);

        Assert.Throws<ArgumentException>(() =>
            service.Crear(titulo, "Desc", Prioridad.Baja));
    }

    [Test]
    public void CrearTicket_DescripcionVacia_LanzaExcepcion()
    {
        var service = new TicketService(_archivo);

        Assert.Throws<ArgumentException>(() =>
            service.Crear("Titulo", "", Prioridad.Media));
    }

    [Test]
    public void CambiarEstado_FlujoCompleto_TerminaCerrado()
    {
        var service = new TicketService(_archivo);

        var ticket = service.Crear(
            "Error",
            "Desc",
            Prioridad.Alta);

        service.TomarTicket(ticket.Id);
        service.ResolverTicket(ticket.Id);
        service.CerrarTicket(ticket.Id);

        var actualizado = service.ObtenerPorId(ticket.Id);

        Assert.That(actualizado!.Estado,
            Is.EqualTo(EstadoTicket.Cerrado));
    }

    [Test]
    public void CerrarTicket_DesdeAbierto_LanzaExcepcion()
    {
        var service = new TicketService(_archivo);

        var ticket = service.Crear(
            "Error",
            "Desc",
            Prioridad.Alta);

        Assert.Throws<InvalidOperationException>(() =>
            service.CerrarTicket(ticket.Id));
    }

    [Test]
    public void ModificarTicketCerrado_LanzaExcepcion()
    {
        var service = new TicketService(_archivo);

        var ticket = service.Crear(
            "Error",
            "Desc",
            Prioridad.Alta);

        service.TomarTicket(ticket.Id);
        service.ResolverTicket(ticket.Id);
        service.CerrarTicket(ticket.Id);

        Assert.Throws<InvalidOperationException>(() =>
            service.TomarTicket(ticket.Id));
    }

    [Test]
    public void BuscarPorId_Inexistente_DevuelveNull()
    {
        var service = new TicketService(_archivo);

        Assert.That(service.ObtenerPorId(999), Is.Null);
    }

    [Test]
    public void ObtenerPorEstado_FiltraCorrectamente()
    {
        var service = new TicketService(_archivo);

        var t1 = service.Crear("A", "Desc", Prioridad.Baja);
        var t2 = service.Crear("B", "Desc", Prioridad.Alta);

        service.TomarTicket(t2.Id);

        var abiertos = service.ObtenerPorEstado(EstadoTicket.Abierto);

        Assert.That(abiertos.Count, Is.EqualTo(1));
    }

    [Test]
    public void BuscarPorTitulo_EncuentraCoincidencias()
    {
        var service = new TicketService(_archivo);

        service.Crear("Error de login", "Desc", Prioridad.Media);
        service.Crear("Impresora", "Desc", Prioridad.Media);

        var resultado = service.BuscarPorTitulo("login");

        Assert.That(resultado.Count, Is.EqualTo(1));
    }
}