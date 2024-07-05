using System;
using DAL.Context;
using DAL.Entities;
using FarmManagerBackend.Models.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AccountRecovery;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Parameters: <Create|ChangePassword> <Username> <New Password>\n- The Create function will create a new Owner account.");
            Environment.Exit(-1);
        }
        
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory());
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            builder.AddJsonFile("appsettings.Development.json", false);
        }
        else
        {
            builder.AddJsonFile("appsettings.json", false);
        }

        IConfiguration config = builder.Build();

        var dbSettings = config.GetSection("DatabaseConfiguration").Get<DatabaseConfig>();

        var context = new NewContext(dbSettings);
        
        var username = args[1];
        var passhash = BCrypt.Net.BCrypt.HashPassword(args[2]);

        if (args[0].ToLower() == "changepassword")
        {
            User? user = context.Users.First(x => x.Name == username);
            if (user is null)
            {
                Console.WriteLine($"User \"{username}\" not found");
                Environment.Exit(-2);
            }

            user.PassHash = passhash;
            context.Users.Update(user);
        }else if (args[0].ToLower() == "create")
        {
            User[] eUser = context.Users.Where(x => x.Name == username).ToArray();
            if (eUser.Length > 0)
            {
                Console.WriteLine($"User \"{username}\" already exists");
                Environment.Exit(-3);
            }
            User newUser = new User()
            {
                Name = username,
                PassHash = passhash,
                Role = "Owner"
            };
            context.Users.Add(newUser);
        }
        
        Console.WriteLine("Saving Changes");
        context.SaveChanges();
        
        Console.WriteLine($"Password for \"{args[1]}\" is \"{args[2]}\"");
    }
}

class NewContext : ManagerContext
{
    private readonly DatabaseConfig _databaseConfig;
    public NewContext(DatabaseConfig databaseConfig)
    {
        _databaseConfig = databaseConfig;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql($"server={_databaseConfig.ServerAddress},{_databaseConfig.Port};database={_databaseConfig.Database};user={_databaseConfig.User};password={_databaseConfig.Password}", ServerVersion.AutoDetect($"server={_databaseConfig.ServerAddress},{_databaseConfig.Port};database={_databaseConfig.Database};user={_databaseConfig.User};password={_databaseConfig.Password}"));
    }
}