using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DAL.Entities;

public class IssueType
{
    [Key]
    public int IssueId { get; set; }
    public string Issue { get; set; } = null!;
    public string Repair { get; set; } = null!;
}