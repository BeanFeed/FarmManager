using DAL.Context;
using DAL.Entities;
using FarmManagerBackend.Exceptions;
using FarmManagerBackend.Models.Printer;
using Microsoft.EntityFrameworkCore;

namespace FarmManagerBackend.Services;

public class PrinterService
{
    private readonly ManagerContext _managerContext;
    
    public PrinterService(ManagerContext managerContext)
    {
        _managerContext = managerContext;
    }

    public async Task AddPrinter(CreatePrinterModel printerData)
    {
        Location? location = await _managerContext.Locations.FindAsync(printerData.LocationName);
        if (location is null) throw new PrinterException("Location not found");

        Printer? ePrinter = await _managerContext.Printers.FindAsync(printerData.Name);
        if (ePrinter is not null) throw new PrinterException($"Printer \"{printerData.Name}\" already exists");
        
        Printer printer = new Printer()
        {
            Name = printerData.Name,
            SerialNumber = printerData.SerialNumber,
            Brand = printerData.Brand,
            Model = printerData.Model,
            PurchaseDate = printerData.PurchaseDate,
            Location = location
        };

        await _managerContext.Printers.AddAsync(printer);
        await _managerContext.SaveChangesAsync();
    }

    public async Task RemovePrinter(string name)
    {
        Printer? printer = await _managerContext.Printers.FindAsync(name);
        if (printer is null) throw new PrinterException("Printer not found");

        _managerContext.Printers.Remove(printer);
        await _managerContext.SaveChangesAsync();
    }

    public async Task ModifyPrinter(ModifyPrinterModel printerData)
    {
        Printer? printer = await _managerContext.Printers.FindAsync(printerData.Name);
        if (printer is null) throw new PrinterException("Printer not found");

        if (printerData.SerialNumber is not null)
        {
            printer.SerialNumber = printerData.SerialNumber;
        }
        
        if (printerData.Brand is not null)
        {
            printer.Brand = printerData.Brand;
        }

        if (printerData.Model is not null)
        {
            printer.Model = printerData.Model;
        }

        if (printerData.LocationName is not null)
        {
            Location? location = await _managerContext.Locations.FindAsync(printerData.LocationName);
            if (location is null) throw new PrinterException($"Location \"{printerData.LocationName}\" not found");

            printer.Location = location;
        }

        if (printerData.PurchaseDate is not null)
        {
            printer.PurchaseDate = printerData.PurchaseDate.Value;
        }

        _managerContext.Printers.Update(printer);

        await _managerContext.SaveChangesAsync();

    }
    
    public async Task<Printer> GetPrinter(string name)
    {
        Printer? printer = await _managerContext.Printers.FindAsync(name);
        if (printer is null) throw new PrinterException("Printer not found");
        return printer;
    }

    public async Task<Printer[]> GetPrinters(string? name)
    {
        Printer[] printers;
        if (name is null) printers = await _managerContext.Printers.ToArrayAsync();
        else printers = await _managerContext.Printers.Where(x => x.Name.Contains(name)).ToArrayAsync();
        return printers;
    }
}