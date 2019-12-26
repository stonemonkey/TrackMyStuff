using System;

namespace TrackMyStuff.DevicesService.DataAccess
{
    public class HeartBeat
    {
        public long Id { get; set; }

        public string DeviceId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

