using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem
{
    internal class TaskFunctions
    {
        public static List<Task> Tasks = new List<Task>();
        internal static Dictionary<string, Task> s_taskDict = new Dictionary<string, Task>();
        internal static string? s_fileName = "";

        private static void FileError(string message)
        {
            Console.WriteLine("! Error: " + message + "\n! Please fix the file and restart the application.\n! Press any key to exit\n...");
            Console.ReadKey();
            Environment.Exit(2);
        }
        public static void LoadTasks(string[] tasksText)
        {
            string[][] taskItemsArray = tasksText.Select(taskString => taskString.Split(new[] { ',' }, 3)).ToArray();

            // First loop - init all tasks
            foreach (string[] taskItems in taskItemsArray)
            {
                Task task = new Task(taskItems[0]);
                s_taskDict.Add(taskItems[0], task);
                Tasks.Add(task);
            }

            // Second loop: updates details for all tasks
            for (int i = 0; i < tasksText.Length; i++)
            {
                Task task = Tasks[i];

                if (!uint.TryParse(taskItemsArray[i][1], out uint timeToCompletion))
                {
                    FileError($"the file '{s_fileName}' has invalid time {taskItemsArray[i][1].Trim()} for task {task.ID}.");
                    
                }

                // Getting dependencies from text to obj
                HashSet<Task> dependencies = new HashSet<Task>();

                if (taskItemsArray[i].Length == 3)
                {
                    string[] dependenciesString = taskItemsArray[i][2].Split(",");

                    if (dependenciesString[0] != " ")
                    {
                        foreach (string dependencyID in dependenciesString)
                        {
                            if (!s_taskDict.TryGetValue(dependencyID.Trim(), out Task dependency))
                            {
                                FileError($"the file '{s_fileName}' does not have task '{dependencyID.Trim()}', though it is listed as a dependency for task '{task.ID}'");
                            }
                            dependencies.Add(dependency);
                        }
                    }
                } 
                task.Update(timeToCompletion, dependencies);
            }
        }
        public static void AddTask(string id, uint timeToCompletion, string[]? dependenciesIDs)
        {
            HashSet<Task> dependencies = new HashSet<Task>();

            if (dependenciesIDs is not null)
            {
                foreach (string dependencyID in dependenciesIDs)
                {
                    if (!s_taskDict.TryGetValue(dependencyID.Trim(), out Task dependency))
                    {
                        Console.WriteLine($" ~ Task '{dependencyID.Trim()}' could not be found.\n ~ Press any key to return to the home screen\n...");
                        Console.ReadKey();
                        return;
                    }
                    dependencies.Add(dependency);
                }
            }

            Task task = new Task(id, timeToCompletion, dependencies);
            Tasks.Add(task);
            try
            {
                s_taskDict.Add(id, task);
            }
            catch
            {
                Console.WriteLine($" ~ Task '{id}' already exists. Task list was not updated.\n ~ Press any key to return to the home screen\n...");
                Console.ReadKey();
                return;
            }
            
        }
        public static void DeleteTask(string idToDelete)
        {
            if (!s_taskDict.TryGetValue(idToDelete.Trim(), out Task taskToDelete))
            {
                Console.WriteLine($" ~ Task '{idToDelete}' could not be found.\n ~ Press any key to return to the home screen\n...");
                Console.ReadKey();
                return;
            }
            foreach (Task task in Tasks)
            {
                task.DeleteDependency(taskToDelete);
            }
            Tasks.Remove(taskToDelete);
            s_taskDict.Remove(idToDelete);
        }
        public static void DeleteTask(List<Task> tasks, Task taskToDelete)
        {
            foreach (Task task in tasks)
            {
                task.DeleteDependency(taskToDelete);
            }
            tasks.Remove(taskToDelete);
        }
        public static void ChangeTime(string id, uint newTime)
        {
            if (!s_taskDict.TryGetValue(id.Trim(), out Task task))
            {
                Console.WriteLine($" ~ Task '{id}' could not be found.\n ~ Press any key to return to the home screen\n...");
                Console.ReadKey();
                return;
            }
            task.ChangeTime(newTime);
        }
        public static void Save()
        {
            string[] taskStrings = Tasks.Select(task => task.FormatFileString()).ToArray();
            File.WriteAllLines(s_fileName, taskStrings);
        }
        public static List<Task>? Sequence()
        {
            List<Task> topOrdering = new List<Task>();

            var reversedTaskDict = s_taskDict.ToDictionary(x => x.Value, x => x.Key);

            Dictionary<Task, HashSet<string>> tasksClone = Tasks.ToDictionary(x => x, x => new HashSet<string>(x.HasDependencies ? x.FormatDependenciesString().Split(',') : new string[0]));

            while (tasksClone.Count != 0)
            {
                Task v = tasksClone.Keys.ToList().Find(x => tasksClone.GetValueOrDefault(x).Count == 0);
                topOrdering.Add(v);

                try
                {
                    tasksClone.Remove(v);
                }
                catch
                {
                    Console.WriteLine($" ~ Tasks have a dependency loop.\n ~ Please fix the tasks and try again.\n ~ Press any key to return to the home screen\n...");
                    Console.ReadKey();
                    return null;
                }

                foreach (HashSet<string> dependencySet in tasksClone.Values)
                {
                    dependencySet.Remove(v.ID);
                }
            }
            return topOrdering;
        }
        public static void EarliestTimes()
        {
            List<Task> sequence = Sequence();
            if (sequence is null)
            {
                throw new Exception("Tasks have a dependency loop.");
            }
            sequence.ForEach(x => x.ClearNetwork());

            foreach (Task node in sequence)
            {
                if (!node.HasDependencies)
                {
                    node.NStart = 0;
                    node.NFinish = node.TimeToCompletion;
                } 
                else
                {
                    uint maxFinish = node.Dependencies.Max(x => x.NFinish);

                    node.NStart = maxFinish;
                    node.NFinish = node.NStart + node.TimeToCompletion;
                }
            }
        }
    }
}
