using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models.Dto;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private IConfiguration _configuration;

        private ServiceBusProcessor _emailCartProcessor;

        //PROCESSOR WHICH LISTENS TO QUEUE for any NEW MEssages which queue needs to send. 
        public AzureServiceBusConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionStrings");
        
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionStrings");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            var options = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,   // Manually complete messages
                MaxConcurrentCalls = 1          // Process messages one at a time for debugging
            };
            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue, options);

        }

        public async Task Start()
        {
            Console.WriteLine("Starting processor email cart req received...");
            try
            {
                _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            }
            catch (Exception ex) {
                throw;
            }
            Console.WriteLine("started processor...");

            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            Console.WriteLine("started error handler...");
            await _emailCartProcessor.StartProcessingAsync();
            Console.WriteLine("Message processing started...");
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
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await  _emailCartProcessor.DisposeAsync();
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