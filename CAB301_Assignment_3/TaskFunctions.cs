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

                uint timeToCompletion = uint.Parse(taskItemsArray[i][1]); // Add try/catch {|!|}

                // Getting dependencies from text to obj
                HashSet<Task> dependencies = new HashSet<Task>();

                string[] dependenciesString = taskItemsArray[i][2].Split(",");

                foreach (string dependencyID in dependenciesString)
                {
                    s_taskDict.TryGetValue(dependencyID.Trim(), out Task dependency);
                    dependencies.Add(dependency);
                }
                
                dependencies.Remove(null);
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
            string[] taskStrings = Tasks.Select(task => task.FormatFileString()).ToArray();
            File.WriteAllLines(s_fileName, taskStrings);
        }
        public static List<Task> Sequence()
        {
            List<Task> topOrdering = new List<Task>();

            var reversedTaskDict = s_taskDict.ToDictionary(x => x.Value, x => x.Key);

            Dictionary<Task, HashSet<string>> tasksClone = Tasks.ToDictionary(x => x, x => new HashSet<string>(x.HasDependencies ? x.FormatDependenciesString().Split(',') : new string[0]));

            while (tasksClone.Count != 0)
            {
                Task v = tasksClone.Keys.ToList().Find(x => tasksClone.GetValueOrDefault(x).Count == 0);
                topOrdering.Add(v);

                tasksClone.Remove(v);
                foreach (HashSet<string> dependencySet in tasksClone.Values)
                {
                    dependencySet.Remove(v.ID);
                }
            }
            return topOrdering;
        }
        public static void EarliestTimes()
        {
            List<Task> topOrdering = Sequence();
            topOrdering.ForEach(x => x.ClearNetwork());

            foreach (Task node in topOrdering)
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
