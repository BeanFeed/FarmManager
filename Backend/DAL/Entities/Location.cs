using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class Location
{
    [Key] 
    public string Name { get; set; } = null!;
}