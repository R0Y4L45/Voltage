using Voltage.Business.Services.Abstract;
using Voltage.Entities.DataBaseContext;

namespace Voltage.Services.HostedService;

public class EmailVerifiedClearHostedService : IHostedService, IDisposable
{
    private readonly IServiceProvider _provider;
    private IUserModifierService? _userModifier;
    private Timer? _timer;

    public EmailVerifiedClearHostedService(IServiceProvider provider)
    {
        _provider = provider;
    }

    private void DoWork(object state)
    {
        _userModifier = _provider.CreateScope().ServiceProvider.GetService<IUserModifierService>();

        foreach (var item in _userModifier!.GetListAsync().Result)
            if(!item.EmailConfirmed)
                _userModifier.Delete(item);
        //Console.WriteLine("User was deleted => " + DateTime.Now.ToString());
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
