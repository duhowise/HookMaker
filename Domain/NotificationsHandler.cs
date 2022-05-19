using DiskQueue;
using HookMaker.Extensions;
using MediatR;

namespace HookMaker.Domain;

public class NotificationsHandler : INotificationHandler<Notification>
{
    private readonly IPersistentQueue _notificationsQueueStorage;
    private readonly ILogger<NotificationsHandler> _logger;

    public NotificationsHandler(IPersistentQueue notificationsQueueStorage,ILogger<NotificationsHandler> logger)
    {
        _notificationsQueueStorage = notificationsQueueStorage;
        _logger = logger;
    }

    //queue
    public Task Handle(Notification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("notification for {data} received",notification);
        //session is created from notification storage to get access to queue


        using var persistentQueueSession = _notificationsQueueStorage.OpenSession();
        _logger.LogInformation("queue session opened in {service}", nameof(NotificationsHandler));


        persistentQueueSession.Enqueue(notification.ToByteArray());
        //notification received is en-queued and persisted with the flush method


        persistentQueueSession.Flush();
        _logger.LogInformation("queue session closed in {service}", nameof(NotificationsHandler));
        return Unit.Task;
    }
}