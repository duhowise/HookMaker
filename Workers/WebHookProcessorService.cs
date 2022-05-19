using AutoMapper;
using DiskQueue;
using HookMaker.Data.Context;
using HookMaker.Data.Models;
using HookMaker.Domain;
using HookMaker.Extensions;

namespace HookMaker.Workers;

public class WebHookProcessorService : BackgroundService
{
    private readonly IPersistentQueue _persistentQueueStorage;
    private readonly IServiceProvider _provider;
    private readonly IMapper _mapper;

    public WebHookProcessorService(IPersistentQueue persistentQueueStorage, IServiceProvider provider, IMapper mapper)
    {
        _persistentQueueStorage = persistentQueueStorage;
        _provider = provider;
        _mapper = mapper;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_persistentQueueStorage.EstimatedCountOfItemsInQueue > 0)
            {
                await using var scope = _provider.CreateAsyncScope();
                using var session = _persistentQueueStorage.OpenSession();
                var notificationBytes = session.Dequeue();
                if (notificationBytes != null)
                {
                    var notification = notificationBytes.FromByteArrayTo<Notification>();
                    var webHook = _mapper.Map<WebHook>(notification);
                    var context = scope.ServiceProvider.GetService<WebHooksDbContext>();
                    if (context == null) throw new ArgumentException("dbContext may have not been registered");
                    await context.WebHooks.AddAsync(webHook, stoppingToken);
                    await context.SaveChangesAsync(stoppingToken);
                    session.Flush();
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }
    }
}