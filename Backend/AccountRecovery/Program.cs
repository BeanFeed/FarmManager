using System;
using DAL.Context;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

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
        
        

        var context = new ManagerContext();
        
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
        optionsBuilder.UseSqlite($"Data Source={Path.Join(Directory.GetCurrentDirectory(), "farmmanagerdb.sqlite")}");
    }
}