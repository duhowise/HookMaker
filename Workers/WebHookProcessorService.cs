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
    private readonly ILogger<WebHookProcessorService> _logger;

    public WebHookProcessorService(IPersistentQueue persistentQueueStorage, IServiceProvider provider, IMapper mapper,ILogger<WebHookProcessorService>logger)
    {
        _persistentQueueStorage = persistentQueueStorage;
        _provider = provider;
        _mapper = mapper;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_persistentQueueStorage.EstimatedCountOfItemsInQueue > 0)
            {
                await using var scope = _provider.CreateAsyncScope();
                //creating a self contained scope for background service in order to resolved items registered per scope
                //this allows you to use dbcontext without registering a factory
                //since background service runs in a singleton scope.
                using var session = _persistentQueueStorage.OpenSession();
                _logger.LogInformation("queue session opened in {service}",nameof(WebHookProcessorService));
                var notificationBytes = session.Dequeue();
                //queue automatically preserves order with first in first out implementation
                if (notificationBytes != null)
                {
                   //notification converted back to object from bytearray via extension method
                    var notification = notificationBytes.FromByteArrayTo<Notification>();
                    var webHook = _mapper.Map<WebHook>(notification);
                    var context = scope.ServiceProvider.GetService<WebHooksDbContext>();
                    if (context == null) throw new ArgumentException("dbContext may have not been registered");
                    await context.WebHooks.AddAsync(webHook, stoppingToken);
                    await context.SaveChangesAsync(stoppingToken);
                    session.Flush();
                    _logger.LogInformation("queue session closed in {service}", nameof(WebHookProcessorService));
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
            //worker runs every three seconds to check if there are new items on queue before attempting any form of processing
        }
    }
}