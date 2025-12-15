using HabitTracker.Commands;
using HabitTracker.Models;
using HabitTracker.Services;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace HabitTracker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ITaskRepository _repo;
        private DateTime _selectedDate = DateTime.Today;

        public ObservableCollection<TaskListItem> UncompletedTasks { get; set; } =
            new ObservableCollection<TaskListItem>();

        public ObservableCollection<TaskListItem> CompletedTasks { get; set; } =
            new ObservableCollection<TaskListItem>();

        private TaskListItem? _selectedCompletedTask;
        private TaskListItem? _selectedUncompletedTask;

        public TaskListItem? SelectedCompletedTask
        {
            get => _selectedCompletedTask;
            set
            {
                if (SetProperty(ref _selectedCompletedTask, value))
                {
                    if (value != null)
                    {
                        // Auswahl in der anderen Liste aufheben
                        SelectedUncompletedTask = null;
                    }
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public TaskListItem? SelectedUncompletedTask
        {
            get => _selectedUncompletedTask;
            set
            {
                if (SetProperty(ref _selectedUncompletedTask, value))
                {
                    if (value != null)
                    {
                        // Auswahl in der anderen Liste aufheben
                        SelectedCompletedTask = null;
                    }
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value, onChanged: () => LoadTasks());
        }

        public event EventHandler<TaskCreationRequestedEventArgs>? TaskCreationRequested;
        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand MarkCompletedCommand { get; }
        public ICommand MarkUncompletedCommand { get; }
        public ICommand DeleteCompletedTaskCommand { get; }
        public ICommand DeleteUncompletedTaskCommand { get; }
        public ICommand OpenTaskCreationCommand { get; }

        public MainViewModel()
        {
            _repo = new TaskRepository(Path.Combine("Data", "tasks.db"));

            MarkCompletedCommand = new RelayCommand(_ => MarkCompleted(), _ => SelectedUncompletedTask != null);
            MarkUncompletedCommand = new RelayCommand(_ => MarkUncompleted(), _ => SelectedCompletedTask != null);
            DeleteCompletedTaskCommand = new RelayCommand(_ => DeleteCompletedTask(), _ => SelectedCompletedTask != null);
            DeleteUncompletedTaskCommand = new RelayCommand(_ => DeleteUncompletedTask(), _ => SelectedUncompletedTask != null);

            OpenTaskCreationCommand = new RelayCommand(_ => OnOpenTaskCreation());

            LoadTasks();
        }

        // Hilfsmethode für PropertyChanged mit optionaler Aktion nach Setzen
        protected bool SetProperty<T>(ref T field, T value, Action? onChanged = null, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            onChanged?.Invoke();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        public void LoadTasks()
        { 
            LoadCompletedTasks();
            LoadUncompletedTasks();
        }

        public void LoadCompletedTasks()
        {
            CompletedTasks.Clear();
            foreach (TaskListItem t in _repo.GetCompletedTasksForDate(SelectedDate))
            {
                TaskListItem taskListItem = SetOverdueText(t);
                CompletedTasks.Add(taskListItem);
            }
        }

        public void LoadUncompletedTasks()
        {
            UncompletedTasks.Clear();
            foreach (TaskListItem t in _repo.GetUncompletedTasksForDate(SelectedDate))
            {
                TaskListItem taskListItem = SetOverdueText(t);
                UncompletedTasks.Add(taskListItem);
            }
        }

        private TaskListItem SetOverdueText(TaskListItem t)
        {
            string overdueText = "";

            if (t.IsCompleted)
            {
                if (t.DueDate.Date == t.CompletedAt.Date)
                {
                    overdueText = "Erledigt";
                }
                else if (t.DueDate.Date < t.CompletedAt.Date)
                {
                    var daysOverdue = (t.CompletedAt.Date - t.DueDate.Date).Days;
                    string dayWord = daysOverdue == 1 ? "Tag" : "Tagen";
                    overdueText = $"Erledigt nach {daysOverdue} {dayWord}";
                }

                t.OverdueText = overdueText;
                return t;
            }
            else
            {
                if (t.DueDate.Date == SelectedDate)
                {
                    overdueText = "Heute fällig";
                }
                else if (t.DueDate.Date < SelectedDate)
                {
                    var daysOverdue = (SelectedDate - t.DueDate.Date).Days;
                    string dayWord = daysOverdue == 1 ? "Tag" : "Tagen";
                    overdueText = $"Seit {daysOverdue} {dayWord} überfällig";
                }

                // TODO: Implement functionality for a time span to see in the MainWindow how overdue or how many days until due
                /*
                else
                {
                    var daysUntilDue = (t.DueDate.Date - SelectedDate).Days;
                    string dayWord = daysUntilDue == 1 ? "Tag" : "Tagen";
                    overdueText = $"In {daysUntilDue} {dayWord} fällig";
                }
                */
                t.OverdueText = overdueText;
                return t;
            }
        }

        private void OnOpenTaskCreation()
        {
            var args = new TaskCreationRequestedEventArgs(_repo, SelectedDate);
            TaskCreationRequested?.Invoke(this, args);
        }

        private void MarkCompleted()
        {
            _repo.MarkTaskAsCompleted(SelectedUncompletedTask!.TaskLogId, SelectedDate);
            LoadTasks();
        }

        private void MarkUncompleted()
        {
            _repo.MarkTaskAsUncompleted(SelectedCompletedTask!.TaskLogId);
            LoadTasks();
        }

        private void DeleteCompletedTask()
        {
            //TODO: Implement repeat pattern handling when deleting tasks
            _repo.DeleteTaskLog(SelectedCompletedTask!.TaskLogId, false);
            LoadCompletedTasks();
        }

        private void DeleteUncompletedTask()
        {
            //TODO: Implement repeat pattern handling when deleting tasks
            _repo.DeleteTaskLog(SelectedUncompletedTask!.TaskLogId, false);
            LoadUncompletedTasks();
        }
    }
}