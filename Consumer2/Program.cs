using System.Text;
using Azure.Messaging.ServiceBus;
using Consumer2;
using Core.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

string connectionString;
string topicName;
string subscriptionName;
var ownerNotifier = new OwnerNotifier();

LoadConfig();

using (var cancellationTokenSource = new CancellationTokenSource())
{
    await ProcessMessagesAsync(connectionString, topicName, subscriptionName, cancellationTokenSource.Token);
}

void LoadConfig()
{
    var builder = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();

    IConfiguration configuration = builder.Build();

    connectionString = configuration.GetConnectionString("ServiceBusConnectionString") ?? string.Empty;
    topicName = configuration["ServiceBusTopicName"] ?? string.Empty;
    subscriptionName = configuration["ServiceBusSubscriptionName"] ?? string.Empty;
}

async Task ProcessMessagesAsync(string connectionString, string topicName, string subscriptionName, CancellationToken cancellationToken)
{
    var receiver = new ServiceBusClient(connectionString).CreateReceiver(topicName, subscriptionName);

    while (!cancellationToken.IsCancellationRequested)
    {
        var receivedMessage = await receiver.ReceiveMessageAsync(maxWaitTime: TimeSpan.FromSeconds(5), cancellationToken);

        if (receivedMessage != null)
        {
            string messageBody = Encoding.UTF8.GetString(receivedMessage.Body);
            var booking = JsonConvert.DeserializeObject<Booking>(messageBody);

            if (booking != null)
            {
                ownerNotifier.NotifyOwner(booking);
            }
        }
    }

    await receiver.CloseAsync();
}
