using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Database;
using TaskManagementApi.Models;

namespace TaskManagementApi.Controllers;

[ApiController]
[Route("v{apiVersion:apiVersion}/[controller]")]
[ApiVersion("1")]
public class TaskManagementController : Controller
{
    private readonly IHumanTaskRepository _taskRepository;
    private readonly ILogger<TaskManagementController> _logger;
    
    public TaskManagementController(IHumanTaskRepository taskManagementService, ILogger<TaskManagementController> logger)
    {
        _taskRepository = taskManagementService;
        _logger = logger;
    }

    // GET: v1/TaskManagement
    [HttpGet]
    public async Task<IActionResult> GetAllTasks(CancellationToken cancellationToken)
    {
        var results = await _taskRepository.GetAllTasksAsync();
        return Ok(results);
    }

    // GET: v1/TaskManagement/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTaskById(int id, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Fetching task {TaskId}", id);
        var task = await _taskRepository.GetTaskByIdAsync(id);
        if (task == null)
        {
            _logger.LogInformation("Task {TaskId} not found", id);
            return NotFound();
        }
        return Ok(task);
    }

    // POST: v1/TaskManagement
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
    {
        // Model validation is automatically handled by [ApiController]; invalid models return 400 before reaching here.
        var entity = new HumanTask
        {
            Title = request.Title,
            Description = request.Description ?? string.Empty,
            DueDate = request.DueDate ?? DateTime.UtcNow.AddDays(7),
            IsComplete = false
        };

        var created = await _taskRepository.CreateTaskAsync(entity);
        if (created == null)
        {
            _logger.LogError("Failed to create task {Title}", request.Title);
            return Problem("An error occurred while creating the task.", statusCode: 500);
        }

        _logger.LogInformation("Created task {TaskId}", created.Id);
        return CreatedAtAction(nameof(GetTaskById), new { id = created.Id, apiVersion = "1" }, created);
    }

    // PUT: v1/TaskManagement/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        // Automatic 400 for invalid model occurs prior to executing this method.
        var updateEntity = new HumanTask
        {
            Title = request.Title,
            Description = request.Description ?? string.Empty,
            DueDate = request.DueDate ?? DateTime.UtcNow.AddDays(7),
            IsComplete = request.IsComplete
        };

        var updated = await _taskRepository.UpdateTaskAsync(id, updateEntity);
        if (updated == null)
        {
            var existing = await _taskRepository.GetTaskByIdAsync(id);
            if (existing == null)
            {
                _logger.LogInformation("Task {TaskId} not found for update", id);
                return NotFound();
            }
            _logger.LogError("Failed to update task {TaskId}", id);
            return Problem("An error occurred while updating the task.", statusCode: 500);
        }

        return Ok(updated);
    }

    // PATCH: v1/TaskManagement/{id}/complete
    [HttpPatch("{id:int}/complete")]
    public async Task<IActionResult> MarkComplete(int id, CancellationToken cancellationToken)
    {
        var existing = await _taskRepository.GetTaskByIdAsync(id);
        if (existing == null)
        {
            return NotFound();
        }
        existing.IsComplete = true;
        var updated = await _taskRepository.UpdateTaskAsync(id, existing);
        if (updated == null)
        {
            return Problem("Failed to mark task complete.", statusCode: 500);
        }
        return Ok(updated);
    }

    // DELETE: v1/TaskManagement/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(int id, CancellationToken cancellationToken)
    {
        var existing = await _taskRepository.GetTaskByIdAsync(id);
        if (existing == null)
        {
            return NotFound();
        }
        var success = await _taskRepository.DeleteTaskAsync(id);
        if (!success)
        {
            _logger.LogError("Failed to delete task {TaskId}", id);
            return Problem("An error occurred while deleting the task.", statusCode: 500);
        }
        _logger.LogInformation("Deleted task {TaskId}", id);
        return NoContent();
    }
}
