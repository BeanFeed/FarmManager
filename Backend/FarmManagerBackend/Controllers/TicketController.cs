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
    [Authorize(Role = Roles.Owner)]
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
    [Authorize(Role = Roles.Owner)]
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

    [HttpPost]
    [Authorize(Role = Roles.Owner)]
    public async Task<IActionResult> AddIssueType(string issue, string repair)
    {
        try
        {
            await _ticketService.AddIssueType(issue, repair);
        }
        catch (TicketException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("Added Issue");
    }

    [HttpDelete]
    [Authorize(Role = Roles.Owner)]
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
    public async Task<IActionResult> GetIssueType()
    {
        return Ok(await _ticketService.GetIssueTypes());
    }

    [HttpPost]
    public async Task<IActionResult> OpenTicket(CreateTicketModel ticketData)
    {
        try
        {
            await _ticketService.OpenTicket(ticketData);
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
    [Authorize(Role = Roles.HeadTech)]
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
    [Authorize(Role = Roles.HeadTech)]
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
    public async Task<IActionResult> GetTickets(string printerName)
    {
        return Ok(await _ticketService.GetTickets(printerName));
    }
}