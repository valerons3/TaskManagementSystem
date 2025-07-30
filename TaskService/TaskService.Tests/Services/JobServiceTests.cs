using AutoMapper;
using FluentAssertions;
using Moq;
using TaskService.Application.Contracts.Jobs;
using TaskService.Application.Contracts.Notifications;
using TaskService.Application.Exceptions;
using TaskService.Application.Interfaces;
using TaskService.Application.Interfaces.Repositories;
using TaskService.Domain.Entities;
using TaskService.Domain.Enums;
using TaskService.Infrastructure.Services;

namespace TaskService.Tests.Services;

public class JobServiceTests
{
    private readonly Mock<IJobRepository> repoMock = new();
    private readonly Mock<IJobHistoryLogger> historyMock = new();
    private readonly Mock<INotificationClient> notificationMock = new();
    private readonly Mock<IMapper> mapperMock = new();
    private readonly JobService jobService;

    public JobServiceTests()
    {
        jobService = new JobService(repoMock.Object, historyMock.Object, notificationMock.Object, mapperMock.Object);
    }

    [Fact]
    public async Task CreateJobAsync_CreatesJobAndReturnsResponse()
    {
        var request = new CreateJobRequest("Test", "Desc" );
        var creatorId = Guid.NewGuid();
        var job = new Job { Title = request.Title, Description = request.Description, CreatorId = creatorId };

        mapperMock.Setup(m => m.Map<JobResponse>(It.IsAny<Job>())).Returns(
            new JobResponse(job.Id, job.Title, job.Description, job.Status, job.CreatorId, job.AssigneeId, job.CreatedAt));

        var result = await jobService.CreateJobAsync(request, creatorId);

        result.Title.Should().Be("Test");
        repoMock.Verify(r => r.AddAsync(It.IsAny<Job>()), Times.Once);
        historyMock.Verify(h => h.LogHistoryAsync(It.IsAny<Guid>(), "Job created", null), Times.Once);
    }
    
    [Fact]
    public async Task GetJobByIdAsync_ReturnsJob_WhenUserAuthorized()
    {
        var userId = Guid.NewGuid();
        var job = new Job { Id = Guid.NewGuid(), CreatorId = userId, Title = "Test" };

        repoMock.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);
        mapperMock.Setup(m => m.Map<JobResponse>(job)).Returns(
            new JobResponse(job.Id, job.Title, job.Description, job.Status, job.CreatorId, job.AssigneeId, job.CreatedAt));

        var result = await jobService.GetJobByIdAsync(job.Id, userId);

        result.Title.Should().Be("Test");
    }
    
    [Fact]
    public async Task GetJobByIdAsync_ThrowsForbidden_WhenUserUnauthorized()
    {
        var job = new Job { Id = Guid.NewGuid(), CreatorId = Guid.NewGuid(), AssigneeId = Guid.NewGuid() };
        var userId = Guid.NewGuid();

        repoMock.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => jobService.GetJobByIdAsync(job.Id, userId));
    }
    
    [Fact]
    public async Task GetJobByIdAsync_ThrowsNotFound_WhenJobMissing()
    {
        repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Job)null!);

        await Assert.ThrowsAsync<JobNotFoundException>(() => jobService.GetJobByIdAsync(Guid.NewGuid(), Guid.NewGuid()));
    }
    
    [Fact]
    public async Task UpdateJobAsync_UpdatesAndNotifies()
    {
        var job = new Job { Id = Guid.NewGuid(), CreatorId = Guid.NewGuid(), AssigneeId = Guid.NewGuid(), Title = "Old" };
        var request = new UpdateJobRequest("New","Desc", JobStatus.InProgress );

        repoMock.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);

        await jobService.UpdateJobAsync(job.Id, job.CreatorId, request);

        job.Title.Should().Be("New");
        job.Description.Should().Be("Desc");
        job.Status.Should().Be(JobStatus.InProgress);

        repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        notificationMock.Verify(n => n.SendNotificationAsync(It.IsAny<NotificationRequest>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteJobAsync_SoftDeletesAndNotifies()
    {
        var job = new Job { Id = Guid.NewGuid(), CreatorId = Guid.NewGuid(), Title = "Title", AssigneeId = Guid.NewGuid() };

        repoMock.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);

        await jobService.DeleteJobAsync(job.Id, job.CreatorId);

        job.IsDeleted.Should().BeTrue();
        repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        notificationMock.Verify(n => n.SendNotificationAsync(It.IsAny<NotificationRequest>()), Times.Once);
    }

    [Fact]
    public async Task AssignJobAsync_AssignsAndNotifies()
    {
        var job = new Job { Id = Guid.NewGuid(), CreatorId = Guid.NewGuid(), Title = "Test" };
        var assigneeId = Guid.NewGuid();

        repoMock.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);

        await jobService.AssignJobAsync(job.Id, assigneeId, job.CreatorId);

        job.AssigneeId.Should().Be(assigneeId);
        repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        notificationMock.Verify(n => n.SendNotificationAsync(It.IsAny<NotificationRequest>()), Times.Once);
    }
    
    [Fact]
    public async Task AssignJobAsync_Throws_WhenUnauthorized()
    {
        var job = new Job { Id = Guid.NewGuid(), CreatorId = Guid.NewGuid() };
        var userId = Guid.NewGuid();

        repoMock.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => jobService.AssignJobAsync(job.Id, Guid.NewGuid(), userId));
    }
}