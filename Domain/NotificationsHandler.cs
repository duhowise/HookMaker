using System.Collections.Concurrent;
using DiskQueue;
using HookMaker.Extensions;
using MediatR;

namespace HookMaker.Domain;

public class NotificationsHandler : INotificationHandler<Notification>
{
    private readonly IPersistentQueue _notificationsQueueStorage;

    public NotificationsHandler(IPersistentQueue notificationsQueueStorage)
    {
        _notificationsQueueStorage = notificationsQueueStorage;
    }

    //queue
    public Task Handle(Notification notification, CancellationToken cancellationToken)
    {
        using var persistentQueueSession = _notificationsQueueStorage.OpenSession();
        persistentQueueSession.Enqueue(notification.ToByteArray());
        persistentQueueSession.Flush();
        return Unit.Task;
    }
}