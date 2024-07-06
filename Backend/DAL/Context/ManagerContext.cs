using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context;

public class ManagerContext : DbContext
{
    public ManagerContext() {}

    public ManagerContext(DbContextOptions<ManagerContext> options) : base(options) {}

    public DbSet<Printer> Printers { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<IssueType> IssueTypes { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseMySql("server=localhost,3306;database=farmmanagerdev;user=developer;password=65026502",
            ServerVersion.AutoDetect("server=localhost,3306;database=farmmanagerdev;user=developer;password=65026502"));
    }
}