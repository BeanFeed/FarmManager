using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public class Ticket
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public User OpenedBy { get; set; } = null!;
    public User? Technician { get; set; } = null!;
    [Required]
    public DateTime DateOpened { get; set; }
    public DateTime? DateClosed { get; set; }
    [Required]
    public string Issue { get; set; } = null!;
    public string? Repair { get; set; } = null!;
    [Required]
    public Printer Printer { get; set; } = null!;
}