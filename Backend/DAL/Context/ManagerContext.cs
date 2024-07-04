using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context;

public class ManagerContext : DbContext
{
    public DbSet<Printer> Printers { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }
}