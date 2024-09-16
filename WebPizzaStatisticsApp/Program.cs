using System.Net;
using System.Net.Sockets;
using System.Text;
using WebPizzaStatistics.Enums;
using WebPizzaStatistics.Models;
using WebPizzaStatistics.Statistics;

namespace WebPizzaStatistics;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Web Pizza Statistics");
        Console.WriteLine("---------------------------------------------");

        // Start a separate thread to continuously update the console
        var displayThread = new Thread(StatisticsService.DisplayStatisticsLoop);
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
            var clientThread = new Thread(() => RunTCPClient(client));

            // Start the client communication thread
            clientThread.Start();
        }
    }

    private static void RunTCPClient(TcpClient client)
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
                var dataField = eventParts[2];

                // Create a new event of the received type
                var newEvent = new Event(eventType, customerId, dataField);

                // Update statistics based on the event type
                StatisticsService.UpdateStatistics(newEvent);
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
    }
}