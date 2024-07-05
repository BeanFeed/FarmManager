namespace FarmManagerBackend.Models.Ticket;

public class CreateTicketModel
{
    public int OpenedBy { get; set; }
    public string Issue { get; set; } = null!;
    public string PrinterName { get; set; } = null!;
}