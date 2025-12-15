using HabitTracker.Commands;
using HabitTracker.Models;
using HabitTracker.Services;
using HabitTracker.Helpers;
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
        private TaskListItem? _selectedCompletedTask;
        private TaskListItem? _selectedUncompletedTask;

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value, onChanged: () => LoadTasks());
        }

        public ObservableCollection<TaskListItem> UncompletedTasks { get; set; } =
            new ObservableCollection<TaskListItem>();

        public ObservableCollection<TaskListItem> CompletedTasks { get; set; } =
            new ObservableCollection<TaskListItem>();

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

        public EnumItem<DeleteMode> SelectedDeleteMode { get; set; }
        public EnumItem<DeleteMode>[] DeleteModes { get; }

        public ICommand OpenTaskCreationCommand { get; }
        public ICommand MarkCompletedCommand { get; }
        public ICommand MarkUncompletedCommand { get; }
        public ICommand DeleteCompletedTaskCommand { get; }
        public ICommand DeleteUncompletedTaskCommand { get; }
        public ICommand ConfirmDeleteCommand { get; }
        public ICommand CancelDeleteCommand { get; }

        public event EventHandler<TaskCreationRequestedEventArgs>? TaskCreationRequested;
        public event EventHandler? RecurringTaskDeletionRequested;
        public event EventHandler? RecurringTaskDeleted;
        public event EventHandler? RecurringTaskDeletionCanceled;
        public event PropertyChangedEventHandler? PropertyChanged;


        public MainViewModel(string connection)
        {
            _repo = new TaskRepository(connection);

            OpenTaskCreationCommand = new RelayCommand(_ => OnOpenTaskCreation());
            MarkCompletedCommand = new RelayCommand(_ => MarkCompleted(), _ => SelectedUncompletedTask != null);
            MarkUncompletedCommand = new RelayCommand(_ => MarkUncompleted(), _ => SelectedCompletedTask != null);
            DeleteCompletedTaskCommand = new RelayCommand(_ => OnDeleteCompletedTask(), _ => SelectedCompletedTask != null);
            DeleteUncompletedTaskCommand = new RelayCommand(_ => OnDeleteUncompletedTask(), _ => SelectedUncompletedTask != null);
            ConfirmDeleteCommand = new RelayCommand(_ => DeleteTask());
            CancelDeleteCommand = new RelayCommand(_ => CancelDelete());


            DeleteModes = new EnumItem<DeleteMode>[]
            {
                new EnumItem<DeleteMode>(DeleteMode.Single, "Nur diese Aufgabe"),
                new EnumItem<DeleteMode>(DeleteMode.Uncompleted, "Ausstehende Aufgaben"),
                new EnumItem<DeleteMode>(DeleteMode.Completed, "Erledigte Aufgaben"),
                new EnumItem<DeleteMode>(DeleteMode.All, "Alle Aufgaben")
            };

            SelectedDeleteMode = DeleteModes[0];

            LoadTasks();
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

        private void OnDeleteCompletedTask()
        {
            if (SelectedCompletedTask!.IsRecurring)
            {
                RecurringTaskDeletionRequested?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                DeleteTask();
            }
        }

        private void OnDeleteUncompletedTask()
        {
            if (SelectedUncompletedTask!.IsRecurring)
            {
                RecurringTaskDeletionRequested?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                DeleteTask();
            }
        }

        private void DeleteTask()
        {
            if (SelectedUncompletedTask != null)
            {
                _repo.DeleteTaskLog(SelectedUncompletedTask.TaskLogId, SelectedDeleteMode.Value);

                if (SelectedUncompletedTask.IsRecurring)
                {
                    RecurringTaskDeleted?.Invoke(this, EventArgs.Empty);
                }
            }
            else if (SelectedCompletedTask != null)
            {
                _repo.DeleteTaskLog(SelectedCompletedTask.TaskLogId, SelectedDeleteMode.Value);

                if (SelectedCompletedTask.IsRecurring)
                {
                    RecurringTaskDeleted?.Invoke(this, EventArgs.Empty);
                }
            }

            SelectedDeleteMode = DeleteModes[0]; // Reset to default
            LoadTasks();
        }

        private void CancelDelete()
        {
            RecurringTaskDeletionCanceled?.Invoke(this, EventArgs.Empty);
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
    }
}