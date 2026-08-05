using System.Text;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataServices
{
    public class MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor) : BackgroundService, IAsyncDisposable
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IEventProcessor _eventProcessor = eventProcessor;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);

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
                await _channel.QueueDeclareAsync(queue: "commandqueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                await _channel.QueueBindAsync(queue: "commandqueue", exchange: "trigger", routingKey: "");

                Console.WriteLine("--> Listening on the Message Bus...");

                _connection.ConnectionShutdownAsync += async (sender, args) =>
                {
                    Console.WriteLine("--> Connection Shutdown");
                    await Task.CompletedTask;
                };


                return _channel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MessageBusSubscriber] Could not initialize RabbitMQ: {ex.Message}");
                throw;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var channel = await GetChannelAsync();

                    var consumer = new AsyncEventingBasicConsumer(channel);

                    consumer.ReceivedAsync += async (ModuleHandle, ea) =>
                    {
                        Console.WriteLine("--> Event Received!");

                        var body = ea.Body;
                        var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                        _eventProcessor.ProcessEvent(notificationMessage);

                        await Task.CompletedTask;
                    };

                    await channel.BasicConsumeAsync(queue: "commandqueue", autoAck: true, consumer: consumer, cancellationToken: stoppingToken);

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        await Task.Delay(1000, stoppingToken);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MessageBusSubscriber] RabbitMQ unavailable, retrying in 5 seconds: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
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
    }
}