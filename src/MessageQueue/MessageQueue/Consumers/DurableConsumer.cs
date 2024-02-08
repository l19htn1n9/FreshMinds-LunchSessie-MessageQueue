using Amqp;
using Amqp.Framing;
using Amqp.Types;

namespace MessageQueue;

public class DurableConsumer
{
    private readonly string _name;
    private readonly string _address;
    private readonly Source _source;
    private readonly Func<CancellationToken, Task> _backgroundAction;
    private Connection? connection;
    private Session? session;
    private ReceiverLink? receiver;
    private CancellationTokenSource? cancellationTokenSource;

    public DurableConsumer(string name, string address)
    {
        _name = name;
        _address = address;
        _source = new Source()
        {
            Address = _address,
            Durable = 2,
            ExpiryPolicy = new Symbol("never")
        };
        _backgroundAction = DoWorkAsync;
    }

    public void StartListening()
    {
        cancellationTokenSource = new CancellationTokenSource();
        connection = new Connection(Settings.Address, null, new Open()
        {
            ContainerId = $"{_name}-con"
        }, null);
        session = new Session(connection);
        receiver = new ReceiverLink(session, _name, _source, null);
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
            receiver = null;
        }
    }

    public async Task CloseAsync()
    {
        if (session != null)
        {
            await session.CloseAsync();
            session = null;
        }
        if (connection != null)
        {
            await connection.CloseAsync();
            connection = null;
        }
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
                    receiver.Accept(message);
                    Console.WriteLine($"{GetType().Name}:{_name} received message: {message.Body} for address {_address}");
                }
            }
        }
    }
}
