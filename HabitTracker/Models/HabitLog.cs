using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTracker.Models
{
    public class HabitLog
    {
        public int Id { get; set; }
        public int HabitId { get; set; }
        public DateTime Date { get; set; }
    }
}
