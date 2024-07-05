using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class Session
{
    [Key]
    public Guid SessionId { get; set; }
    public int UserId { get; set; }
    public bool Valid { get; set; } = true;
}