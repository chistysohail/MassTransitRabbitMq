using System;
using System.Threading.Tasks;
using MassTransit;
using Contracts;

namespace Consumer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // Configure MassTransit to use RabbitMQ on localhost
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // This creates the queue 'order-submitted-queue' and listens on it
                cfg.ReceiveEndpoint("order-submitted-queue", e =>
                {
                    // simplest handler-style consumer
                    e.Handler<OrderSubmitted>(async context =>
                    {
                        var m = context.Message;
                        Console.WriteLine(
                            $"[Consumer] Received OrderSubmitted: {m.OrderId} for {m.CustomerName}, Total: {m.Total}");
                    });
                });
            });

            Console.WriteLine("Starting consumer bus...");
            await busControl.StartAsync();
            try
            {
                Console.WriteLine("Consumer listening on 'order-submitted-queue'.");
                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
            }
            finally
            {
                Console.WriteLine("Stopping consumer bus...");
                await busControl.StopAsync();
            }
        }
    }
}

// must be identical (namespace + class name) to Producer's version
namespace Contracts
{
    public class OrderSubmitted
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; } = default!;
        public decimal Total { get; set; }
    }
}
