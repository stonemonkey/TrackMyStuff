using System;

namespace TrackMyStuff.Common.Events
{
    public class DeviceStatusUpdatedEvent : IEvent
    {
        public string DeviceId { get; set; }
        public DateTime LastSeenAt { get; set; }
    }
}