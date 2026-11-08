using Confluent.Kafka;
using PractitionerDirectory.Services;
using PractitionerDirectory.Services.Models;
using System.Text.Json;

namespace PractitionerDirectory.Integrations
{
    public class Worker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(
            IServiceScopeFactory scopeFactory,
            ILogger<Worker> logger,
            IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = _configuration["Kafka:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            var topic = _configuration["Kafka:Topic"];
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(topic);
            _logger.LogInformation($"Consumer запущен, слушаем топик{topic}");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumed = consumer.Consume(stoppingToken);
                    _logger.LogInformation($"Получено сообщение{consumed.Message.Value}");

                    var modelDTO = JsonSerializer.Deserialize<PractitionerDTO>(
                        consumed.Message.Value,
                        jsonOptions);

                    if (modelDTO != null)
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService<IPractitionerService>();

                        await service.UpsertAsync(modelDTO);
                    }

                    consumer.Commit(consumed);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка обработки сообщения");
                    await Task.Delay(5000, stoppingToken);
                }
            }

            consumer.Close();
        }
    }
}
