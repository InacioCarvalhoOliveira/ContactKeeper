using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ContactKeeper.Services.Queues
{
    public class UserQueues
    {
        public async Task AuthUserSenderAsync(string mensagem, CancellationToken cancellationToken = default)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "mqsa",
                Password = "GosthTro0per.ZUZ236",
                Port = 8081,
                VirtualHost = "/"
            };

            await using var connection = await factory.CreateConnectionAsync(cancellationToken);
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "teste",
                durable: true,          // true if you want persistence
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );

            var body = Encoding.UTF8.GetBytes(mensagem);

            var props = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "teste",          // nome da fila
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: cancellationToken
            );

        }
    }
}
