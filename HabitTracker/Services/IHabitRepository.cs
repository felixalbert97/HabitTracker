using System;
using System.Collections.Generic;
using System.Text;
using HabitTracker.Models;

namespace HabitTracker.Services
{
    public interface IHabitRepository
    {
        List<Habit> GetAllHabits();
        void AddHabit(Habit habit);
        void DeleteHabit(int id);

        HabitLog? GetLogForDate(int habitId, DateTime date);
        void AddLog(HabitLog log);
        List<HabitLog> GetLogsForHabit(int habitId);
    }
}
