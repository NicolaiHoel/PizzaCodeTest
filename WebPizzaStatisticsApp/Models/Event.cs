
namespace WebPizzaStatistics.Models;

// Define an event class
public class Event
{
    public EventType Type { get; }
    public DateTime Timestamp { get; }
    public string CustomerId { get; }

    public Event(EventType type, string customerId)
    {
        Type = type;
        Timestamp = DateTime.Now;
        CustomerId = customerId;
    }
}
