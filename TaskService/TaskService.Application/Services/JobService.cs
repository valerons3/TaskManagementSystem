using AutoMapper;
using TaskService.Application.Contracts.Jobs;
using TaskService.Application.Contracts.Notifications;
using TaskService.Application.Exceptions;
using TaskService.Application.Interfaces;
using TaskService.Application.Interfaces.Repositories;
using TaskService.Domain.Entities;
using TaskService.Domain.Enums;

namespace TaskService.Infrastructure.Services;

public class JobService : IJobService
{
    private readonly IJobRepository jobRepository;
    private readonly IJobHistoryLogger jobHistoryLogger;
    private readonly INotificationClient notificationClient;
    private readonly IMapper mapper;

    public JobService(IJobRepository jobRepository, IJobHistoryLogger jobHistoryLogger,
        INotificationClient notificationClient, IMapper mapper)
    {
        this.jobRepository = jobRepository;
        this.jobHistoryLogger = jobHistoryLogger;
        this.notificationClient = notificationClient;
        this.mapper = mapper;
    }
    
    public async Task<JobResponse> CreateJobAsync(CreateJobRequest request, Guid creatorId)
    {
        var job = new Job
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            Title = request.Title,
            Description = request.Description,
            Status = JobStatus.Todo,
            CreatedAt = DateTime.UtcNow,
        };
        
        await jobRepository.AddAsync(job);
        await jobRepository.SaveChangesAsync();
        await jobHistoryLogger.LogHistoryAsync(job.Id, "Job created", null);

        return mapper.Map<JobResponse>(job);
    }

    public async Task<PagedResult<JobResponse>> GetJobsAsync(GetJobsRequest request, Guid userId)
    {
        int skip = (request.Page - 1) * request.PageSize;

        var jobs = await jobRepository.GetUserJobsAsync(
            userId,
            request.Search,
            request.Status,
            skip,
            request.PageSize);

        int totalCount = await jobRepository.GetUserJobsCountAsync(userId, request.Search, request.Status);

        var items = mapper.Map<List<JobResponse>>(jobs);

        return new PagedResult<JobResponse>(totalCount, items);
    }

    public async Task<JobResponse> GetJobByIdAsync(Guid jobId, Guid userId)
    {
        Job? job = await jobRepository.GetByIdAsync(jobId);

        if (job is null)
            throw new JobNotFoundException(jobId);
        if (job.CreatorId != userId && job.AssigneeId != userId)
            throw new ForbiddenAccessException();
        

        return mapper.Map<JobResponse>(job);
    }

    public async Task UpdateJobAsync(Guid jobId, Guid userId, UpdateJobRequest request)
    {
        Job? job = await jobRepository.GetByIdAsync(jobId);
        if (job is null)
            throw new JobNotFoundException(jobId);
        if (job.CreatorId != userId && job.AssigneeId != userId)
            throw new ForbiddenAccessException();
        
        if (request.Title is not null)
            job.Title = request.Title;
        if (request.Description is not null)
            job.Description = request.Description;
        if (request.Status is not null)
            job.Status = request.Status.Value;

        await jobRepository.SaveChangesAsync();
        await jobHistoryLogger.LogHistoryAsync(job.Id, "Job updated", null);
        if (job.AssigneeId is Guid assigneeId)
        {
            await notificationClient.SendNotificationAsync(new NotificationRequest(
                assigneeId,
                "Изменение задачи",
                $"Назначенная вам задача \"{job.Title}\" была изменена"));
        }
    }

    public async Task DeleteJobAsync(Guid jobId, Guid userId)
    {
        Job? job = await jobRepository.GetByIdAsync(jobId);
        
        if (job is null) 
            throw new JobNotFoundException(jobId);
        
        if (job.CreatorId != userId)
            throw new ForbiddenAccessException();
        
        job.IsDeleted = true;
        
        await jobRepository.SaveChangesAsync();
        await jobHistoryLogger.LogHistoryAsync(job.Id, "Job deleted", null);
        
        if (job.AssigneeId is Guid assigneeId)
        {
            await notificationClient.SendNotificationAsync(new NotificationRequest(
                assigneeId,
                "Удалена задача",
                $"Назначенная вам задача \"{job.Title}\" была удалена"));
        }
    }

    public async Task AssignJobAsync(Guid jobId, Guid assigneeId, Guid userId)
    {
        Job? job = await jobRepository.GetByIdAsync(jobId);
        if (job is null)
            throw new JobNotFoundException(jobId);
        
        if (job.CreatorId != userId)
            throw new ForbiddenAccessException();

        job.AssigneeId = assigneeId;
        
        await jobRepository.SaveChangesAsync();
        await jobHistoryLogger.LogHistoryAsync(job.Id, $"Assigned to user {assigneeId}", null);
        await notificationClient.SendNotificationAsync(new NotificationRequest(assigneeId,
            "Новая задача",
            $"Вам назначена новая задача: \"{job.Title}\""));
    }
    
}

