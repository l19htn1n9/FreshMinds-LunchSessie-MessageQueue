namespace MessageQueue;

public class DeadLetterQueueExample
{
    public static async Task RunAsync()
    {
        var consumer = new RejectConsumer("consumer-1", "my-queue");

        var producer = new Producer("producer-1", "my-queue");

        await producer.SendMessageAsync("test-message-1");

        consumer.StartListening();

        await Task.Delay(100);
        Console.WriteLine("Press enter to stop....");
        Console.ReadLine();

        await producer.CloseAsync();
        await consumer.StopListeningAsync();
        await consumer.CloseAsync();
    }
}
