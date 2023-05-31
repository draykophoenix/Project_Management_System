using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagementSystem
{
    internal class CLI
    {
        private static List<Task> _tasks = new List<Task>();
        private static Dictionary<string, Task> _taskDict = new Dictionary<string, Task>();
        public static void LoadTasks(string fileName)
        {
            string[] tasksText = File.ReadAllLines(@"C:\Users\drayk\source\repos\Project_Management_System\CAB301_Assignment_3\Tasks.txt"); // {|!|}

            string[][] taskItemsArray = (from taskString in tasksText select taskString.Split(new[] { ',' }, 3)).ToArray();

            // First loop - init all tasks
            foreach (string[] taskItems in taskItemsArray)
            {
                Task task = new Task(taskItems[0]);
                _taskDict.Add(taskItems[0], task);
                _tasks.Add(task);
            }

            // Second loop: updates details for all tasks
            for (int i = 0; i < tasksText.Length; i++)
            {
                Task task = _tasks[i];

                uint timeToCompletion = uint.Parse(taskItemsArray[i][1]); // Add try/catch {|!|}

                // Getting dependencies from text to obj
                HashSet<Task> dependencies = new HashSet<Task>();
                if (taskItemsArray[i].Length == 3) // If the task has dependencies
                {
                    string[] dependenciesString = taskItemsArray[i][2].Split(",");

                    foreach (string dependencyID in dependenciesString)
                    {
                        _taskDict.TryGetValue(dependencyID.Trim(), out Task dependency);
                        dependencies.Add(dependency);
                    }
                }
                task.Update(timeToCompletion, dependencies);
            }
        }
        public static void ListTasks()
        {
            Console.WriteLine("======== Tasks ========");
            _tasks.ForEach(Console.WriteLine);
            Console.WriteLine("=======================");
        }
        public static void AddTask()
        {
            Console.WriteLine("--------");
            Console.WriteLine("What is the ID of the task you want to add?");
            Console.Write("ID:");
            string? id = Console.ReadLine();

            Console.WriteLine("How long does it take to complete this task?");
            Console.Write("Time:");
            uint timeToCompletion = uint.Parse(Console.ReadLine());

            Console.WriteLine("List the tasks that would need to be completed before you can start this task.\nEnter each task ID seperated by a comma (',') ");
            Console.Write("Tasks:");
            HashSet<Task> dependencies = new HashSet<Task>();

            string[] dependenciesString = Console.ReadLine().Split(',');
            foreach (string dependencyID in dependenciesString)
            {
                _taskDict.TryGetValue(dependencyID.Trim(), out Task dependency);
                dependencies.Add(dependency);
            }

            Task task = new Task(id, timeToCompletion, dependencies);
            _tasks.Add(task);
            _taskDict.Add(id, task);
        }
        public static void DeleteTask()
        {
            Console.WriteLine("--------");
            Console.WriteLine("What is the ID of the task you want to remove?");
            Console.Write("ID:");
            string? idToDelete = Console.ReadLine();

            Task taskToDelete = null;
            foreach (Task task in _tasks)
            {
                if (task.ID == idToDelete)
                {
                    taskToDelete = task;
                }
                else
                {
                    _taskDict.TryGetValue(idToDelete.Trim(), out Task dependencyToDelete);
                    task.DeleteDependency(dependencyToDelete);
                }
            }
            _tasks.Remove(taskToDelete);
            _taskDict.Remove(idToDelete);
        }

        public static void Main()
        {
            /*
            Ask the user to enter the name of a text file in which the information about the tasks in
            a project and the dependencies among the tasks are stored ...
            */
            Console.WriteLine("Enter the file name containing your tasks{|!|}:");
            Console.Write("Name:");
            string? fileName = Console.ReadLine();

            /*
            and read the information
            from the text file into the system.
            */
            LoadTasks(fileName);

            Console.WriteLine();
            ListTasks();

            AddTask();

            ListTasks();

            DeleteTask();

            ListTasks();
        }
    }
}
