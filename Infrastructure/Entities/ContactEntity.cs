
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class ContactEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string FullName { get; set; } = null!;
    [Required]
    public string Email { get; set; } = null!;
    public string? Service { get; set; }

    [Required]
    public string Message { get; set; } = null!;
    

    public DateTime CreatedAt { get; set; } = DateTime.Now;

}
