using DAL.Context;
using DAL.Entities;
using FarmManagerBackend.Exceptions;
using FarmManagerBackend.Models.Ticket;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;

namespace FarmManagerBackend.Services;

public class TicketService
{
    private readonly ManagerContext _managerContext;
    private readonly UserService _userService;
    private readonly PrinterService _printerService;
    
    public TicketService(ManagerContext managerContext, UserService userService, PrinterService printerService)
    {
        _managerContext = managerContext;
        _userService = userService;
        _printerService = printerService;
    }
    
    public async Task AddLocation(string name)
    {
        Location? eLocation = await _managerContext.Locations.FindAsync(name);
        if (eLocation is not null) throw new TicketException("Location already exists");

        Location location = new Location()
        {
            Name = name
        };

        await _managerContext.Locations.AddAsync(location);
        await _managerContext.SaveChangesAsync();
    }

    public async Task RemoveLocation(string name)
    {
        Location? location = await _managerContext.Locations.FindAsync(name);
        if (location is null) throw new TicketException("Location doesn't exist");

        _managerContext.Locations.Remove(location);

        await _managerContext.SaveChangesAsync();
    }

    public async Task<Location[]> GetLocations()
    {
        Location[] locations = await _managerContext.Locations.ToArrayAsync();
        return locations;
    }

    public async Task AddIssueType(string issue, string repair)
    {
        IssueType? issueType = await _managerContext.IssueTypes.Where(x => x.Issue == issue && x.Repair == repair).FirstOrDefaultAsync();
        if (issueType is not null) throw new TicketException("Issue type already exists");

        IssueType newIssue = new IssueType()
        {
            Issue = issue,
            Repair = repair
        };

        await _managerContext.IssueTypes.AddAsync(newIssue);
        await _managerContext.SaveChangesAsync();
    }

    public async Task RemoveIssueType(string issue, string repair)
    {
        IssueType? issueType = await _managerContext.IssueTypes.Where(x => x.Issue == issue && x.Repair == repair).FirstOrDefaultAsync();
        if (issueType is null) throw new TicketException("Issue type doesn't exist");

        _managerContext.IssueTypes.Remove(issueType);
        await _managerContext.SaveChangesAsync();
    }

    public async Task<IssueType[]> GetIssueTypes()
    {
        IssueType[] issueTypes = await _managerContext.IssueTypes.ToArrayAsync();
        return issueTypes;
    }
    
    public async Task OpenTicket(CreateTicketModel ticketData, int openedBy)
    {
        try
        {
            await _printerService.GetPrinter(ticketData.PrinterName);
        }
        catch (PrinterException e)
        {
            throw new TicketException(e.Message);
        }
        
        Ticket ticket = new Ticket()
        {
            Issue = ticketData.Issue,
            OpenedBy = openedBy,
            DateOpened = DateTime.Now,
            Printer = ticketData.PrinterName
        };

        await _managerContext.Tickets.AddAsync(ticket);
        await _managerContext.SaveChangesAsync();
    }

    public async Task CloseTicket(CloseTicketModel ticketData, int user)
    {
        Ticket? ticket = await _managerContext.Tickets.FindAsync(ticketData.Id);
        if (ticket is null) throw new TicketException($"Ticket {ticketData.Id} not found");
        if (ticket.DateClosed is not null) throw new TicketException($"Ticket {ticketData.Id} already closed");
        
        ticket.DateClosed = DateTime.Now;
        
        ticket.Technician = user;
        ticket.Repair = ticketData.Repair;

        _managerContext.Tickets.Update(ticket);
        await _managerContext.SaveChangesAsync();
    }

    public async Task DeleteTicket(int id)
    {
        Ticket? ticket = await _managerContext.Tickets.FindAsync(id);
        if (ticket is null) throw new TicketException($"Ticket {id} not found");
        _managerContext.Tickets.Remove(ticket);
        await _managerContext.SaveChangesAsync();
    }

    public async Task ModifyTicket(ModifyTicketModel ticketData)
    {
        Ticket? ticket = await _managerContext.Tickets.FindAsync(ticketData.Id);
        if (ticket is null) throw new TicketException($"Ticket {ticketData.Id} not found");

        if (ticketData.OpenedBy is not null)
        {
            try
            {
                await _userService.GetUser(ticketData.OpenedBy.Value);
            }
            catch (UserException e)
            {
                throw new TicketException($"User {ticketData.OpenedBy.Value} not found");
            }

            ticket.OpenedBy = ticketData.OpenedBy.Value;
        }

        if (ticketData.Technician is not null)
        {
            try
            {
                await _userService.GetUser(ticketData.Technician.Value);
            }
            catch (UserException e)
            {
                throw new TicketException($"User {ticketData.Technician.Value} not found");
            }

            ticket.Technician = ticketData.Technician.Value;
        }

        if (ticketData.Reopen is not null && ticketData.Reopen.Value)
        {
            ticket.DateClosed = null;
        }

        if (ticketData.Issue is not null)
        {
            ticket.Issue = ticketData.Issue;
        }

        if (ticketData.Repair is not null)
        {
            ticket.Repair = ticketData.Repair;
        }

        if (ticketData.PrinterName is not null)
        {
            try
            {
                await _printerService.GetPrinter(ticketData.PrinterName);
            }
            catch (PrinterException e)
            {
                throw new TicketException($"Printer {ticketData.PrinterName} not found");
            }

            ticket.Printer = ticketData.PrinterName;
        }

        _managerContext.Tickets.Update(ticket);
        await _managerContext.SaveChangesAsync();
    }
    
    public async Task<ReturnTicket[]> GetTickets(string? name, bool? onlyOpen, bool? sortDescending)
    {
        Ticket[] tickets;
        if (name is not null)
            tickets = await _managerContext.Tickets.Where(x => onlyOpen != null && onlyOpen.Value ? x.DateClosed == null && x.Printer.Contains(name) : x.Printer.Contains(name)).ToArrayAsync();
        else tickets = await _managerContext.Tickets.Where(x => onlyOpen == null || !onlyOpen.Value || x.DateClosed == null).ToArrayAsync();

        ReturnTicket[] returnTickets = new ReturnTicket[tickets.Length];
        
        for (int i = 0; i < tickets.Length; i++)
        {
            returnTickets[i] = new ReturnTicket()
            {
                TicketId = tickets[i].TicketId,
                OpenedBy = (await _userService.GetUser(tickets[i].OpenedBy)).Name,
                Technician = tickets[i].Technician != null ? (await _userService.GetUser(tickets[i].Technician!.Value)).Name : null,
                DateOpened = tickets[i].DateOpened,
                DateClosed = tickets[i].DateClosed,
                Issue = tickets[i].Issue,
                Repair = tickets[i].Repair,
                Printer = tickets[i].Printer
            };
            
        }
        if(sortDescending != null && !sortDescending.Value) Array.Reverse(returnTickets);
        return returnTickets;
    }
    
    
}