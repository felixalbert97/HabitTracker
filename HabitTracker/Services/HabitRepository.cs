using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;
using HabitTracker.Models;

namespace HabitTracker.Services
{
    public class HabitRepository : IHabitRepository
    {
        private readonly string _connectionString;

        public HabitRepository(string databasePath)
        {
            _connectionString = $"Data Source={databasePath}";
            Initialize();
        }

        private void Initialize()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();

            cmd.CommandText =
            @"CREATE TABLE IF NOT EXISTS Habits (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT,
                Category TEXT,
                CreatedAt TEXT
              );";
            cmd.ExecuteNonQuery();

            cmd.CommandText =
            @"CREATE TABLE IF NOT EXISTS HabitLogs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                HabitId INTEGER,
                Date TEXT
              );";
            cmd.ExecuteNonQuery();
        }

        public List<Habit> GetAllHabits()
        {
            var result = new List<Habit>();

            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Category, CreatedAt FROM Habits;";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Habit
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Category = reader.GetString(2),
                    CreatedAt = DateTime.Parse(reader.GetString(3))
                });
            }

            return result;
        }

        public void AddHabit(Habit habit)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText =
                "INSERT INTO Habits (Name, Category, CreatedAt) VALUES (@name, @cat, @date);";

            cmd.Parameters.AddWithValue("@name", habit.Name);
            cmd.Parameters.AddWithValue("@cat", habit.Category);
            cmd.Parameters.AddWithValue("@date", habit.CreatedAt.ToString("o"));

            cmd.ExecuteNonQuery();
        }

        public void DeleteHabit(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM Habits WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public HabitLog? GetLogForDate(int habitId, DateTime date)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText =
                "SELECT Id, HabitId, Date FROM HabitLogs WHERE HabitId=@hid AND Date=@date";

            cmd.Parameters.AddWithValue("@hid", habitId);
            cmd.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new HabitLog
                {
                    Id = reader.GetInt32(0),
                    HabitId = reader.GetInt32(1),
                    Date = DateTime.Parse(reader.GetString(2))
                };
            }

            return null;
        }

        public void AddLog(HabitLog log)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText =
                "INSERT INTO HabitLogs (HabitId, Date) VALUES (@id, @date);";

            cmd.Parameters.AddWithValue("@id", log.HabitId);
            cmd.Parameters.AddWithValue("@date", log.Date.ToString("yyyy-MM-dd"));

            cmd.ExecuteNonQuery();
        }

        public List<HabitLog> GetLogsForHabit(int habitId)
        {
            var result = new List<HabitLog>();

            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText =
                "SELECT Id, HabitId, Date FROM HabitLogs WHERE HabitId=@hid;";

            cmd.Parameters.AddWithValue("@hid", habitId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new HabitLog
                {
                    Id = reader.GetInt32(0),
                    HabitId = reader.GetInt32(1),
                    Date = DateTime.Parse(reader.GetString(2))
                });
            }

            return result;
        }
    }
}