using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTracker.Models
{
    public class TaskTemplate
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
