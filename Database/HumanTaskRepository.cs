using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Models;
using Microsoft.Extensions.Logging;
using TaskManagementApi.DTOs;

namespace TaskManagementApi.Database;

public class HumanTaskRepository : IHumanTaskRepository
{
    private readonly TaskManagementContext _context;
    private readonly ILogger<HumanTaskRepository> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public HumanTaskRepository(TaskManagementContext context, IHttpContextAccessor httpContextAccessor, ILogger<HumanTaskRepository> logger)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }
    
    private string? GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
    
    public async Task<List<HumanTask>> GetAllTasksAsync()
    {
        try
        {
            var userId = GetCurrentUserId();
            return await _context.HumanTasks.Where(t => t.AppUserId == userId).ToListAsync();
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
            var userId = GetCurrentUserId();
            return _context.HumanTasks.Where(t => !t.IsComplete && t.AppUserId == userId).ToListAsync();
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
            var userId = GetCurrentUserId();
            return await _context.HumanTasks.FirstOrDefaultAsync(t => t.Id == id && t.AppUserId == userId);
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
            task.AppUserId = GetCurrentUserId()!;
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
            var userId = GetCurrentUserId();
            var existingTask = await _context.HumanTasks.FirstOrDefaultAsync(t => t.Id == id && t.AppUserId == userId);
            if (existingTask == null)
                return null;

            existingTask.Description = task.Description;
            existingTask.IsComplete = task.IsComplete;
            existingTask.DueDate = task.DueDate;
            existingTask.Title = task.Title;
            existingTask.AppUserId = userId;

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
            var userId = GetCurrentUserId();
            var task = await _context.HumanTasks.FirstOrDefaultAsync(t => t.Id == id && t.AppUserId == userId);
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

    public async Task<PagedResult<HumanTask>> GetTasksAsync(int pageNumber, int pageSize, string? search, bool? isComplete)
    {
        var userId = GetCurrentUserId();
        var query = _context.HumanTasks.Where(t => t.AppUserId == userId);
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t => t.Title.Contains(search) || t.Description.Contains(search));
        }
        if (isComplete.HasValue)
        {
            query = query.Where(t => t.IsComplete == isComplete.Value);
        }
        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.DueDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return new PagedResult<HumanTask>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}