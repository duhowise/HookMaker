using System.Collections.Concurrent;
using HookMaker.Data.Context;
using MediatR;

namespace HookMaker.Domain;

public class NotificationsHandler:INotificationHandler<Notification>
{
    private readonly WebHooksDbContext _context;
    private readonly ConcurrentQueue<Notification> _notifications;

    public NotificationsHandler(WebHooksDbContext context)
    {
        _context = context;
        _notifications = new ConcurrentQueue<Notification>();
    }
   
    //queue
    public Task Handle(Notification notification, CancellationToken cancellationToken)
    {

        _notifications.Enqueue(notification);
        
        
        //Task.Run(() =>
        //{
        //    //send notification to queue
        //});
        return Unit.Task;
    }
}