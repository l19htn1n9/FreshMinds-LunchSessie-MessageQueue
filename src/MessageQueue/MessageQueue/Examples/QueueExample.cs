namespace MessageQueue;

public static class QueueExample
{
    public static async Task RunAsync()
    {
        var consumer1 = new Consumer("consumer-1", "my-queue");
        var consumer2 = new Consumer("consumer-2", "my-queue");

        var producer1 = new Producer("producer-1", "my-queue", false);
        var producer2 = new Producer("producer-2", "my-queue", false);

        await producer1.SendMessageAsync("test-message-1");

        consumer1.StartListening();
        consumer2.StartListening();

        await Task.Delay(100);
        Console.WriteLine("Press enter to send next message....");
        Console.ReadLine();
        await producer2.SendMessageAsync("test-message-2");

        await Task.Delay(100);
        Console.WriteLine("Press enter to stop example...");
        Console.ReadLine();

        await producer1.CloseAsync();
        await producer2.CloseAsync();
        await consumer1.StopListeningAsync();
        await consumer1.CloseAsync();
        await consumer2.StopListeningAsync();
        await consumer2.CloseAsync();
    }
}
