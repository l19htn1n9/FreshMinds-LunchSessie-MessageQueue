namespace MessageQueue;

public static class TtlQueueExample
{
    public static async Task RunAsync()
    {
        var producer = new TtlProducer("ttl-producer", "my-queue");
        await producer.SendMessageAsync("this-message-will-expire");
        await producer.CloseAsync();
        await Task.Delay(5000);
        Console.WriteLine("Please check the expiry queue");
    }
}
