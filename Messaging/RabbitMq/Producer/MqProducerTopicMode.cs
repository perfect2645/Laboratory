using Messaging.RabbitMq.Connections;
using Messaging.RabbitMq.Models;
using Messaging.RabbitMq.Producer;
using RabbitMQ.Client;
using System.Text;
using Utils.Json;
using Utils.Tasking;

namespace service.messaging.Clients.Producer
{
    public abstract class MqProducerTopicMode<TPayload> : IRabbitMqTopicProducer<TPayload> where TPayload : ITopicPayload
    {
        private readonly IRabbitMqConnectionFactory _connectionFactory;
        private IChannel? _channel;
        private readonly Task _connectionBuildingTask;

        #region Init

        public MqProducerTopicMode(IRabbitMqConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;

            _connectionBuildingTask = BuildConnectionAsync();
            _connectionBuildingTask.SafeFireAndForget(OnCompleted, OnError);
        }

        private void OnError(Exception exception)
        {
            throw exception;
        }

        private void OnCompleted()
        {
            // No action needed
        }   

        private async Task BuildConnectionAsync()
        {
            var connection = await _connectionFactory.GetConnectionAsync();
            _channel = await connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: _connectionFactory.RabbitMqSettings.ExchangeName, type: ExchangeType.Topic, durable:true);
        }

        #endregion Init

        public virtual async Task ProduceAsync(TPayload messagePayload, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(messagePayload);

            await _connectionBuildingTask;

            var properties = new BasicProperties
            {
                Persistent = true
            };

            var jsonMsg = JsonSerializerUtil.SerializeCamelCase(messagePayload);
            byte[] msgBody = Encoding.UTF8.GetBytes(jsonMsg);
            await _channel!.BasicPublishAsync(exchange: _connectionFactory.RabbitMqSettings.ExchangeName,
                routingKey: messagePayload.Topic,
                mandatory: true,
                basicProperties: properties,
                body: msgBody,
                cancellationToken: ct
            );
        }
    }
}
