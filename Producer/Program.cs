using System;
using System.Threading.Tasks;
using MassTransit;
using Contracts;

namespace Producer
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
            });

            Console.WriteLine("Starting producer bus...");
            await busControl.StartAsync();
            try
            {
                // This URI points to the queue we want to send to
                var sendToUri = new Uri("rabbitmq://localhost/order-submitted-queue");
                var endpoint = await busControl.GetSendEndpoint(sendToUri);

                Console.WriteLine("Producer started.");
                Console.WriteLine("Type an order ID (GUID) and press Enter to send.");
                Console.WriteLine("Press just Enter to use a random ID.");
                Console.WriteLine("Type 'q' and press Enter to quit.");
                Console.WriteLine();

                while (true)
                {
                    Console.Write("Order ID (or 'q' to quit): ");
                    var input = Console.ReadLine();

                    if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
                        break;

                    Guid orderId;

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        // random id, looks like: 3f7b6c4f-56ed-45a0-a1a2-f987d902c099
                        orderId = Guid.NewGuid();
                    }
                    else if (!Guid.TryParse(input, out orderId))
                    {
                        Console.WriteLine("Invalid GUID, try again.");
                        continue;
                    }

                    var message = new OrderSubmitted
                    {
                        OrderId = orderId,
                        CustomerName = "Ali",
                        Total = 149.99m
                    };

                    await endpoint.Send(message);

                    Console.WriteLine(
                        $"[Producer] Sent OrderSubmitted: {message.OrderId} for {message.CustomerName}, Total: {message.Total}");
                }
            }
            finally
            {
                Console.WriteLine("Stopping producer bus...");
                await busControl.StopAsync();
            }
        }
    }
}

// IMPORTANT: message type MUST have a namespace (Contracts) so MassTransit is happy.
namespace Contracts
{
    public class OrderSubmitted
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; } = default!;
        public decimal Total { get; set; }
    }
}
