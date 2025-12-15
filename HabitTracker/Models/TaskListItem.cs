using HabitTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTracker.Models
{

    public class TaskListItem
    {
        public int TaskLogId { get; init; }
        public string? Name { get; init; }
        public string? Category { get; init; }
        public DateTime DueDate { get; init; }
        public bool IsCompleted { get; init; }
        public DateTime CompletedAt { get; init; }
        public bool IsRecurring { get; init; }
        public string? OverdueText { get; set; }
    }
}
