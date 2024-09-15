using System.Net.Sockets;
using System.Text;

namespace WebPizzaEventSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Web Pizza Event Simulator");
            Console.WriteLine("---------------------------");

            // Connect to the TCP endpoint (Receiver app) running on localhost at port 8888
            using (TcpClient client = new TcpClient("localhost", 8888))
            {
                NetworkStream stream = client.GetStream();

                // Define 10 different customer IDs
                string[] customerIds = new string[10];
                for (int i = 0; i < customerIds.Length; i++)
                {
                    customerIds[i] = Guid.NewGuid().ToString();
                }

                // Simulate sending events continuously
                while (true)
                {
                    // Generate random event data using a random customer ID
                    string eventData = GenerateEventData(customerIds);

                    // Convert event data to bytes
                    byte[] bytesToSend = Encoding.UTF8.GetBytes(eventData);

                    // Send the event data to the receiver app
                    stream.Write(bytesToSend, 0, bytesToSend.Length);
                    Console.WriteLine($"Sent event data: {eventData}");

                    // Pause for a short duration before sending the next event
                    Thread.Sleep(100);
                }
            }
        }

        enum EventType
        {
            PizzaOrdered,
            CustomerCreated,
            CustomerAbandonedOrder,
            PizzaPrepared,
            DeliveryDispatched,
            DeliveryDelivered,
            PaymentReceived,
            CustomerFeedbackReceived,
            LoyaltySignup,
            LoyaltyPointsEarned // New event type for earning loyalty points
        }

        // Method to generate random event data
        static string GenerateEventData(string[] customerIds)
        {
            Random rand = new Random();
            int eventType = rand.Next(0, 9); // Generate a random event type number between 1 and 10

            // Choose a random customer ID from the array of customer IDs
            string customerId = customerIds[rand.Next(customerIds.Length)];

            // Generate additional data based on the event type
            string additionalData = (EventType)eventType switch
            {
                EventType.PizzaOrdered => "PizzaType: Pepperoni",
                EventType.CustomerCreated => "CustomerType: Web",
                EventType.CustomerAbandonedOrder => "Reason: OutOfStock",
                EventType.PizzaPrepared => "PizzaType: Margherita",
                EventType.DeliveryDispatched => "DeliveryMethod: Bike",
                EventType.DeliveryDelivered => "DeliveryTime: 30 minutes",
                EventType.PaymentReceived => "PaymentMethod: CreditCard",
                EventType.CustomerFeedbackReceived => "Rating: 5",
                EventType.LoyaltySignup => "LoyaltyStatus: Active",
                EventType.LoyaltyPointsEarned => "PointsEarned: 10",
                _ => ""
            };

            // Combine event type, customer ID, and additional data into a single string
            return $"{eventType},{customerId},{additionalData}";
        }
    }
}