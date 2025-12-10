using HabitTracker.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HabitTracker.Services
{
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
                Directory.CreateDirectory(dir); // stellt sicher, dass das Verzeichnis existiert
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
                TaskTemplateId INTEGER,
                StartDate TEXT,
                EndDate TEXT,
                RepeatPattern INTEGER,
                CreatedAt TEXT,
                FOREIGN KEY(TaskTemplateId) REFERENCES TaskTemplate(Id)
                );";

            cmd.ExecuteNonQuery();

            cmd.CommandText =
                @"CREATE TABLE IF NOT EXISTS TaskLog (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TaskTemplateId INTEGER,
                TaskCreationLogId INTEGER,
                DueDate TEXT,
                IsCompleted INTEGER,
                FOREIGN KEY(TaskTemplateId) REFERENCES TaskTemplate(Id)
                FOREIGN KEY(TaskCreationLogId) REFERENCES TaskCreationLog(Id)
                );";

            cmd.ExecuteNonQuery();
        }


        public void AddTaskLog(TaskLog taskLog)
        {
            throw new NotImplementedException();
        }

        public void AddTaskTemplate(TaskLog taskLog)
        {
            throw new NotImplementedException();
        }

        public void DeleteTaskLog(int id, bool deleteAllRepetitions)
        {
            throw new NotImplementedException();
        }

        public void DeleteTaskTemplate(int id)
        {
            throw new NotImplementedException();
        }

        public List<TaskTemplate> GetAllTaskTemplates()
        {
            throw new NotImplementedException();
        }

        public List<TaskLog> GetCompletedTasksLogsForDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public List<TaskLog> GetUncompletedTasksLogsForDate(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
