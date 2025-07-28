using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskService.Application.Contracts.Jobs;
using TaskService.Application.Interfaces;
using TaskService.Domain.Enums;

namespace TaskService.API.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly IJobService jobService;

    public TasksController(IJobService jobService)
    {
        this.jobService = jobService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateJob(CreateJobRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var job = await jobService.CreateJobAsync(request, Guid.Parse(userId));
        return CreatedAtAction(nameof(GetJobById), new { id = job.Id }, job);
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetJobs(
        [FromQuery] string? search,
        [FromQuery] JobStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var request = new GetJobsRequest(search, status, page, pageSize);
        var result = await jobService.GetJobsAsync(request);
        return Ok(result);
    }
    
    
    [HttpGet("{id}")]
    public IActionResult GetJobById(Guid id)
    {
        return Ok(); 
    }
}