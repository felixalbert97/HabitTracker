using System;
using System.Collections.Generic;
using System.Text;
using HabitTracker.Models;

namespace HabitTracker.Services
{
    public interface ITaskRepository
    {
        List<TaskTemplate> GetAllTaskTemplates();
        void AddTaskTemplate(TaskLog taskLog);
        void DeleteTaskTemplate(int id);

        List<TaskLog> GetUncompletedTasksLogsForDate(DateTime date);
        List<TaskLog> GetCompletedTasksLogsForDate(DateTime date);
        void AddTaskLog(TaskLog taskLog);
        void DeleteTaskLog(int id, bool deleteAllRepetitions);

    }
}
