namespace MessageQueue;

public static class NonDurableMessageExample
{
    public static async Task RunAsync()
    {
        var producer = new Producer("producer", "my-queue", false);
        await producer.SendMessageAsync("this-will-not-persist");
        await producer.CloseAsync();
    }
}
