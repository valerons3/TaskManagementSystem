using TaskService.Domain.Entities;

namespace TaskService.Application.Interfaces.Repositories;

public interface IJobHistoryRepository
{
    Task AddAsync(JobHistory history);
}