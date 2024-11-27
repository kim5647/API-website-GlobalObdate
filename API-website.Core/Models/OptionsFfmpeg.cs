namespace API_website.Core.Models
{
    public class OptionsFfmpeg
    {
        public string VideoName { get; set; } = string.Empty;
        public int UserIdClaim { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public bool Revers { get; set; }
        public double SlowdownFactor { get; set; }
    }
}
