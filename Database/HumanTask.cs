using TaskManagementApi.Database.Security;

namespace TaskManagementApi.Models;

public class HumanTask
{
    public int Id { get; set; }
    public string Title { get; set; } = "Untitled Task";
    public string Description { get; set; } = "";
    public DateTime DueDate { get; set; }
    public bool IsComplete { get; set; }
    
    public string AppUserId { get; set; }      // Foreign key
    public AppUser AppUser { get; set; }       // Navigation property
}