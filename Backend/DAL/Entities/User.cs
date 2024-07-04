using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string PassHash { get; set; } = null!;
}