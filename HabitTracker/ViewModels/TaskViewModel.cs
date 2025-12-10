using HabitTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTracker.ViewModels
{

    public class TaskViewModel(int taskLogId, string name, string category, DateTime dueDate, bool isCompleted)
    {
        public int TaskLogId { get; init; } = taskLogId;
        public string Name { get; init; } = name;
        public string Category { get; init; } = category;
        public DateTime DueDate { get; init; } = dueDate;
        public bool IsCompleted { get; init; } = isCompleted;
    }
}
