using System;
using System.Data.SQLite;
using System.Globalization;

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

                // Optionally drop old table
                Console.Write("Do you want to drop the existing Tasks table? (y/n): ");
                string dropChoice = Console.ReadLine()?.Trim().ToLower();
                if (dropChoice == "y")
                {
                    string dropTable = "DROP TABLE IF EXISTS Tasks;";
                    using (var dropCmd = new SQLiteCommand(dropTable, connection))
                    {
                        dropCmd.ExecuteNonQuery();
                        Console.WriteLine("⚠️ Existing Tasks table dropped.");
                    }
                }

                // Create table if not exists
                string createTable = @"CREATE TABLE IF NOT EXISTS Tasks (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Description TEXT,
                        CustomDate TEXT NOT NULL,
                        HoursSpent REAL DEFAULT 0,
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
                    Console.WriteLine("6. Search Tasks by Date");

                    Console.Write("Choose an option: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            // Add Task
                            string title;
                            do
                            {
                                Console.Write("Enter task title: ");
                                title = Console.ReadLine()?.Trim();
                                if (string.IsNullOrEmpty(title))
                                    Console.WriteLine("❌ Title cannot be empty.");
                            } while (string.IsNullOrEmpty(title));

                            string desc;
                            do
                            {
                                Console.Write("Enter task description: ");
                                desc = Console.ReadLine()?.Trim();
                                if (string.IsNullOrEmpty(desc))
                                    Console.WriteLine("❌ Description cannot be empty.");
                            } while (string.IsNullOrEmpty(desc));

                            string customDate;
                            DateTime parsedDate;
                            while (true)
                            {
                                Console.Write("Enter task date (yy/MM/dd or yyyy/MM/dd): ");
                                customDate = Console.ReadLine()?.Trim();
                                if (DateTime.TryParseExact(customDate,
                                        new[] { "yy/MM/dd", "yyyy/MM/dd" },
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None,
                                        out parsedDate))
                                {
                                    customDate = parsedDate.ToString("yy/MM/dd");
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("❌ Invalid date format.");
                                }
                            }

                            double hours;
                            while (true)
                            {
                                Console.Write("Enter hours spent: ");
                                if (double.TryParse(Console.ReadLine(), out hours) && hours >= 0)
                                    break;
                                else
                                    Console.WriteLine("❌ Invalid input. Hours must be a non-negative number.");
                            }

                            string insert = @"INSERT INTO Tasks (Title, Description, CustomDate, HoursSpent, CreatedAt)
                                              VALUES (@title, @desc, @customDate, @hours, @createdAt);";

                            using (var insertCmd = new SQLiteCommand(insert, connection))
                            {
                                insertCmd.Parameters.AddWithValue("@title", title);
                                insertCmd.Parameters.AddWithValue("@desc", desc);
                                insertCmd.Parameters.AddWithValue("@customDate", customDate);
                                insertCmd.Parameters.AddWithValue("@hours", hours);
                                insertCmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("o"));
                                insertCmd.ExecuteNonQuery();
                            }
                            Console.WriteLine("✅ Task inserted.");
                            break;

                        case "2":
                            // View Tasks
                            string select = "SELECT Id, Title, Description, CustomDate, HoursSpent, IsCompleted FROM Tasks;";
                            using (var selectCmd = new SQLiteCommand(select, connection))
                            using (var reader = selectCmd.ExecuteReader())
                            {
                                Console.WriteLine("\n📋 All Tasks:");
                                while (reader.Read())
                                {
                                    Console.WriteLine($"{reader["Id"]}. {reader["Title"]} - {reader["Description"]} - Date: {reader["CustomDate"]} - Hours: {reader["HoursSpent"]} - Done? {reader["IsCompleted"]}");
                                }
                            }
                            break;

                        case "3":
                            // Update Task (mark as completed)
                            int updateId;
                            while (true)
                            {
                                Console.Write("Enter task Id to mark as completed: ");
                                if (int.TryParse(Console.ReadLine(), out updateId) && updateId > 0)
                                    break;
                                else
                                    Console.WriteLine("❌ Invalid Id. Must be a positive integer.");
                            }

                            string update = "UPDATE Tasks SET IsCompleted = 1 WHERE Id = @id;";
                            using (var updateCmd = new SQLiteCommand(update, connection))
                            {
                                updateCmd.Parameters.AddWithValue("@id", updateId);
                                int rows = updateCmd.ExecuteNonQuery();
                                Console.WriteLine($"✅ Updated {rows} task(s).");
                            }
                            break;

                        case "4":
                            // Delete Task
                            int deleteId;
                            while (true)
                            {
                                Console.Write("Enter task Id to delete: ");
                                if (int.TryParse(Console.ReadLine(), out deleteId) && deleteId > 0)
                                    break;
                                else
                                    Console.WriteLine("❌ Invalid Id. Must be a positive integer.");
                            }

                            string delete = "DELETE FROM Tasks WHERE Id = @id;";
                            using (var deleteCmd = new SQLiteCommand(delete, connection))
                            {
                                deleteCmd.Parameters.AddWithValue("@id", deleteId);
                                int rows = deleteCmd.ExecuteNonQuery();
                                Console.WriteLine($"✅ Deleted {rows} task(s).");
                            }
                            break;

                        case "5":
                            running = false;
                            break;


                        case "6":
                            // Search Tasks by Date
                            DateTime searchDate;
                            string searchInput;
                            while (true)
                            {
                                Console.Write("Enter date to search (yy/MM/dd or yyyy/MM/dd): ");
                                searchInput = Console.ReadLine()?.Trim();
                                if (DateTime.TryParseExact(searchInput,
                                        new[] { "yy/MM/dd", "yyyy/MM/dd" },
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None,
                                        out searchDate))
                                {
                                    break; // exit the loop when a valid date is entered
                                }
                                else
                                {
                                    Console.WriteLine("❌ Invalid date format.");
                                }
                            }

                            string searchQuery = "SELECT Id, Title, Description, CustomDate, HoursSpent, IsCompleted " +
                                                 "FROM Tasks WHERE CustomDate = @date;";

                            using (var searchCmd = new SQLiteCommand(searchQuery, connection))
                            {
                                searchCmd.Parameters.AddWithValue("@date", searchDate.ToString("yy/MM/dd"));
                                using (var reader = searchCmd.ExecuteReader())
                                {
                                    Console.WriteLine($"\n📋 Tasks on {searchDate:yy/MM/dd}:");
                                    bool found = false;
                                    while (reader.Read())
                                    {
                                        found = true;
                                        Console.WriteLine($"{reader["Id"]}. {reader["Title"]} - {reader["Description"]} - Hours: {reader["HoursSpent"]} - Done? {reader["IsCompleted"]}");
                                    }
                                    if (!found)
                                    {
                                        Console.WriteLine("No tasks found for this date.");
                                    }
                                }
                            }
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








