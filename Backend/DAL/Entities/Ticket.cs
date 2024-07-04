using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public class Ticket
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public User OpenedBy { get; set; } = null!;
    public User Technician { get; set; } = null!;
    public DateTime DateOpened { get; set; }
    public DateTime DateClosed { get; set; }
    public string Repair { get; set; } = null!;
    public Printer Printer { get; set; } = null!;
}