using System.ComponentModel.DataAnnotations;

namespace ToDoAPI.Models.Dtos;

public class PercentCompleteDto
{
    [Range(0, 100, ErrorMessage = "Percent complete must be between 0 and 100.")]
    public int PercentComplete { get; set; }
}
