
using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class Printer
{
    [Key]
    public string Name { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string Location { get; set; } = null!;
    public DateTime PurchaseDate { get; set; }

}