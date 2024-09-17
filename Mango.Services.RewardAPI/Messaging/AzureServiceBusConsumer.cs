using Azure.Messaging.ServiceBus;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedRewardsSubscription;

        private IConfiguration _configuration;
        private RewardsService _rewardService;

        private ServiceBusProcessor _rewardProcessor;
        //private ServiceBusProcessor _registerUserProcessor;


        //PROCESSOR WHICH LISTENS TO QUEUE for any NEW MEssages which queue needs to send. 
        public AzureServiceBusConsumer(IConfiguration configuration, RewardsService rewardService)
        {
            _configuration = configuration;
            _rewardService = rewardService;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionStrings");
        
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionStrings");
            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            orderCreatedRewardsSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscription");

            var options = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,   // Manually complete messages
                MaxConcurrentCalls = 1          // Process messages one at a time for debugging
            };
            var client = new ServiceBusClient(serviceBusConnectionString);
            _rewardProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedRewardsSubscription);

        }

        public async Task Start()
        {
            _rewardProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestReceived;
            _rewardProcessor.ProcessErrorAsync += ErrorHandler;
            await _rewardProcessor.StartProcessingAsync();

        }

        private  Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnNewOrderRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            
            RewardsMessage objMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);

            try
            {
                //TODO try to log email.
                await _rewardService.UpdateRewards(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task Stop()
        {
            await _rewardProcessor.StopProcessingAsync();
            await _rewardProcessor.DisposeAsync();

        }
    }
}
//
////OnEmailCartRequestReceived;
//async args =>
//                {
//    var message = args.Message;
//    Console.WriteLine("test1");

//    Console.WriteLine(args.Message);

//    var body = Encoding.UTF8.GetString(message.Body);

//    CartDto objMessage = JsonConvert.DeserializeObject<CartDto>(body);

//    try
//    {
//        Console.WriteLine(args.Message);

//        //TODO try to log email.
//        await args.CompleteMessageAsync(args.Message);
//    }
//    catch (Exception ex)
//    {
//        throw;
//    }

//};