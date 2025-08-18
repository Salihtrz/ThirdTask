using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using ThirdTask.Events.Dtos;

namespace ThirdTask.Auth.Application.RabbitMQ
{
    public class ProductCreatedEventConsumer : BackgroundService
    {
        private readonly string _exchangeName = "product-created";
        private readonly string _queueName = "auth-service-queue";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Fanout, durable: true);
            await channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(queue: _queueName, exchange: _exchangeName, routingKey: "");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var productEvent = JsonSerializer.Deserialize<ProductCreatedEventDto>(json);
                await Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
