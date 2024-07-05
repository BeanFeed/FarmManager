
using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class Printer
{
    [Key]
    public string Name { get; set; } = null!;
    [Required]
    public string SerialNumber { get; set; } = null!;
    [Required]
    public string Brand { get; set; } = null!;
    [Required]
    public string Model { get; set; } = null!;
    [Required]
    public Location Location { get; set; } = null!;
    [Required]
    public DateTime PurchaseDate { get; set; }

}