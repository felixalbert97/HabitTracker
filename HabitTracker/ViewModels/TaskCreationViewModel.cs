using HabitTracker.Commands;
using HabitTracker.Models;
using HabitTracker.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace HabitTracker.ViewModels
{
    public class TaskCreationViewModel : INotifyPropertyChanged
    {
        private readonly ITaskRepository _repo;
        private TaskTemplate? _selectedTaskTemplate;
        private string? _nameField;
        private string? _categoryField;
        private RepeatPattern _selectedRepeatOption = RepeatPattern.Einmalig;
        private DateTime _startDate;
        private DateTime _endDate;

        public event EventHandler? TaskCreated;
        public event PropertyChangedEventHandler? PropertyChanged;

        // TODO: evtl. durch Template ViewModel ersetzen
        public ObservableCollection<TaskTemplate> TaskTemplates { get; } = new ObservableCollection<TaskTemplate>();

        public TaskTemplate? SelectedTaskTemplate
        {
            get => _selectedTaskTemplate;
            set
            {
                if (_selectedTaskTemplate == value) return;
                _selectedTaskTemplate = value;
                OnPropertyChanged();
                if (value != null)
                {
                    NameField = value.Name;
                    CategoryField = value.Category;
                }

                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string? NameField
        {
            get => _nameField;
            set
            {
                if (_nameField == value) return;
                _nameField = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string? CategoryField
        {
            get => _categoryField;
            set
            {
                if (_categoryField == value) return;
                _categoryField = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate == value) return;
                if (value > EndDate)
                {
                    EndDate = value;
                }
                _startDate = value;
                OnPropertyChanged();
            }
        }
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate == value) return;
                if (value < StartDate)
                {
                    MessageBox.Show("Das Enddatum darf nicht vor dem Startdatum liegen.", "Ungültiges Datum", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }  
                _endDate = value;
                OnPropertyChanged();
            }
        }

        public bool IsEndDateEnabled
        {
            get => SelectedRepeatOption != RepeatPattern.Einmalig;
        }

        public double EndDateTextOpacity
        {
            get => IsEndDateEnabled ? 1 : 0.5;
        }

        public RepeatPattern SelectedRepeatOption { 
            get => _selectedRepeatOption;
            set
            {
                if (_selectedRepeatOption == value) return;
                if (value == RepeatPattern.Einmalig)
                {
                    EndDate = StartDate;
                    OnPropertyChanged(nameof(EndDate));
                }
                _selectedRepeatOption = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEndDateEnabled));
                OnPropertyChanged(nameof(EndDateTextOpacity));
            }
        }

        public static RepeatPattern[] RepeatValues
        {
            get => Enum.GetValues<RepeatPattern>();
        }

        public ICommand TaskCreationCommand { get; }
        public ICommand AddTaskTemplateCommand { get; }
        public ICommand DeleteTaskTemplateCommand { get; }



        public TaskCreationViewModel(ITaskRepository repo, DateTime initialDate)
        {
            _repo = repo;
            StartDate = initialDate;
            EndDate = initialDate;

            TaskCreationCommand = new RelayCommand(_ => ProcessTaskCreation(), _ => !string.IsNullOrWhiteSpace(NameField));

            AddTaskTemplateCommand = new RelayCommand(_ => AddTaskTemplate(),
                _ => !string.IsNullOrWhiteSpace(NameField) &&
                !(NameField == _selectedTaskTemplate?.Name && CategoryField == _selectedTaskTemplate?.Category));
            DeleteTaskTemplateCommand = new RelayCommand(_ => DeleteTaskTemplate(), _ => SelectedTaskTemplate != null);

            LoadTaskTemplates();
        }

        private void LoadTaskTemplates()
        {
            TaskTemplates.Clear();
            foreach (TaskTemplate taskTemplate in _repo.GetAllTaskTemplates())
                TaskTemplates.Add(taskTemplate);
        }

        private void ProcessTaskCreation()
        {
            var taskCreationLog = new TaskCreationLog
            {
                Name = NameField!.Trim(),
                Category = CategoryField?.Trim(),
                StartDate = StartDate,
                EndDate = EndDate,
                RepeatPattern = SelectedRepeatOption,
                CreatedAt = DateTime.Now,
            };

            _repo.ProcessTaskCreation(taskCreationLog);

            TaskCreated?.Invoke(this, EventArgs.Empty);
        }

        private void AddTaskTemplate()
        {
            // Normalisiere Eingaben für Vergleich
            var name = NameField!.Trim();
            var category = CategoryField?.Trim();

            // Existenzprüfung: Name UND Kategorie müssen übereinstimmen (case-insensitive)
            var exists = TaskTemplates.Any(t =>
                string.Equals(t.Name?.Trim(), name, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(t.Category?.Trim(), category, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                MessageBox.Show("Ein Template mit diesem Namen und dieser Kategorie existiert bereits.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _repo.AddTaskTemplate(new TaskTemplate
            {
                Name = name,
                Category = category,
                CreatedAt = DateTime.Now
            });

            LoadTaskTemplates();
        }

        private void DeleteTaskTemplate()
        {
            _repo.DeleteTaskTemplate(SelectedTaskTemplate!.Id);
            LoadTaskTemplates();
        }

        // Wenn derselbe Eintrag erneut angeklickt wird, die Felder erneut befüllen
        public void ReapplySelectedTemplate()
        {
            if (SelectedTaskTemplate == null) return;
            NameField = SelectedTaskTemplate.Name;
            CategoryField = SelectedTaskTemplate.Category;
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
