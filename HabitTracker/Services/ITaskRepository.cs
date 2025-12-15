using System;
using System.Collections.Generic;
using System.Text;
using HabitTracker.Models;

namespace HabitTracker.Services
{
    public interface ITaskRepository
    {
        IEnumerable<TaskTemplate> GetAllTaskTemplates();
        IEnumerable<TaskListItem> GetCompletedTasksForDate(DateTime date);
        IEnumerable<TaskListItem> GetUncompletedTasksForDate(DateTime date);

        void AddTaskTemplate(TaskTemplate taskTemplate);
        void DeleteTaskTemplate(int id);
        void ProcessTaskCreation(TaskCreationLog taskCreationLog);
        public void MarkTaskAsCompleted(int id, DateTime CompletedAt);
        public void MarkTaskAsUncompleted(int id);
        void DeleteTaskLog(int id, DeleteMode deleteMode);
        
    }
}
