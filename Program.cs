
        using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MobileTaskTracker
{
    // Model for a single task
    public class TaskItem
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    class Program
    {
        // File path for persistent storage
        private static readonly string DataFilePath = "tasks.json";

        static void Main(string[] args)
        {
            Console.WriteLine("=== Mobile Task Tracker (Built on Phone!) ===");

            while (true)
            {
                Console.WriteLine("\n--- Menu ---");
                Console.WriteLine("1. Add Task");
                Console.WriteLine("2. List Tasks");
                Console.WriteLine("3. Complete Task");
                Console.WriteLine("4. Exit");
                Console.Write("Choose an option: ");

                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            AddTask();
                            break;
                        case "2":
                            ListTasks();
                            break;
                        case "3":
                            CompleteTask();
                            break;
                        case "4":
                            Console.WriteLine("Goodbye!");
                            return;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // Catch-all to keep the app running even if something unexpected happens
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        // Load tasks from JSON file
        private static List<TaskItem> LoadTasks()
        {
            if (!File.Exists(DataFilePath))
            {
                // If no file exists yet, start with an empty list
                return new List<TaskItem>();
            }

            try
            {
                string json = File.ReadAllText(DataFilePath);
                var tasks = JsonSerializer.Deserialize<List<TaskItem>>(json);
                return tasks ?? new List<TaskItem>();
            }
            catch (JsonException)
            {
                // Handle corrupted JSON gracefully: reset the file and start fresh
                Console.WriteLine("Warning: tasks.json is corrupted. Resetting database...");
                File.Delete(DataFilePath);
                return new List<TaskItem>();
            }
            catch (IOException ex)
            {
                throw new Exception("Cannot access file. Check permissions.", ex);
            }
        }

        // Save tasks to JSON file
        private static void SaveTasks(List<TaskItem> tasks)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true // Human-readable JSON
            };
            string json = JsonSerializer.Serialize(tasks, options);
            File.WriteAllText(DataFilePath, json);
        }

        private static void AddTask()
        {
            Console.Write("Enter task description: ");
            string? description = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(description))
            {
                Console.WriteLine("Description cannot be empty.");
                return;
            }

            var tasks = LoadTasks();
            int newId = tasks.Count == 0 ? 1 : tasks[tasks.Count - 1].Id + 1;

            var newTask = new TaskItem
            {
                Id = newId,
                Description = description,
                IsCompleted = false,
                CreatedAt = DateTime.Now
            };

            tasks.Add(newTask);
            SaveTasks(tasks);
            Console.WriteLine($"Task #{newId} added successfully!");
        }

        private static void ListTasks()
        {
            var tasks = LoadTasks();

            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks found. Add one!");
                return;
            }

            Console.WriteLine($"\nTotal tasks: {tasks.Count}");
            foreach (var task in tasks)
            {
                string status = task.IsCompleted ? "[DONE]" : "[TODO]";
                string symbol = task.IsCompleted ? "✅" : "⏳";
                Console.WriteLine($"{symbol} [{task.Id}] {status} - {task.Description} (Created: {task.CreatedAt:dd.MM HH:mm})");
            }
        }

        private static void CompleteTask()
        {
            Console.Write("Enter Task ID to mark as complete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var tasks = LoadTasks();
            var task = tasks.Find(t => t.Id == id);

            if (task == null)
            {
                Console.WriteLine("Task not found.");
                return;
            }

            task.IsCompleted = true;
            SaveTasks(tasks);
            Console.WriteLine($"Task #{id} marked as complete!");
        }
    }
}

    
