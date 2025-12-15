using System;
using System.Collections.Generic;
using HabitTracker.Models;
using HabitTracker.Services;

namespace HabitTracker.ViewModels
{
    public class TaskCreationRequestedEventArgs : EventArgs
    {
        public ITaskRepository Repository { get; }
        public DateTime StartDate { get; }


        public TaskCreationRequestedEventArgs(ITaskRepository repository, DateTime startDate)
        {
            Repository = repository;
            StartDate = startDate;
        }
        
            
    }
}