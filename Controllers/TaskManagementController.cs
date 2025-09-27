using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Database;


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

    [HttpGet]
    public async Task<IActionResult> GetAllTasks()
    {
        var results = await _taskRepository.GetAllTasksAsync();
        return Ok(results);
    }
    
}
