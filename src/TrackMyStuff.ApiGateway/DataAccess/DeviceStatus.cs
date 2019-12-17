using System;

namespace TrackMyStuff.ApiGateway.DataAccess
{
    public class DeviceStatus
    {
        public string DeviceId { get; set; }

        public DateTime LastSeenAt { get; set; }
    }
}

