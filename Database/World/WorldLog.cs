using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.World
{
    public class WorldLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Source { get; set; } // e.g., "GameWorld", "Database", etc.
        public string? Type { get; set; } // The log message
    }
}
