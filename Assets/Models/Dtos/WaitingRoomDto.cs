using System;
using System.Collections.Generic;

namespace Assets.Models.Dtos
{
    public class WaitingRoomDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<WaitingPlayerDto> Players { get; set; }
        public string TokenPlayerAsHost { get; set; }
        public DateTime? StartTime { get; set; }
    }
}
