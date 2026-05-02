namespace Order.Infrastructure.Settings;

public class RabbitMqSettings
{
    public string Host { get; init; } = "localhost";
    public string Username { get; init; } = "guest";
    public string Password { get; init; } = "guest";
}
