using System.Collections.Concurrent;
using DiskQueue;
using HookMaker.Data.Context;
using HookMaker.Extensions;
using MediatR;

namespace HookMaker.Domain;

public class NotificationsHandler:INotificationHandler<Notification>
{
    private readonly WebHooksDbContext _context;
    readonly IPersistentQueue _notificationsQueueStorage = new PersistentQueue(Directory.GetCurrentDirectory());
    public NotificationsHandler(WebHooksDbContext context)
    {
        _context = context;
    }
   
    //queue
    public Task Handle(Notification notification, CancellationToken cancellationToken)
    {
        using var persistentQueueSession=_notificationsQueueStorage.OpenSession();
        persistentQueueSession.Enqueue(notification.ToByteArray());
        persistentQueueSession.Flush();

        //Task.Run(() =>
        //{
        //    //send notification to queue
        //});
        return Unit.Task;
    }



    private void ProcessNotifications()
    {
        
    }
}