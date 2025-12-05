using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using HabitTracker.Commands;
using HabitTracker.Models;
using HabitTracker.Services;
using System.Windows.Input;

namespace HabitTracker.ViewModels
{
    public class MainViewModel
    {
        private readonly IHabitRepository _repo;

        public ObservableCollection<HabitViewModel> Habits { get; set; } =
            new ObservableCollection<HabitViewModel>();

        private HabitViewModel? _selectedHabit;
        public HabitViewModel? SelectedHabit
        {
            get => _selectedHabit;
            set => _selectedHabit = value;
        }

        public ICommand AddHabitCommand { get; }
        public ICommand MarkDoneCommand { get; }
        public ICommand DeleteHabitCommand { get; }

        public MainViewModel()
        {
            _repo = new HabitRepository("habits.db");

            AddHabitCommand = new RelayCommand(_ => AddHabit());
            MarkDoneCommand = new RelayCommand(_ => MarkDoneToday(), _ => SelectedHabit != null);
            DeleteHabitCommand = new RelayCommand(_ => DeleteHabit(), _ => SelectedHabit != null);

            LoadHabits();
        }

        private void LoadHabits()
        {
            Habits.Clear();
            foreach (var h in _repo.GetAllHabits())
                Habits.Add(new HabitViewModel(h));
        }

        private void AddHabit()
        {
            var habit = new Habit
            {
                Name = "Neue Gewohnheit",
                Category = "Allgemein",
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
        }
    }
}