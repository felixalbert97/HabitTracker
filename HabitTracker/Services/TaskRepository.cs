using HabitTracker.Models;
using Microsoft.Data.Sqlite;
using System.ComponentModel;
using System.IO;

namespace HabitTracker.Services
{
    public enum DeleteMode
    {
        Single,
        Uncompleted,
        Completed,
        All
    }

    public class TaskRepository : ITaskRepository
    {
        private readonly string _connectionString;

        public TaskRepository(string databasePath)
        {
            // relativen Pfad zur ausführbaren Anwendung auflösen
            var fullPath = Path.IsPathRooted(databasePath)
                ? databasePath
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, databasePath);

            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir); 
            }

            _connectionString = $"Data Source={fullPath};Mode=ReadWriteCreate";
            Initialize();
        }

        private void Initialize()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();

            cmd.CommandText =
            @"CREATE TABLE IF NOT EXISTS TaskTemplate (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT,
                Category TEXT,
                CreatedAt TEXT
              );";
            cmd.ExecuteNonQuery();

            cmd.CommandText =
            @"CREATE TABLE IF NOT EXISTS TaskCreationLog (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name Text,
                Category TEXT,
                StartDate TEXT,
                EndDate TEXT,
                RepeatPattern INTEGER,
                CreatedAt TEXT
                );";

            cmd.ExecuteNonQuery();

            cmd.CommandText =
                @"CREATE TABLE IF NOT EXISTS TaskLog (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TaskCreationLogId INTEGER,
                DueDate TEXT,
                IsCompleted INTEGER,
                CompletedAt TEXT,
                IsRecurring INTEGER,
                FOREIGN KEY(TaskCreationLogId) REFERENCES TaskCreationLog(Id)
                );";

            cmd.ExecuteNonQuery();
        }


        public void AddTaskTemplate(TaskTemplate taskTemplate)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            using var cmd = con.CreateCommand();
            cmd.CommandText = "INSERT INTO TaskTemplate (Name, Category, CreatedAt) VALUES (@name, @category, @createdAt);";
            cmd.Parameters.AddWithValue("@name", taskTemplate.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@category", taskTemplate.Category ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@createdAt", taskTemplate.CreatedAt.ToString("o"));

            cmd.ExecuteNonQuery();
        }

        public void ProcessTaskCreation(TaskCreationLog taskCreationLog)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            using var tran = con.BeginTransaction();
            using var cmd = con.CreateCommand();
            cmd.Transaction = tran;

            cmd.CommandText =
                "INSERT INTO TaskCreationLog (Name, Category, StartDate, EndDate, RepeatPattern, CreatedAt) " +
                "VALUES (@name, @category, @start, @end, @repeat, @created);";
            cmd.Parameters.AddWithValue("@name", taskCreationLog.Name);
            cmd.Parameters.AddWithValue("@category", taskCreationLog.Category);
            cmd.Parameters.AddWithValue("@start", taskCreationLog.StartDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@end", taskCreationLog.EndDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@repeat", (int)taskCreationLog.RepeatPattern);
            cmd.Parameters.AddWithValue("@created", taskCreationLog.CreatedAt.ToString("o"));

            cmd.ExecuteNonQuery();

            // last_insert_rowid() auf derselben Verbindung/Transaktion abfragen
            cmd.Parameters.Clear();
            cmd.CommandText = "SELECT last_insert_rowid();";
            var id64 = (long)cmd.ExecuteScalar()!;
            var id = (int)id64;

            // Objekt aktualisieren
            taskCreationLog.Id = id;

            tran.Commit();

            AddTaskLogs(taskCreationLog);
        }

        public static IEnumerable<TaskLog> CreateTaskLogs(TaskCreationLog taskCreationLog)
        {
            bool isRecurring = IsRecurringTask(taskCreationLog);

            DateTime dueDate = taskCreationLog.StartDate;
            while (true)
            {
                if (dueDate > taskCreationLog.EndDate)
                    break;
                yield return new TaskLog
                {
                    TaskCreationLogId = taskCreationLog.Id,
                    DueDate = dueDate,
                    IsCompleted = false,
                    IsRecurring = isRecurring,
                };
                switch (taskCreationLog.RepeatPattern)
                {
                    case RepeatPattern.Single:
                        {
                            yield break;
                        }
                    case RepeatPattern.Daily:
                        {
                            dueDate = dueDate.AddDays(1);
                            break;
                        }
                    case RepeatPattern.Weekly:
                        {
                            dueDate = dueDate.AddDays(7);
                            break;
                        }
                    case RepeatPattern.Monthly:
                        {
                            dueDate = dueDate.AddMonths(1);
                            break;
                        }
                    default:
                        {
                            yield break;
                        }
                }
            }
        }

        public static bool IsRecurringTask(TaskCreationLog taskCreationLog)
        {
            switch (taskCreationLog.RepeatPattern)
            {
                case RepeatPattern.Single:
                    {
                        return false;
                    }
                case RepeatPattern.Daily:
                    {
                        return taskCreationLog.EndDate >= taskCreationLog.StartDate.AddDays(1);
                    }
                case RepeatPattern.Weekly:
                    {
                        return taskCreationLog.EndDate >= taskCreationLog.StartDate.AddDays(7);
                    }
                case RepeatPattern.Monthly:
                    {
                        return taskCreationLog.EndDate >= taskCreationLog.StartDate.AddMonths(1);
                    }
                default:
                    {   
                        return false;
                    }
            }
        }

        public void AddTaskLogs(TaskCreationLog taskCreationLog)
        {
            IEnumerable<TaskLog> taskLogs = CreateTaskLogs(taskCreationLog);
            AddTaskLogs(taskLogs);
        }

        public void AddTaskLogs(IEnumerable<TaskLog> taskLogs)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            using var tran = con.BeginTransaction();
            using var cmd = con.CreateCommand();
            cmd.Transaction = tran;

            cmd.CommandText = "INSERT INTO TaskLog (TaskCreationLogId, DueDate, IsCompleted, IsRecurring) " +
                              "VALUES (@creationId, @due, @isCompleted, @isRecurring);";

            // Parameter einmalig anlegen (Initialwerte werden überschrieben)
            var pCreationId = cmd.Parameters.AddWithValue("@creationId", 0);
            var pDue = cmd.Parameters.AddWithValue("@due", "");
            var pIsCompleted = cmd.Parameters.AddWithValue("@isCompleted", 0);
            var pIsRecurring = cmd.Parameters.AddWithValue("@isRecurring", 0);

            var any = false;
            foreach (var log in taskLogs)
            {
                any = true;
                pCreationId.Value = log.TaskCreationLogId;
                pDue.Value = log.DueDate.ToString("yyyy-MM-dd");
                pIsCompleted.Value = log.IsCompleted ? 1 : 0;
                pIsRecurring.Value = log.IsRecurring ? 1 : 0;

                cmd.ExecuteNonQuery();
            }

            if (any)
                tran.Commit();
            else
                tran.Rollback();

        }

        public void MarkTaskAsCompleted(int id, DateTime CompletedAt)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            using var cmd = con.CreateCommand();
            cmd.CommandText = "UPDATE TaskLog SET IsCompleted = 1, CompletedAt = @cd WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@cd", CompletedAt.ToString("yyyy-MM-dd"));
            cmd.ExecuteNonQuery();
        }

        public void MarkTaskAsUncompleted(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            using var cmd = con.CreateCommand();
            cmd.CommandText = "UPDATE TaskLog SET IsCompleted = 0, CompletedAt = NULL WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void DeleteTaskLog(int id, DeleteMode deleteMode)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            using var cmd = con.CreateCommand();

            switch (deleteMode)
            {
                case DeleteMode.All:
                    {
                        cmd.CommandText = @"
                        DELETE FROM TaskLog 
                        WHERE TaskCreationLogId = (
                            SELECT TaskCreationLogId 
                            FROM TaskLog 
                            WHERE Id = @id
                        );";
                        break;
                    }
                case DeleteMode.Uncompleted:
                    {
                        cmd.CommandText = @"
                        DELETE FROM TaskLog 
                        WHERE TaskCreationLogId = (
                            SELECT TaskCreationLogId 
                            FROM TaskLog 
                            WHERE Id = @id
                        )
                        AND IsCompleted = 0;";
                        break;
                    }
                case DeleteMode.Completed:
                    {
                        cmd.CommandText = @"
                        DELETE FROM TaskLog 
                        WHERE TaskCreationLogId = (
                            SELECT TaskCreationLogId 
                            FROM TaskLog 
                            WHERE Id = @id
                        )
                        AND IsCompleted = 1;";
                        break;
                    }
                case DeleteMode.Single:
                    {
                        cmd.CommandText = "DELETE FROM TaskLog WHERE Id = @id;";
                        break;
                    }
            }
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

        }

        public void DeleteTaskTemplate(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            using var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM TaskTemplate WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public IEnumerable<TaskTemplate> GetAllTaskTemplates()
        { 

            using var con = new SqliteConnection(_connectionString);
            con.Open();

            using var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Category, CreatedAt FROM TaskTemplate;";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return new TaskTemplate
                {
                    Id = reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Category = reader.IsDBNull(2) ? null : reader.GetString(2),
                    CreatedAt = DateTime.Parse(reader.GetString(3))
                };
            }
        }

        public IEnumerable<TaskListItem> GetCompletedTasksForDate(DateTime date)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            using var cmd = con.CreateCommand();
            cmd.CommandText = @"SELECT tl.Id, tcl.Name, tcl.Category, tl.DueDate, tl.CompletedAt, tl.IsRecurring
                                FROM TaskLog tl 
                                JOIN TaskCreationLog tcl ON tl.TaskCreationLogId = tcl.Id
                                WHERE tl.IsCompleted = 1
                                AND tl.CompletedAt = @date;";

            cmd.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return new TaskListItem
                {
                    TaskLogId = reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Category = reader.IsDBNull(2) ? null : reader.GetString(2),
                    DueDate = DateTime.Parse(reader.GetString(3)),
                    IsCompleted = true,
                    CompletedAt = DateTime.Parse(reader.GetString(4)),
                    IsRecurring = reader.GetInt32(5) == 1,
                };
            }
            
        }

        public IEnumerable<TaskListItem> GetUncompletedTasksForDate(DateTime date)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            using var cmd = con.CreateCommand();
            cmd.CommandText = @"SELECT tl.Id, tcl.Name, tcl.Category, tl.DueDate, tl.IsRecurring
                                FROM TaskLog tl 
                                JOIN TaskCreationLog tcl ON tl.TaskCreationLogId = tcl.Id
                                WHERE tl.IsCompleted = 0
                                AND tl.DueDate <= @date;";

            cmd.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return new TaskListItem
                {
                    TaskLogId = reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Category = reader.IsDBNull(2) ? null : reader.GetString(2),
                    DueDate = DateTime.Parse(reader.GetString(3)),
                    IsCompleted = false,
                    IsRecurring = reader.GetInt32(4) == 1,
                };
            }
        }
    }
}
