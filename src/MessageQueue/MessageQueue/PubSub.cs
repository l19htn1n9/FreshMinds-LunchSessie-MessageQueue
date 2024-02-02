using Amqp;

namespace MessageQueue
{
    internal class PubSub
    {
        public static async Task Run()
        {
            var connection = new Connection(new Address("localhost", 5672, "artemis", "artemis", "/", "amqp"));
            var session = new Session(connection);
            var senderLink = new SenderLink(session, "my-sender", "my-queue::pub-sub");
            var message = new Message("my-test-message")
            {
                Header = new()
                {
                    Durable = false
                }
            };

            await senderLink.SendAsync(message);

            await senderLink.CloseAsync();
            await session.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
