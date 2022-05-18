using MediatR;

namespace HookMaker.Domain;

public class Notification:INotification
{
    public DateTime Date { get; set; }
    public string Json { get; set; }

}