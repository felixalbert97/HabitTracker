using System.Configuration;
using System.Data;
using System.Windows;
using SQLitePCL;

namespace HabitTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Ensure native e_sqlite3 is initialized before any SQLite usage
            Batteries_V2.Init();

            base.OnStartup(e);
        }
    }
}
