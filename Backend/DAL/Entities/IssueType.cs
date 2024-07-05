using Microsoft.EntityFrameworkCore;

namespace DAL.Entities;

[Keyless]
public class IssueType
{
    public string Issue { get; set; } = null!;
    public string Repair { get; set; } = null!;
}