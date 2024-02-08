using Amqp;

namespace MessageQueue;

public class Consumer
{
    private readonly string _name;
    private readonly string _address;
    private readonly Connection _connection;
    private readonly ISession _session;
    private readonly IReceiverLink _receiver;
    private CancellationTokenSource? cancellationTokenSource;
    private readonly Func<CancellationToken, Task> _backgroundAction;

    public Consumer(string name, string address)
    {
        _name = name;
        _address = address;
        _connection = new Connection(Settings.Address);
        _session = new Session(_connection);
        _receiver = _session.CreateReceiver(_name, _address);
        _backgroundAction = DoWorkAsync;
    }

    public void StartListening()
    {
        cancellationTokenSource = new CancellationTokenSource();
        _backgroundAction.Invoke(cancellationTokenSource.Token);
    }

    public async Task StopListeningAsync()
    {
        if (cancellationTokenSource != null)
        {
            await cancellationTokenSource.CancelAsync();
        }
    }

    public async Task CloseAsync()
    {
        await _receiver.CloseAsync();
        await _session.CloseAsync();
        await _connection.CloseAsync();
    }

    private async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var message = await _receiver.ReceiveAsync();

            if (message != null)
            {
                Console.WriteLine($"{GetType().Name}:{_name} received message: {message.Body} for address {_address}");

                _receiver.Accept(message);
            }
        }
    }
}
