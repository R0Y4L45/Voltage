using Voltage.Entities.DataBaseContext;
using Voltage.Entities.Entity;

namespace Voltage.Services.HostedService;

public class EmailVerifiedClearHostedService : IHostedService, IDisposable
{
    private readonly IServiceProvider _provider;
    private Timer? _timer;

    public EmailVerifiedClearHostedService(IServiceProvider provider) =>
        _provider = provider;

    private void DoWork(object state)
    {
        using VoltageDbContext? context = _provider.CreateScope().ServiceProvider.GetService<VoltageDbContext>();
        
        if (context is not null)
        {
            foreach (User user in context.Users)
                if (!user.EmailConfirmed)
                    context.Users.Remove(user);

            context.SaveChanges();
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork!, null, TimeSpan.Zero, TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }
    public void Dispose() => _timer?.Dispose();
}
