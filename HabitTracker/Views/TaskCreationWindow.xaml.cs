using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HabitTracker.ViewModels;

namespace HabitTracker.Views
{
    public partial class TaskCreationWindow : Window
    {
        public TaskCreationWindow()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object? sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is TaskCreationViewModel oldVm)
                oldVm.TaskCreated -= OnTaskCreated;

            if (e.NewValue is TaskCreationViewModel newVm)
                newVm.TaskCreated += OnTaskCreated;
        }

        private void OnTaskCreated(object? sender, System.EventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void TemplatesListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBox listBox) return;
            if (DataContext is not TaskCreationViewModel vm) return;

            // 1) Falls ein Eingabefeld fokussiert ist, dessen Binding jetzt in die Quelle schreiben.
            //    So stellen wir sicher, dass die aktuelle UI-Eingabe ins ViewModel übernommen wird,
            //    bevor wir ReapplySelectedTemplate aufrufen.
            var focused = Keyboard.FocusedElement as FrameworkElement;
            if (focused != null)
            {
                // Beispiel: TextBox.Text
                var tb = focused as TextBox;
                if (tb != null)
                {
                    var be = tb.GetBindingExpression(TextBox.TextProperty);
                    be?.UpdateSource();
                }

                // ggf. weitere Typen hier behandeln (z.B. ComboBox) falls nötig
            }

            var dep = (DependencyObject)e.OriginalSource;
            var item = ItemsControl.ContainerFromElement(listBox, dep) as ListBoxItem;
            if (item == null) return;

            // Wenn das angeklickte Item bereits selektiert ist, die Felder erneut befüllen
            if (item.DataContext == vm.SelectedTaskTemplate)
            {
                vm.ReapplySelectedTemplate();

            }
        }

        private void TemplatesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
