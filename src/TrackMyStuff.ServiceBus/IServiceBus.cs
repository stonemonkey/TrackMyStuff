using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.InteropExtensions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TrackMyStuff.Common.Commands;

namespace TrackMyStuff.ServiceBus
{
    public interface IServiceBus
    {
        void Connect();
        Task PublishAsync(ICommand @event);
        void Subscribe<T, TH>() where T: ICommand where TH: ICommandHandler<T>;
    }

    public class AzureServiceBus : IServiceBus
    {
        private readonly IQueueClient _queueClient;
        private readonly IServiceProvider _serviceProvider;

        public AzureServiceBus(IQueueClient queueClient, IServiceProvider serviceProvider)
        {
            _queueClient = queueClient;
            _serviceProvider = serviceProvider;
        }

        public void Connect()
        {
            _queueClient.RegisterMessageHandler(
                ProcessMessagesAsync, ConfigureMessageHandlerOptions(ProcessExceptionAsync));
        }

        public Task PublishAsync(ICommand @event)
        {
            var eventName = @event.GetType().Name.Replace("ICommand", "");
            var jsonMessage = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            var message = new Message
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = body,
                Label = eventName,
            };

            return _queueClient.SendAsync(message);
        }

        public void Subscribe<T, TH>() where T: ICommand where TH: ICommandHandler<T>
        {
            _serviceProvider.Add
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken cancelationToken)
        {
            var messageBody = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine($"Received message: SequenceNumber: {message.SystemProperties.SequenceNumber} Body:{messageBody}");

            message.GetBody<ICommand>();
            // Complete the message so that it is not received again.
            // This can be done only if the queue Client is created in ReceiveMode.PeekLock mode (which is the default).
            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);

            // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
            // If queueClient has already been closed, you can choose to not call CompleteAsync() or AbandonAsync() etc.
            // to avoid unnecessary exceptions.
        }

        private Task ProcessExceptionAsync(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        private MessageHandlerOptions ConfigureMessageHandlerOptions(Func<ExceptionReceivedEventArgs, Task> onException)
        {
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            return new MessageHandlerOptions(onException)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false,
            };
        }     
    }
}