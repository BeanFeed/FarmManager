namespace FarmManagerBackend.Models.Ticket;

public class CreateTicketModel
{
    public string Issue { get; set; } = null!;
    public string PrinterName { get; set; } = null!;
}