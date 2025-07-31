using FluentAssertions;
using Moq;
using NotificationService.Application.Contracts;
using NotificationService.Application.Exceptions;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Interfaces.Repositories;
using NotificationService.Domain.Entities;

namespace NotificationService.Tests.Services;

public class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> repoMock = new();
    private readonly Mock<INotificationHubClient> hubMock = new();

    private readonly NotificationService.Infrastructure.Services.NotificationService service;

    public NotificationServiceTests()
    {
        service = new NotificationService.Infrastructure.Services.NotificationService(
            repoMock.Object,
            hubMock.Object
        );
    }

    [Fact]
    public async Task CreateNotificationAsync_Should_AddNotification_And_CallHub()
    {
        var request = new NotificationRequest
        (
            Guid.NewGuid(),
            "Test Title",
            "Test Message"
        );

        await service.CreateNotificationAsync(request);

        repoMock.Verify(r => r.AddAsync(It.Is<Notification>(n =>
            n.UserId == request.UserId &&
            n.Title == request.Title &&
            n.Message == request.Message
        )), Times.Once);

        repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        hubMock.Verify(h => h.SendNotificationAsync(request), Times.Once);
    }

    [Fact]
    public async Task GetNotificationsByUserIdAsync_Should_ReturnNotificationResponses()
    {
        var userId = Guid.NewGuid();

        repoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(new List<Notification>
        {
            new() { Id = Guid.NewGuid(), Title = "Title", Message = "Msg", IsRead = false, CreatedAt = DateTime.UtcNow }
        });

        var result = await service.GetNotificationsByUserIdAsync(userId);

        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Title");
    }

    [Fact]
    public async Task MarkAsReadAsync_Should_UpdateIsRead()
    {
        var id = Guid.NewGuid();
        var notification = new Notification { Id = id, IsRead = false };

        repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(notification);

        await service.MarkAsReadAsync(id);

        notification.IsRead.Should().BeTrue();
        repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task MarkAsReadAsync_Should_Throw_When_NotFound()
    {
        var id = Guid.NewGuid();
        repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Notification?)null);

        Func<Task> act = async () => await service.MarkAsReadAsync(id);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Notification with id: {id} not found");
    }
}