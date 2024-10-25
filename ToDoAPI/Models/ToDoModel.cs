using System.ComponentModel.DataAnnotations;

namespace ToDoAPI.Models;

public class ToDoModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public required string Title { get; set; }

    public string? Description { get; set; }

    [Range(0, 100)]
    public int PercentComplete { get; set; }

    [Required]
    public DateTime ExpiryDate { get; set; }

    public bool IsCompleted { get; set; }
}
