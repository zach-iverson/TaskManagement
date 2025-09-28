using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.Models;

public class UpdateTaskRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsComplete { get; set; }
}

