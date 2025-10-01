using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Models;
using Microsoft.Extensions.Logging;

namespace TaskManagementApi.Database;

public class HumanTaskRepository : IHumanTaskRepository
{
    private readonly TaskManagementContext _context;
    private readonly ILogger<HumanTaskRepository> _logger;
    
    public HumanTaskRepository(TaskManagementContext context, ILogger<HumanTaskRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<List<HumanTask>> GetAllTasksAsync()
    {
        try
        {
            return await _context.HumanTasks.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tasks.");
            return new List<HumanTask>();
        }
    }

    public Task<List<HumanTask>> GetAllIncompleteTasksAsync()
    {
        try
        {
            return _context.HumanTasks.Where(t => !t.IsComplete).ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<HumanTask?> GetTaskByIdAsync(int id)
    {
        try
        {
            return await _context.HumanTasks.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving task with id {id}.");
            return null;
        }
    }

    public async Task<HumanTask?> CreateTaskAsync(HumanTask task)
    {
        try
        {
            await _context.HumanTasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task.");
            return null;
        }
    }

    public async Task<HumanTask?> UpdateTaskAsync(int id, HumanTask task)
    {
        try
        {
            var existingTask = await _context.HumanTasks.FindAsync(id);
            if (existingTask == null)
                return null;

            existingTask.Description = task.Description;
            existingTask.IsComplete = task.IsComplete;
            existingTask.DueDate = task.DueDate;
            existingTask.Title = task.Title;

            await _context.SaveChangesAsync();
            return existingTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating task with id {id}.");
            return null;
        }
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        try
        {
            var task = await _context.HumanTasks.FindAsync(id);
            if (task == null)
                return false;

            _context.HumanTasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting task with id {id}.");
            return false;
        }
    }
}