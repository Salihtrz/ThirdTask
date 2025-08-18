using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace ThirdTask.Products.Application.RabbitMQ
{
    public class ProductCreatedEventConsumer
    {
        private readonly string _exchangeName = "product-created";
        private readonly string _queueName;
        public ProductCreatedEventConsumer(string serviceQueueName)
        {
            _queueName = serviceQueueName;
        }
        public async Task StartAsync()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Fanout, durable: true);

            await channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            await channel.QueueBindAsync(queue: _queueName, exchange: _exchangeName, routingKey: "");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
            };

            await channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer);

            await Task.Delay(Timeout.Infinite);
        }
    }
}
