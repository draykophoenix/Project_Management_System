using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem
{
    internal class TaskFunctions
    {
        public static List<Task> Tasks = new List<Task>();
        internal static Dictionary<string, Task> s_taskDict = new Dictionary<string, Task>();
        internal static string? s_fileName = "";

        public static void LoadTasks(string[] tasksText)
        {
            string[][] taskItemsArray = (from taskString in tasksText select taskString.Split(new[] { ',' }, 3)).ToArray();

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

                uint timeToCompletion = uint.Parse(taskItemsArray[i][1]); // Add try/catch {|!|}

                // Getting dependencies from text to obj
                HashSet<Task> dependencies = new HashSet<Task>();
                if (taskItemsArray[i].Length == 3) // If the task has dependencies
                {
                    string[] dependenciesString = taskItemsArray[i][2].Split(",");

                    foreach (string dependencyID in dependenciesString)
                    {
                        s_taskDict.TryGetValue(dependencyID.Trim(), out Task dependency);
                        dependencies.Add(dependency);
                    }
                }
                task.Update(timeToCompletion, dependencies);
            }
        }
        public static void AddTask(string id, uint timeToCompletion, string[] dependenciesIDs)
        {
            HashSet<Task> dependencies = new HashSet<Task>();

            foreach (string dependencyID in dependenciesIDs)
            {
                s_taskDict.TryGetValue(dependencyID.Trim(), out Task dependency);
                dependencies.Add(dependency);
            }

            Task task = new Task(id, timeToCompletion, dependencies);
            Tasks.Add(task);
            s_taskDict.Add(id, task);
        }
        public static void DeleteTask(string idToDelete)
        {
            s_taskDict.TryGetValue(idToDelete.Trim(), out Task taskToDelete);

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
            s_taskDict.TryGetValue(id.Trim(), out Task task);
            task.ChangeTime(newTime);
        }
        public static void Save()
        {
            string[] taskStrings = (from task in Tasks select task.FormatFileString()).ToArray();
            File.WriteAllLines(s_fileName, taskStrings);
        }
        public static List<Task> Sequence()
        {
            List<Task> topOrdering = new List<Task>();

            List<Task> sCopyTasks = new List<Task>(Tasks);
            while (sCopyTasks.Count != 0)
            {
                Task v = (from freeTask in sCopyTasks where !freeTask.HasDependencies select freeTask).First();
                topOrdering.Add(v);
                DeleteTask(sCopyTasks, v);
            }
            return topOrdering;
        }
    }
}
