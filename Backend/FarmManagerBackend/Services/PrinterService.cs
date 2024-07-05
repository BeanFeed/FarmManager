using DAL.Context;
using DAL.Entities;
using FarmManagerBackend.Exceptions;

namespace FarmManagerBackend.Services;

public class PrinterService
{
    private readonly ManagerContext _managerContext;
    
    public PrinterService(ManagerContext managerContext)
    {
        _managerContext = managerContext;
    }
    
    public async Task<Printer> GetPrinter(string name)
    {
        Printer? printer = await _managerContext.Printers.FindAsync(name);
        if (printer is null) throw new PrinterException("Printer not found");
        return printer;
    }
}