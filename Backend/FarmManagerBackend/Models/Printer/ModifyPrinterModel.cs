namespace FarmManagerBackend.Models.Printer;

public class ModifyPrinterModel
{
    public string Name { get; set; } = null!;
    public string? SerialNumber { get; set; } = null!;
    public string? Brand { get; set; } = null!;
    public string? Model { get; set; } = null!;
    public string? LocationName { get; set; } = null!;
    public DateTime? PurchaseDate { get; set; }
}