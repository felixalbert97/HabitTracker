using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTracker.Models
{
    public class TaskLog
    {
        public int Id { get; set; }
        public int TaskTemplateId { get; set; }
        public int TaskCreationLogId { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
