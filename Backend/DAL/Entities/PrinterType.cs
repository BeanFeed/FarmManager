using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class PrinterType
{
    [Key]
    public int PrinterTypeId { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    
}