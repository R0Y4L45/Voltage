using Voltage.Business.Services.Abstract;
using Voltage.Entities.Entity;

namespace Voltage.Services.HostedService;

public class EmailVerifiedClearHostedService : IHostedService, IDisposable
{
    private readonly IServiceProvider _provider;
    private IUserManagerService? _userManagerService;
    private Timer? _timer;

    public EmailVerifiedClearHostedService(IServiceProvider provider)
    {
        _provider = provider;
    }

    private void DoWork(object state)
    {
        _userManagerService = _provider.CreateScope().ServiceProvider.GetService<IUserManagerService>();

        foreach (User item in _userManagerService!.GetAllUsers().Result)
            if(!item.EmailConfirmed)
                _userManagerService.DeleteAsync(item).Wait();
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
