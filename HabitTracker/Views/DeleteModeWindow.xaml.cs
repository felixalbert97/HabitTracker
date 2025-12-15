using HabitTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HabitTracker.Views
{
    public partial class DeleteModeWindow : Window
    {
        public DeleteModeWindow()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        } 

        private void OnDataContextChanged(object? sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is MainViewModel oldVm)
            {
                oldVm.RecurringTaskDeleted -= OnRecurringTaskDeleted;
                oldVm.RecurringTaskDeletionCanceled -= OnRecurringTaskDeletionCanceled;
            }

            if (e.NewValue is MainViewModel newVm)
            {
                newVm.RecurringTaskDeleted += OnRecurringTaskDeleted;
                newVm.RecurringTaskDeletionCanceled += OnRecurringTaskDeletionCanceled;
            }
        }

        private void OnRecurringTaskDeleted(object? sender, System.EventArgs e)
        {
            Close();
        }

        private void OnRecurringTaskDeletionCanceled(object? sender, System.EventArgs e)
        {
            Close();
        }
    }
}
