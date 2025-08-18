using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using ThirdTask.Products.Application.Interfaces;

namespace ThirdTask.Products.Application.RabbitMQ
{
    public class RabbitMqEventPublisher : IEventPublisher
    {
        private readonly string _exchangeName = "product-created";
        public async Task PublishAsync<T>(T @event)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            // Fanout Exchange oluştur (tüm servisler dinleyebilsin)
            await channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Fanout, durable: true);

            var json = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(json);

            // Mesajı exchange'e gönder
            await channel.BasicPublishAsync(exchange: _exchangeName, routingKey: "", body: body);
        }
    }
}
