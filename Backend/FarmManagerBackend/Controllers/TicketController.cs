using DAL.Entities;
using FarmManagerBackend.Exceptions;
using FarmManagerBackend.Filters;
using FarmManagerBackend.Models.Ticket;
using FarmManagerBackend.Models.User;
using FarmManagerBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagerBackend.Controllers;

[ApiController]
[Route("v1/[controller]/[action]")]
[Authenticate]
public class TicketController : ControllerBase
{
    private readonly TicketService _ticketService;
    
    public TicketController(TicketService ticketService)
    {
        _ticketService = ticketService;
    }
    
    [HttpPost]
    [Authorize(Role = Roles.HeadTechnician)]
    public async Task<IActionResult> AddLocation(string Name)
    {
        try
        {
            await _ticketService.AddLocation(Name);
        }
        catch (TicketException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("Location added");
    }

    [HttpDelete]
    [Authorize(Role = Roles.HeadTechnician)]
    public async Task<IActionResult> RemoveLocation(string Name)
    {
        try
        {
            await _ticketService.RemoveLocation(Name);
        }
        catch (TicketException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("Removed location");
    }

    [HttpGet]
    public async Task<IActionResult> GetLocations()
    {
        return Ok(await _ticketService.GetLocations());
    }

    [HttpPost]
    [Authorize(Role = Roles.HeadTechnician)]
    public async Task<IActionResult> AddIssueType(AddIssueTypeModel issueType)
    {
        try
        {
            await _ticketService.AddIssueType(issueType.Issue, issueType.Repair);
        }
        catch (TicketException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("Added Issue");
    }

    [HttpDelete]
    [Authorize(Role = Roles.HeadTechnician)]
    public async Task<IActionResult> RemoveIssueType(string issue, string repair)
    {
        try
        {
            await _ticketService.RemoveIssueType(issue, repair);
        }
        catch (TicketException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("Removed Issue");
    }

    [HttpGet]
    public async Task<IActionResult> GetIssueTypes()
    {
        return Ok(await _ticketService.GetIssueTypes());
    }

    [HttpGet]
    public async Task<IActionResult> GetIssueVariants(string? repairByIssue)
    {
        if (repairByIssue != null)
        {
            return Ok(await _ticketService.GetIssueVariants(repairByIssue));
        }
        else
        {
            return Ok(await _ticketService.GetIssueVariants());
        }
    }

    [HttpPost]
    public async Task<IActionResult> OpenTicket(CreateTicketModel ticketData)
    {
        try
        {
            User user = (User)HttpContext.Items["User"]!;
            await _ticketService.OpenTicket(ticketData, user.Id);
        }
        catch (TicketException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("Opened ticket");
    }

    [HttpPost]
    [Authorize(Role = Roles.Technician)]
    public async Task<IActionResult> CloseTicket(CloseTicketModel ticketData)
    {
        try
        {
            User user = (User)HttpContext.Items["User"]!;
            await _ticketService.CloseTicket(ticketData, user.Id);
        }
        catch (TicketException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("Closed ticket");
    }

    [HttpDelete]
    [Authorize(Role = Roles.HeadTechnician)]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        try
        {
            await _ticketService.DeleteTicket(id);
        }
        catch (TicketException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("Ticket deleted");
    }

    [HttpPost]
    [Authorize(Role = Roles.HeadTechnician)]
    public async Task<IActionResult> ModifyTicket(ModifyTicketModel ticketData)
    {
        try
        {
            await _ticketService.ModifyTicket(ticketData);
        }
        catch (TicketException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("Modified ticket");
    }

    [HttpGet]
    [Authorize(Role = Roles.Technician)]
    public async Task<IActionResult> GetTickets(string? printerName, bool? onlyOpen, bool? sortDescending)
    {
        return Ok(await _ticketService.GetTickets(printerName, onlyOpen, sortDescending));
    }
}