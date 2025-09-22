using System;
using System.Data.SQLite;

namespace CrudAppTaskLogger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=tasks.db;Version=3;";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("✅ Connected to SQLite!");

                // Create table if not exists
                string createTable = @"CREATE TABLE IF NOT EXISTS Tasks (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Title TEXT NOT NULL,
                                        Description TEXT,
                                        IsCompleted INTEGER NOT NULL DEFAULT 0,
                                        CreatedAt TEXT NOT NULL
                                    );";

                using (var command = new SQLiteCommand(createTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                Console.WriteLine("✅ Table ensured.");

                bool running = true;

                while (running)
                {
                    Console.WriteLine("\n--- Task Menu ---");
                    Console.WriteLine("1. Add Task");
                    Console.WriteLine("2. View Tasks");
                    Console.WriteLine("3. Update Task");
                    Console.WriteLine("4. Delete Task");
                    Console.WriteLine("5. Exit");
                    Console.Write("Choose an option: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            // Add Task
                            Console.Write("Enter task title: ");
                            string title = Console.ReadLine();
                            Console.Write("Enter task description: ");
                            string desc = Console.ReadLine();

                            string insert = @"INSERT INTO Tasks (Title, Description, CreatedAt)
                                              VALUES (@title, @desc, @createdAt);";

                            using (var insertCmd = new SQLiteCommand(insert, connection))
                            {
                                insertCmd.Parameters.AddWithValue("@title", title);
                                insertCmd.Parameters.AddWithValue("@desc", desc);
                                insertCmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("o"));
                                insertCmd.ExecuteNonQuery();
                            }
                            Console.WriteLine("✅ Task inserted.");
                            break;

                        case "2":
                            // View Tasks
                            string select = "SELECT Id, Title, Description, IsCompleted FROM Tasks;";
                            using (var selectCmd = new SQLiteCommand(select, connection))
                            using (var reader = selectCmd.ExecuteReader())
                            {
                                Console.WriteLine("\n📋 All Tasks:");
                                while (reader.Read())
                                {
                                    Console.WriteLine($"{reader["Id"]}. {reader["Title"]} - {reader["Description"]} - Done? {reader["IsCompleted"]}");
                                }
                            }
                            break;

                        case "3":
                            // Update Task
                            Console.Write("Enter task Id to mark as completed: ");
                            if (int.TryParse(Console.ReadLine(), out int updateId))
                            {
                                string update = "UPDATE Tasks SET IsCompleted = 1 WHERE Id = @id;";
                                using (var updateCmd = new SQLiteCommand(update, connection))
                                {
                                    updateCmd.Parameters.AddWithValue("@id", updateId);
                                    int rows = updateCmd.ExecuteNonQuery();
                                    Console.WriteLine($"✅ Updated {rows} task(s).");
                                }
                            }
                            else
                            {
                                Console.WriteLine("❌ Invalid Id");
                            }
                            break;

                        case "4":
                            // Delete Task
                            Console.Write("Enter task Id to delete: ");
                            if (int.TryParse(Console.ReadLine(), out int deleteId))
                            {
                                string delete = "DELETE FROM Tasks WHERE Id = @id;";
                                using (var deleteCmd = new SQLiteCommand(delete, connection))
                                {
                                    deleteCmd.Parameters.AddWithValue("@id", deleteId);
                                    int rows = deleteCmd.ExecuteNonQuery();
                                    Console.WriteLine($"✅ Deleted {rows} task(s).");
                                }
                            }
                            else
                            {
                                Console.WriteLine("❌ Invalid Id");
                            }
                            break;

                        case "5":
                            running = false;
                            break;

                        default:
                            Console.WriteLine("❌ Invalid option, try again.");
                            break;
                    }
                }

                Console.WriteLine("👋 Goodbye!");
            }
        }
    }
}






