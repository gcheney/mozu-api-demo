using System; 

namespace MozuApiDemo.Test
{
    public class EventItem
    {
        public string Id { get; set; }
        public string EventId { get; set; }
        public string EntityId { get; set; }
        public string Topic { get; set; }
        public string Status { get; set; }
        public DateTime QueuedDateTime { get; set; }
        public DateTime ProcessedDateTime { get; set; }

        public int Retry { get; set; }
        public override string ToString()
        {
            return $"{Id} | {EntityId}~{Topic}~{Status}~{QueuedDateTime}";
        }

    }

    public enum EventStatus
    {
        Processed,
        Pending,
        Failed
    }
}
