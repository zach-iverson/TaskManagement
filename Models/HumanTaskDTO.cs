namespace TaskManagementApi.Models;

public class HumanTaskDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsComplete { get; set; }
    
    public static HumanTaskDto FromEntity(HumanTask task)
    {
        return new HumanTaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            IsComplete = task.IsComplete
        };
    }
}