using Microsoft.EntityFrameworkCore;

namespace DAL.Entities;

public class IssueType
{
    public int Id { get; set; }
    public string Issue { get; set; } = null!;
    public string Repair { get; set; } = null!;
}