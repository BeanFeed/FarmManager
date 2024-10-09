using FarmManagerBackend.Exceptions;
using FarmManagerBackend.Filters;
using FarmManagerBackend.Models.Printer;
using FarmManagerBackend.Models.User;
using FarmManagerBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagerBackend.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authenticate]
public class PrinterController : ControllerBase
{
    private readonly PrinterService _printerService;
    
    public PrinterController(PrinterService printerService)
    {
        _printerService = printerService;
    }

    [HttpPost]
    [Authorize(Role = Roles.HeadTechnician)]
    public async Task<IActionResult> AddPrinter(CreatePrinterModel printerData)
    {
        try
        {
            await _printerService.AddPrinter(printerData);
        }
        catch (PrinterException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("Added printer");
    }

    [HttpDelete]
    [Authorize(Role = Roles.HeadTechnician)]
    public async Task<IActionResult> RemovePrinter(string name)
    {
        try
        {
            await _printerService.RemovePrinter(name);
        }
        catch (PrinterException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("Removed printer");
    }

    [HttpPost]
    [Authorize(Role = Roles.HeadTechnician)]
    public async Task<IActionResult> AddPrinterType(PrinterTypeModel printerType)
    {
        try
        {
            await _printerService.AddPrinterType(printerType);
        }
        catch (PrinterException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("Printer Type added");
    }
    
    [HttpDelete]
    [Authorize(Role = Roles.HeadTechnician)]
    public async Task<IActionResult> RemovePrinterType(int id)
    {
        try
        {
            await _printerService.RemovePrinterType(id);
        }
        catch (PrinterException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("Removed Printer Type");
    }

    [HttpGet]
    [Authorize(Role = Roles.HeadTechnician)]
    public async Task<IActionResult> GetPrinterTypes()
    {
        return Ok(await _printerService.GetPrinterTypes());
    }

    [HttpGet]
    [Authorize(Role = Roles.HeadTechnician)]
    public async Task<IActionResult> GetPrinterTypeVariants(string? modelByBrand)
    {
        if (modelByBrand != null)
        {
            return Ok(await _printerService.GetPrinterTypeVariants(modelByBrand));
        }
        else
        {
            return Ok(await _printerService.GetPrinterTypeVariants());
        }
    }

    [HttpPost]
    [Authorize(Role = Roles.HeadTechnician)]
    public async Task<IActionResult> ModifyPrinter(ModifyPrinterModel printerData)
    {
        try
        {
            await _printerService.ModifyPrinter(printerData);
        }
        catch (PrinterException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("Modified printer");
    }

    [HttpGet]
    public async Task<IActionResult> GetPrinter(string name)
    {
        try
        {
            return Ok(await _printerService.GetPrinter(name));
        }
        catch (PrinterException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPrinters(string? name, bool? byRoom)
    {
        try
        {
            if(byRoom is true) return Ok(await _printerService.GetPrinters(name, byRoom.Value));
            return Ok(await _printerService.GetPrinters(name));
        }
        catch (PrinterException e)
        {
            return BadRequest(e.Message);
        }
    }
}