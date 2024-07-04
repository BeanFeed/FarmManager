using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class Session
{
    [Key]
    public Guid SessionId { get; set; }
    public string User { get; set; }
    public bool Valid { get; set; } = true;
}