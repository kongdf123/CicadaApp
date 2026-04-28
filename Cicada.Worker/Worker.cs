using Cicada.Biz.Services;

namespace Cicada.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly TelemetryProducer _producer;
        private readonly TelemetryConsumer _consumer;

        public Worker(ILogger<Worker> logger, TelemetryProducer producer, TelemetryConsumer consumer)
        {
            _logger = logger;
            _producer = producer;
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                _ = Task.Run(() => _producer.StartAsync(stoppingToken));
                _ = Task.Run(() => _consumer.StartAsync(stoppingToken));

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
