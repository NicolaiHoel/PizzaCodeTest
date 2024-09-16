
using WebPizzaStatistics.Enums;

namespace WebPizzaStatistics.Models;

// Define an event class
public class Event
{
    public EventType Type { get; }
    public DateTime Timestamp { get; }
    public string CustomerId { get; }
    public string DataField { get; }

    public Event(EventType type, string customerId, string dataField)
    {
        Type = type;
        Timestamp = DateTime.Now;
        CustomerId = customerId;
        DataField = dataField;
    }
}
