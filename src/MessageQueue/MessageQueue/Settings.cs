using Amqp;

namespace MessageQueue;

public static class Settings
{
    public static Address Address => new("localhost", 5672, "artemis", "artemis", "/", "amqp");
}
