using AutoMapper;
using HookMaker.Data.Models;

namespace HookMaker.Domain.Mappers;

public class NotificationMapper:Profile
{
    public NotificationMapper()
    {
        CreateMap<Notification, WebHook>().ReverseMap();
    }
}