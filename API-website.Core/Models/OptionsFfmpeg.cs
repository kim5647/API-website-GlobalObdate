using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_website.Core.Models
{
    public class OptionsFfmpeg
    {
        public string VideoName { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public bool Revers { get; set; }
        public double SlowdownFactor { get; set; }

    }
}
