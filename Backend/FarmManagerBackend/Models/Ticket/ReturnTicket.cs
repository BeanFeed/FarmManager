namespace FarmManagerBackend.Models.Ticket;

public class ReturnTicket
{
    public int TicketId { get; set; }
    public string OpenedBy { get; set; } = null!;
    public string? Technician { get; set; }
    public DateTime DateOpened { get; set; }
    public DateTime? DateClosed { get; set; }
    public string Issue { get; set; } = null!;
    public string? Repair { get; set; } = null!;
    public string Printer { get; set; } = null!;
}