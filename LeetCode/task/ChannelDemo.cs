using System.Threading.Channels;
using Utils.Events;

namespace LeetCode.task
{
    public class ChannelDemo
    {
        public async Task RunAsync()
        {
            var msgChannel = Channel.CreateUnbounded<Message>();

            var messages = Enumerable.Range(1, 30).AsParallel().AsOrdered()
                .Select(i => new Message(i, $"Message {i}")).ToList();

            using var cts = new CancellationTokenSource();
            var sender =  SendMessagesAsync(msgChannel.Writer, messages);
            var receiver =  ReceiveMessagesAsync(msgChannel.Reader, cts.Token);

            await sender;

            await Task.Delay(100); // make sure all messages are received
            cts.Cancel(); // messages are all handled, stop the receiver
            await receiver;

            LogEvents.Publish("ChannelDemo completed.");
        }

        private async Task SendMessagesAsync(ChannelWriter<Message> channelWriter, List<Message> messages) 
        {
            foreach (var message in messages)
            {
                await SendMessageAsync(channelWriter, message);
            }
        }

        private async Task SendMessageAsync(ChannelWriter<Message> channelWriter, Message message)
        {
            await channelWriter.WriteAsync(message);
            LogEvents.Publish($"Sender: {message.Id} Sent.");
            await Task.Delay(200); // Simulate some processing delay
        }

        private async Task ReceiveMessagesAsync(ChannelReader<Message> reader, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var message = await reader.ReadAsync(cancellationToken);
                    LogEvents.Publish($"Receiver: {message.Id} Received.");
                }
            }
            catch (OperationCanceledException ex)
            {
                LogEvents.Publish($"Receiver was cancelled: {ex.Message}");
            }
            catch (Exception ex)
            {
                LogEvents.Publish($"Receiver error occurred: {ex.Message}");
            }
        }
    }

    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public Message(int id, string content)
        {
            Id = id;
            Content = content;
        }
    }
}
