using System.Net;
using System.Net.Sockets;
using System.Text;
using ConsoleTables;
using WebPizzaStatistics.Models;

namespace WebPizzaStatistics;

internal class Program
{
    private static readonly Dictionary<string, Customer> Customers = new();
    private static readonly Dictionary<string, int> Orders = new(); // Track the orders
    private static readonly Dictionary<string, int> CompletedOrders = new();
    private static readonly Dictionary<string, int> LoyaltyPoints = new();

    private static void Main(string[] args)
    {
        Console.WriteLine("Web Pizza Statistics");
        Console.WriteLine("---------------------------------------------");

        // Start a separate thread to continuously update the console
        var displayThread = new Thread(DisplayStatisticsLoop);
        displayThread.Start();

        // Start listening for incoming TCP connections on port 8888
        var server = new TcpListener(IPAddress.Any, 8888);
        server.Start();

        Console.WriteLine("Listening for incoming TCP connections...");

        // Continuously accept incoming client connections
        while (true)
        {
            // Accept a new client connection
            var client = server.AcceptTcpClient();
            Console.WriteLine("Connected to a client.");

            // Create a new thread to handle communication with the client
            var clientThread = new Thread(() =>
            {
                // Get the network stream for reading and writing data
                var stream = client.GetStream();

                while (client.Connected)
                {
                    try
                    {
                        // Read data from the client
                        var buffer = new byte[1024];
                        var bytesRead = stream.Read(buffer, 0, buffer.Length);
                        var eventData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        // Parse the received data into event type and customer ID
                        var eventParts = eventData.Split(',');
                        if (eventParts.Length != 3)
                        {
                            Console.WriteLine("Invalid event data received.");
                            continue;
                        }

                        EventType eventType;
                        if (!Enum.TryParse(eventParts[0], out eventType))
                        {
                            Console.WriteLine("Invalid event type received.");
                            continue;
                        }

                        var customerId = eventParts[1];

                        // Create a new event of the received type
                        var newEvent = new Event(eventType, customerId);

                        // Update statistics based on the event type
                        UpdateStatistics(newEvent);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        break;
                    }
                }

                // Close the client connection
                client.Close();
                Console.WriteLine("Disconnected from the client.");
            });

            // Start the client communication thread
            clientThread.Start();
        }
    }

    // Method to continuously update the console with statistics
    private static void DisplayStatisticsLoop()
    {
        while (true)
        {
            // Clear the console
            Console.Clear();

            // Display the current statistics
            Console.WriteLine("Web Pizza Statistics");
            Console.WriteLine("---------------------------------------------");
            DisplayStatistics();

            // Sleep for a short duration before refreshing the screen
            Thread.Sleep(1000);
        }
    }

    private static void UpdateStatistics(Event newEvent)
    {
        lock (Customers)
        {
            switch (newEvent.Type)
            {
                case EventType.PizzaOrdered:
                    if (!Orders.ContainsKey(newEvent.CustomerId))
                        Orders[newEvent.CustomerId] = 0;
                    Orders[newEvent.CustomerId]++;
                    break;
                case EventType.CustomerCreated:
                    if (!Customers.ContainsKey(newEvent.CustomerId))
                        Customers[newEvent.CustomerId] = new Customer(newEvent.CustomerId);
                    break;
                case EventType.CustomerAbandonedOrder:
                    if (Orders.ContainsKey(newEvent.CustomerId))
                    {
                        Orders[newEvent.CustomerId]--;
                        if (Orders[newEvent.CustomerId] < 0)
                            Orders[newEvent.CustomerId] = 0;
                    }
                    break;
                case EventType.PaymentReceived:
                    if (Orders.ContainsKey(newEvent.CustomerId))
                    {
                        if (!CompletedOrders.ContainsKey(newEvent.CustomerId))
                            CompletedOrders[newEvent.CustomerId] = 0;
                        CompletedOrders[newEvent.CustomerId]++;
                        Orders[newEvent.CustomerId]--;
                        if (Customers.ContainsKey(newEvent.CustomerId))
                        {
                            var customer = Customers[newEvent.CustomerId];
                            customer.IsLoyaltyMember = CompletedOrders[newEvent.CustomerId] >= 3; // Customer becomes a loyalty member after 3 completed orders
                            if (customer.IsLoyaltyMember)
                            {
                                if (!LoyaltyPoints.ContainsKey(newEvent.CustomerId))
                                    LoyaltyPoints[newEvent.CustomerId] = 0;
                                LoyaltyPoints[newEvent.CustomerId] += 10; // Earn 10 loyalty points for each payment received
                            }
                        }
                    }
                    break;
                case EventType.LoyaltySignup:
                    if (Customers.ContainsKey(newEvent.CustomerId))
                    {
                        var customer = Customers[newEvent.CustomerId];
                        customer.IsLoyaltyMember = true;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private static bool OrderContainsCustomerId(Event newEvent)
    {
        return Orders.ContainsKey(newEvent.CustomerId);
    }

    private static void DisplayStatistics()
    {
        lock (Customers)
        {
            Console.WriteLine("Statistics:");
            var table = new ConsoleTable("Customer ID", "Orders", "Completed Orders", "Loyalty Member", "Loyalty Points");

            foreach (var customerId in Customers.Keys)
            {
                var orders = Orders.ContainsKey(customerId) ? Orders[customerId] : 0;
                var completedOrders = CompletedOrders.ContainsKey(customerId) ? CompletedOrders[customerId] : 0;
                var isLoyaltyMember = Customers[customerId].IsLoyaltyMember;
                var loyaltyPointsEarned = LoyaltyPoints.ContainsKey(customerId) ? LoyaltyPoints[customerId] : 0;

                table.AddRow(customerId, orders, completedOrders, isLoyaltyMember, loyaltyPointsEarned);
            }

            table.Write(Format.MarkDown);
        }
    }
}