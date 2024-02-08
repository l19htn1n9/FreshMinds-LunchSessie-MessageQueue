using Amqp;

namespace MessageQueue;

public class Producer
{
    private readonly string _name;
    private readonly string _address;
    private readonly Connection _connection;
    private readonly ISession _session;
    private readonly ISenderLink _sender;
    private readonly bool _durable;

    public Producer(string name, string address, bool durable)
    {
        _name = name;
        _address = address;
        _durable = durable;
        _connection = new Connection(Settings.Address);
        _session = new Session(_connection);
        _sender = _session.CreateSender(_name, _address);
    }

    public async Task SendMessageAsync(string messageBody)
    {
        var message = new Message(messageBody)
        {
            Header = new()
            {
                Durable = _durable,
            },
        };

        await _sender.SendAsync(message);
        Console.WriteLine($"{GetType().Name}:{_name} sent message: {messageBody} to {_address}");
    }

    public async Task CloseAsync()
    {
        await _sender.CloseAsync();
        await _session.CloseAsync();
        await _connection.CloseAsync();
    }
}
