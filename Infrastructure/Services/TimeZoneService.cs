using Application.Interfaces.Services;

namespace Infrastructure.Services
{
    public class TimeZoneService: ITimeZoneService
    {
        private TimeZoneInfo gmt7TimeZone;
        public TimeZoneService()
        {
            gmt7TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        }
        public DateTime GetGMT7Time()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, gmt7TimeZone);
        }
    }
}
