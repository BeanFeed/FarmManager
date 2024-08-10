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

        await CloseOpenTickets(printer.Name);
        
        _managerContext.Printers.Remove(printer);
        await _managerContext.SaveChangesAsync();
    }

    public async Task AddPrinterType(PrinterTypeModel printerType)
    {
        bool exists =
            await _managerContext.PrinterTypes.AnyAsync(x =>
                x.Brand == printerType.Brand && x.Model == printerType.Model);
        if (exists) throw new PrinterException("Model already exists");

        PrinterType pType = new PrinterType()
        {
            Brand = printerType.Brand,
            Model = printerType.Model
        };

        await _managerContext.PrinterTypes.AddAsync(pType);
        await _managerContext.SaveChangesAsync();
    }

    public async Task RemovePrinterType(int id)
    {
        PrinterType? printerType = await _managerContext.PrinterTypes.FindAsync(id);
        if (printerType is null) throw new PrinterException("Printer Type not found");

        _managerContext.PrinterTypes.Remove(printerType);
        await _managerContext.SaveChangesAsync();
    }

    public async Task<PrinterType[]> GetPrinterTypes()
    {
        PrinterType[] printerTypes = await _managerContext.PrinterTypes.ToArrayAsync();
        return printerTypes;
    }

    public async Task<string[]> GetPrinterTypeVariants(string? modelByBrand = null)
    {
        List<PrinterType> printerTypes = await _managerContext.PrinterTypes.OrderBy(x => x.Brand).ToListAsync();
        List<string> returnList = new List<string>();
        if (modelByBrand != null)
        {
            foreach (PrinterType printerType in printerTypes)
            {
                if(printerType.Brand == modelByBrand) returnList.Add(printerType.Model);
            }
        }
        else
        {
            foreach (PrinterType printerType in printerTypes)
            {
                if(!returnList.Contains(printerType.Brand)) returnList.Add(printerType.Brand);
            }
        }

        return returnList.ToArray();
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
        Printer? printer = await _managerContext.Printers.Include(printer => printer.Location).Where(x => x.Name == name).FirstOrDefaultAsync();
        if (printer is null) throw new PrinterException("Printer not found");
        return printer;
    }

    public async Task<Printer[]> GetPrinters(string? name)
    {
        Printer[] printers;
        if (name is null) printers = await _managerContext.Printers.Include(printer => printer.Location).ToArrayAsync();
        else printers = await _managerContext.Printers.Where(x => x.Name.Contains(name))
            .Include(printer => printer.Location).ToArrayAsync();
        return printers;
    }
    
    private async Task CloseOpenTickets(string printerName)
    {
        Ticket[] tickets = await _managerContext.Tickets.Where(x => x.Printer == printerName && x.DateClosed == null).ToArrayAsync();
        
        for (int i = 0; i < tickets.Length; i++)
        {
            tickets[i].DateClosed = DateTime.Now;
            tickets[i].Repair = "Auto closed because printer was deleted.";

            _managerContext.Tickets.Update(tickets[i]);
        }

        await _managerContext.SaveChangesAsync();
    }
}