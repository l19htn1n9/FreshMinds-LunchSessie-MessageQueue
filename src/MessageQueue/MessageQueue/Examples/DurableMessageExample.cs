namespace MessageQueue;

public static class DurableMessageExample
{
    public static async Task RunAsync()
    {
        var producer = new Producer("producer", "my-queue", true);
        await producer.SendMessageAsync("this-will-persist");
        await producer.CloseAsync();
    }
}
