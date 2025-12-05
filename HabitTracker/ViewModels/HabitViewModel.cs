using System;
using System.Collections.Generic;
using System.Text;
using HabitTracker.Models;

namespace HabitTracker.ViewModels
{
    public class HabitViewModel
    {
        public Habit Habit { get; }

        public HabitViewModel(Habit habit)
        {
            Habit = habit;
        }

        public string Name => Habit.Name;
        public string Category => Habit.Category;
    }
}
