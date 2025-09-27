using TaskManagementApi.Models;

namespace TaskManagementApi.Database;

public interface IHumanTaskRepository
{
    public Task<List<HumanTask>> GetAllTasksAsync();
    public Task<HumanTask?> GetTaskByIdAsync(int id);
    public Task<HumanTask?> CreateTaskAsync(Models.HumanTask task);
    public Task<HumanTask?> UpdateTaskAsync(int id, Models.HumanTask task);
    public Task<bool> DeleteTaskAsync(int id);    
}