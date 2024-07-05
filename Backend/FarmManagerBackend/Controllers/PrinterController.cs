using FarmManagerBackend.Exceptions;
using FarmManagerBackend.Filters;
using FarmManagerBackend.Models.Printer;
using FarmManagerBackend.Models.User;
using FarmManagerBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagerBackend.Controllers;

[ApiController]
[Route("v1/[controller]/[action]")]
[Authenticate]
public class PrinterController : ControllerBase
{
    private readonly PrinterService _printerService;
    
    public PrinterController(PrinterService printerService)
    {
        _printerService = printerService;
    }

    [HttpPost]
    [Authorize(Role = Roles.Owner)]
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
    [Authorize(Role = Roles.Owner)]
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
    [Authorize(Role = Roles.Owner)]
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
    public async Task<IActionResult> GetPrinters(string name)
    {
        try
        {
            return Ok(await _printerService.GetPrinters(name));
        }
        catch (PrinterException e)
        {
            return BadRequest(e.Message);
        }
    }
}