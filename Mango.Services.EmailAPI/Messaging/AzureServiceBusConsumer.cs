using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string registerUserQueue;
        private readonly string orderCreated_Topic;
        private readonly string orderCreated_Email_Subscription;


        private IConfiguration _configuration;
        private EmailService _emailService;

        private ServiceBusProcessor _emailCartProcessor;
        private ServiceBusProcessor _registerUserProcessor;
        private ServiceBusProcessor _emailOrderPlacedProcessor;




        //PROCESSOR WHICH LISTENS TO QUEUE for any NEW MEssages which queue needs to send. 
        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionStrings");
        
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionStrings");

            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");

            orderCreated_Topic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            orderCreated_Email_Subscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscription");



            var options = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,   // Manually complete messages
                MaxConcurrentCalls = 1          // Process messages one at a time for debugging
            };
            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue, options);
            _registerUserProcessor = client.CreateProcessor(registerUserQueue, options);

            _emailOrderPlacedProcessor= client.CreateProcessor(orderCreated_Topic, orderCreated_Email_Subscription);

        }

        

        private  Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            
            CartDto objMessage = JsonConvert.DeserializeObject<CartDto>(body);

            try
            {
                //TODO try to log email.
                await _emailService.EmailCartAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex) {
                throw;
            }
        }


        private async Task OnRegistrationRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            string objMessage = JsonConvert.DeserializeObject<string>(body);

            try
            {
                //TODO try to log email.
                await _emailService.RegisterUserEmailAndLog(body);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();

            _registerUserProcessor.ProcessMessageAsync += OnRegistrationRequestReceived;
            _registerUserProcessor.ProcessErrorAsync += ErrorHandler;
            await _registerUserProcessor.StartProcessingAsync();

            _emailOrderPlacedProcessor.ProcessMessageAsync += OnOrderPlacedRequestReceived;
            _emailOrderPlacedProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailOrderPlacedProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await  _emailCartProcessor.DisposeAsync();

            await _registerUserProcessor.StopProcessingAsync();
            await _registerUserProcessor.DisposeAsync();

            await _emailOrderPlacedProcessor.StopProcessingAsync();
            await _emailOrderPlacedProcessor.DisposeAsync();

        }

        private async Task OnOrderPlacedRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessage objMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);

            try
            {
                //TODO try to log email.
                await _emailService.LogOrderPlaced(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
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