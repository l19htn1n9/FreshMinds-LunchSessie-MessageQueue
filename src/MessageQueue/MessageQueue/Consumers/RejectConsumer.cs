using Amqp;

namespace MessageQueue;

public class RejectConsumer
{
    private readonly string _name;
    private readonly string _address;
    private readonly Connection _connection;
    private readonly ISession _session;
    private readonly Func<CancellationToken, Task> _backgroundAction;
    private IReceiverLink? receiver;
    private CancellationTokenSource? cancellationTokenSource;
    
    public RejectConsumer(string name, string address)
    {
        _name = name;
        _address = address;
        _connection = new Connection(Settings.Address);
        _session = new Session(_connection);
        _backgroundAction = DoWorkAsync;
    }

    public void StartListening()
    {
        cancellationTokenSource = new CancellationTokenSource();
        receiver = _session.CreateReceiver(_name, _address);
        _backgroundAction.Invoke(cancellationTokenSource.Token);
    }

    public async Task StopListeningAsync()
    {
        if (cancellationTokenSource != null)
        {
            await cancellationTokenSource.CancelAsync();
        }

        if (receiver != null)
        {
            await receiver.CloseAsync();
        }
    }

    public async Task CloseAsync()
    {
        await _session.CloseAsync();
        await _connection.CloseAsync();
    }

    private async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (receiver != null)
            {
                var message = await receiver.ReceiveAsync();

                if (message != null)
                {
                    receiver.Reject(message);
                    Console.WriteLine($"{GetType().Name}:{_name} rejected message: {message.Body} for address {_address}");
                }
            }
        }
    }
}
