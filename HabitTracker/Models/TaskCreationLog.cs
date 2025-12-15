using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTracker.Models
{ 

    public class TaskCreationLog
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public RepeatPattern RepeatPattern { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
