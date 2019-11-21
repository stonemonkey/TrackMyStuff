using System;

namespace TrackMyStuff.ApiGateway.Queries
{
    public class DeviceStatus
    {
        public string DeviceId { get; set; }

        public DateTime LastSeenAt { get; set; }
    }
}

