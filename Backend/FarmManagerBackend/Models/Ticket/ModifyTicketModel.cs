namespace FarmManagerBackend.Models.Ticket;

public class ModifyTicketModel
{
    public int Id { get; set; }
    public int? OpenedBy { get; set; }
    public int? Technician { get; set; }
    public bool? Reopen { get; set; }
    public string? Issue { get; set; } = null!;
    public string? Repair { get; set; } = null!;
    public string? PrinterName { get; set; } = null!;
}