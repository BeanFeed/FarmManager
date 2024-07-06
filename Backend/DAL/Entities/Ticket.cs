using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public class Ticket
{
    [Key]
    public int TicketId { get; set; }
    [Required]
    public int OpenedBy { get; set; }
    public int? Technician { get; set; }
    [Required]
    public DateTime DateOpened { get; set; }
    public DateTime? DateClosed { get; set; }
    [Required]
    public string Issue { get; set; } = null!;
    public string? Repair { get; set; } = null!;
    [Required]
    public string Printer { get; set; } = null!;
}