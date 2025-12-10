using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using HabitTracker.Commands;
using HabitTracker.Models;
using HabitTracker.Services;
using System.Windows.Input;
using System.IO;
using System.Windows;

namespace HabitTracker.ViewModels
{
    public class MainViewModel
    {
        private readonly ITaskRepository _repo;

        public ObservableCollection<TaskViewModel> UncompletedTasks { get; set; } = 
            new ObservableCollection<TaskViewModel>();

        public ObservableCollection<TaskViewModel> CompletedTasks { get; set; } =
            new ObservableCollection<TaskViewModel>();
        public TaskViewModel? SelectedTask { get; set; }


        // TODO: evtl. durch Template ViewModel ersetzen
        public ObservableCollection<TaskTemplate> TaskTemplates { get; set; } =
            new ObservableCollection<TaskTemplate>();
        public TaskTemplate? SelectedTaskTemplate { get; set; }


        public ICommand MarkCompletedCommand { get; }
        public ICommand MarkUncompletedCommand { get; }
        public ICommand DeleteTaskCommand { get; }

        public MainViewModel()
        {
            _repo = new TaskRepository(Path.Combine("Data", "tasks.db"));

            MarkCompletedCommand = new RelayCommand(_ => MarkCompleted(), _ => SelectedTask != null);
            MarkUncompletedCommand = new RelayCommand(_ => MarkUncompleted(), _ => SelectedTask != null);
            DeleteTaskCommand = new RelayCommand(_ => DeleteTask(), _ => SelectedTask != null);

            //LoadHabits();
            //LoadHabitsDoneToday();
        }

        private void MarkCompleted()
        {

        }

        private void MarkUncompleted()
        {

        }

        private void DeleteTask()
        {
        }


        /*
        private readonly IHabitRepository _repo;

        public ObservableCollection<HabitViewModel> Habits { get; set; } =
            new ObservableCollection<HabitViewModel>();

        public ObservableCollection<HabitViewModel> HabitsDoneToday { get; set; } =
            new ObservableCollection<HabitViewModel>();

        private HabitViewModel? _selectedHabit;
        public HabitViewModel? SelectedHabit
        {
            get => _selectedHabit;
            set => _selectedHabit = value;
        }

        public string? NewHabitName { get; set; }
        public string? NewHabitCategory { get; set; }


        public ICommand AddHabitCommand { get; }
        public ICommand MarkDoneCommand { get; }
        public ICommand DeleteHabitCommand { get; }

        public MainViewModel()
        {   
            _repo = new HabitRepository(Path.Combine("Data", "habits.db"));

            AddHabitCommand = new RelayCommand(_ => AddHabit());
            MarkDoneCommand = new RelayCommand(_ => MarkDoneToday(), _ => SelectedHabit != null);
            DeleteHabitCommand = new RelayCommand(_ => DeleteHabit(), _ => SelectedHabit != null);

            LoadHabits();
            LoadHabitsDoneToday();
        }

        private void LoadHabits()
        {
            Habits.Clear();
            foreach (var h in _repo.GetAllHabits())
                Habits.Add(new HabitViewModel(h));
        }

        private void AddHabit()
        {
            if (NewHabitName == null || NewHabitName.Trim() == "")
            {
                MessageBox.Show("Bitte geben Sie einen Namen für die Gewohnheit ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            var habit = new Habit
            {
                Name = NewHabitName,
                Category = NewHabitCategory == null || NewHabitCategory.Trim() == "" ? "Keine" :  NewHabitCategory,
                CreatedAt = DateTime.Now
            };

            _repo.AddHabit(habit);
            LoadHabits();
        }

        private void DeleteHabit()
        {
            if (SelectedHabit == null) return;

            _repo.DeleteHabit(SelectedHabit.Habit.Id);
            LoadHabits();
        }

        private void MarkDoneToday()
        {
            if (SelectedHabit == null) return;

            var today = DateTime.Today;

            var existing = _repo.GetLogForDate(SelectedHabit.Habit.Id, today);

            if (existing == null)
            {
                var log = new HabitLog
                {
                    HabitId = SelectedHabit.Habit.Id,
                    Date = today
                };
                _repo.AddLog(log);
            }
            LoadHabitsDoneToday();
        }

        private void LoadHabitsDoneToday()
        {
            HabitsDoneToday.Clear();
            var today = DateTime.Today;
            var habits = _repo.GetHabitsForDate(today);
            foreach (var h in habits)
                HabitsDoneToday.Add(new HabitViewModel(h));
        }*/
    }
}