using Microsoft.EntityFrameworkCore;
using Voltage.Entities.DataBaseContext;
using Voltage.Entities.Entity;

namespace Voltage.Services.HostedService;

public class EmailVerifiedClearHostedService : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly IServiceProvider _provider;

    public EmailVerifiedClearHostedService(IServiceProvider provider)
    {
        _provider = provider;
    }

    private void DoWork(object state)
    {
        IServiceScope scope = _provider.CreateScope();
        VoltageDbContext? context = scope.ServiceProvider.GetService<VoltageDbContext>();

        foreach (var item in context!.Users)
        {
            if(!item.EmailConfirmed)
            {
                context.Users.Remove(item);
                context.SaveChanges();
            }
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork!, new VoltageDbContext(), TimeSpan.Zero, TimeSpan.FromMinutes(3));

        return Task.CompletedTask;
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }
    public void Dispose() => _timer?.Dispose();
}
