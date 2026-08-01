using System.Text;
using System.Text.Json;
using PlatformService.Models;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class RabbitMqMessageBusClient(IConfiguration configuration) : IMessageBusClient, IAsyncDisposable
    {
        private readonly IConfiguration _configuration = configuration;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);

        public async Task PublishNewPlatformAsync(PlatformPublishedDto platformPublishedDto)
        {
            ArgumentNullException.ThrowIfNull(platformPublishedDto);

            string message = JsonSerializer.Serialize(platformPublishedDto);
            Console.WriteLine($"[MessageBusClient] Preparing to publish message: {message}");

            await SendMessageAsync(message);
        }

        public async ValueTask DisposeAsync()
        {
            Console.WriteLine("[MessageBusClient] Disposing resources...");

            if (_channel != null)
            {
                await _channel.CloseAsync();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
            }

            _connectionLock.Dispose();
            GC.SuppressFinalize(this);
        }

        private async Task<IChannel> GetChannelAsync()
        {
            if (_channel != null && _connection != null && _connection.IsOpen)
            {
                return _channel;
            }

            await _connectionLock.WaitAsync();
            try
            {
                if (_channel != null && _connection != null && _connection.IsOpen)
                {
                    return _channel;
                }

                if (!int.TryParse(_configuration["RabbitMQPort"], out int port))
                {
                    port = 5672;
                }

                var factory = new ConnectionFactory()
                {
                    HostName = _configuration["RabbitMQHost"] ?? "localhost",
                    Port = port
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdownAsync += async (sender, args) =>
                {
                    Console.WriteLine($"[MessageBusClient] RabbitMQ connection shutdown: {args.ReplyText}");
                    await Task.CompletedTask;
                };

                Console.WriteLine("[MessageBusClient] Connected to RabbitMQ");

                return _channel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MessageBusClient] Could not connect to RabbitMQ: {ex.Message}");
                throw;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        private async Task SendMessageAsync(string message)
        {
            try
            {
                var channel = await GetChannelAsync();
                var body = Encoding.UTF8.GetBytes(message);
                var props = new BasicProperties();

                await channel.BasicPublishAsync(
                    exchange: "trigger",
                    routingKey: "",
                    mandatory: false,
                    basicProperties: props,
                    body: body
                );

                Console.WriteLine($"[MessageBusClient] Sent message successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MessageBusClient] Failed to send message: {ex.Message}");
            }
        }
    }
}
