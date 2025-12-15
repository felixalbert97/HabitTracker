using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HabitTracker.ViewModels;

namespace HabitTracker.Views
{
    public partial class MainWindow
    {
        private const string connection = "Data/tasks.db"; //HARDCODED CONNECTION STRING
        private readonly MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            _mainViewModel = new MainViewModel(connection);
            DataContext = _mainViewModel;

            _mainViewModel.TaskCreationRequested += OnTaskCreationRequested;
            _mainViewModel.RecurringTaskDeletionRequested += OnTaskDeletionRequested;
        }

        public void OnTaskCreationRequested(object? sender, TaskCreationRequestedEventArgs args)
        {
            var taskCreationViewModel = new TaskCreationViewModel(args.Repository, args.StartDate);
            var taskCreationWindow = new TaskCreationWindow
            {
                Owner = this,
                DataContext = taskCreationViewModel
            };

            var result = taskCreationWindow.ShowDialog();
            if (result == true)
            {
                _mainViewModel.LoadTasks();
            }

        }

        public void OnTaskDeletionRequested(object? sender, System.EventArgs args)
        {
            var deleteModeWindow = new DeleteModeWindow()
            {
                Owner = this,
                DataContext = _mainViewModel,
            };

            var result = deleteModeWindow.ShowDialog();
        }
    }
}