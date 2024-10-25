using System.ComponentModel.DataAnnotations;

namespace ToDoAPI.Models.Dtos;

public class CreateToDoDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title length can't exceed 100 characters.")]
    public required string Title { get; set; }

    public string? Description { get; set; }

    [Range(0, 100, ErrorMessage = "Percent complete must be between 0 and 100.")]
    public int PercentComplete { get; set; }

    [Required]
    public DateTime ExpiryDate { get; set; }
}
