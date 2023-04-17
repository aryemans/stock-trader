using System;
namespace stock_trader_backend.Models
{
	public struct ScheduleObject
	{
        public string? dayOfWeek { get; set; }
        public string? startTime { get; set; }
        public string? endTime { get; set; }
        public bool? isHoliday { get; set; }
    }
}

