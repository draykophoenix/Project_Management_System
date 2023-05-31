using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ProjectManagementSystem
{
    internal class CLI
    {
        private static List<Task> _tasks = new List<Task>();
        private static Dictionary<string, Task> _taskDict = new Dictionary<string, Task>();
        private enum Choice
        {
            EXIT,
            ADD,
            DELETE,
            CHANGETIME,
            SAVE,
            SEQUENCE,
            EARLIESTTIMES
        };
        private const string TITLE = "================ Project Management System ================";
        private const string PAD = "> ";
        private static string? fileName = "";
        private static bool isChanged = false;
        public static void IncorrectInput()
        {
            Console.WriteLine(" ~ The provided input was of the incorrect type ~ \n ~  Press any key to return to the home screen  ~ \n...");
            Console.ReadKey();
            Console.Clear();
        }
        public static void LoadTasks(string[] tasksText)
        {
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
            Console.Clear();
            Console.WriteLine(TITLE);
            Console.WriteLine($"== File: {fileName}{(isChanged? "*" : "")}\n");
            Console.WriteLine("------------ Tasks ------------>");
            _tasks.ForEach(Console.WriteLine);
            Console.WriteLine("------------------------------->");
            Console.WriteLine();
        }
        public static void AddTask()
        {
            Console.WriteLine(PAD + "What is the ID of the task you want to add?");
            Console.Write("ID:");
            string? id = Console.ReadLine();

            Console.WriteLine(PAD + "How long does it take to complete this task?");
            Console.Write("Time:");
            uint timeToCompletion = uint.Parse(Console.ReadLine());

            Console.WriteLine(PAD + "List the tasks that would need to be completed before you can start this task.\nEnter each task ID seperated by a comma (',') ");
            Console.Write("Tasks:");
            string[] dependenciesString = Console.ReadLine().Split(',');

            HashSet<Task> dependencies = new HashSet<Task>();

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
            Console.WriteLine(PAD + "What is the ID of the task you want to remove?");
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
        public static void ChangeTime()
        {
            Console.WriteLine(PAD + "What is the ID of the task you want to update?");
            Console.Write("ID:");
            string? id = Console.ReadLine();
            Console.WriteLine(PAD + "What is the new time to complete this task?");
            Console.Write("Time:");
            uint newTime = uint.Parse(Console.ReadLine());

            _taskDict.TryGetValue(id.Trim(), out Task task);
            task.ChangeTime(newTime);
        }
        public static void Save()
        {
            string[] taskStrings = (from task in _tasks select task.FormatFileString()).ToArray();
            File.WriteAllLines(fileName, taskStrings);
        }

        public static void Main()
        {
            /*
            Ask the user to enter the name of a text file in which the information about the tasks in
            a project and the dependencies among the tasks are stored ...
            */

            string[]? tasksText = null;

            while (tasksText is null)
            {
                Console.Clear();
                Console.WriteLine(TITLE + "\n");
                Console.WriteLine(PAD + "Enter the name of the file that contains your tasks:");
                Console.Write("Name:");
                fileName = Console.ReadLine();

                try
                {
                    tasksText = File.ReadAllLines(fileName);
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Could not find a file with that name.\nThis path is relative to :\n\t" + Directory.GetCurrentDirectory() + "\nPress any key to continue\n...");
                    Console.ReadKey();
                }
            }

            /*
            and read the information
            from the text file into the system.
            */
            LoadTasks(tasksText);

            while (true)
            {
                ListTasks();

                Console.WriteLine(PAD + "Please choose an action:");
                Console.WriteLine(PAD + "(1) Add a new task");
                Console.WriteLine(PAD + "(2) Remove a task");
                Console.WriteLine(PAD + "(3) Change the time it takes to complete a task");
                Console.WriteLine(PAD + "(4) Save the updated task list and details");
                Console.WriteLine(PAD + "(5) Find a sequence that the tasks can be completed in");
                Console.WriteLine(PAD + "(6) Find the earliest possible commencement time for each task");
                Console.WriteLine(PAD + "(0) Exit");
                Console.Write("Action:");
                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    IncorrectInput();
                    continue;
                }

                switch (choice)
                {
                    case (int)Choice.EXIT:
                        if (isChanged)
                        {
                            ListTasks();
                            Console.WriteLine(PAD + "You have unsaved changes. Do you want save them before exiting?");
                            Console.Write("(y/n):");
                            if (!char.TryParse(Console.ReadLine().ToLower(), out char boolChoice))
                            {
                                IncorrectInput();
                                break;
                            }
                            if (boolChoice == 'y')
                            {
                                // SAVE HERE
                            }
                            else if (!new char[] { 'y', 'n' }.Contains(boolChoice))
                            {
                                IncorrectInput();
                                break;
                            }
                        }
                        ListTasks();
                        Environment.Exit(0);
                        break;

                    case (int)Choice.ADD:
                        ListTasks();
                        AddTask();
                        isChanged = true;
                        break;

                    case (int)Choice.DELETE:
                        ListTasks();
                        DeleteTask();
                        isChanged = true;
                        break;

                    case (int)Choice.CHANGETIME:
                        ListTasks();
                        ChangeTime();
                        isChanged = true;
                        break;

                    case (int)Choice.SAVE:
                        ListTasks();
                        Save();
                        isChanged = false;
                        Console.WriteLine("Task saved successfully\nPress any key to continue\n...");
                        Console.ReadKey();
                        break;

                    case (int)Choice.SEQUENCE:

                        break;

                    case (int)Choice.EARLIESTTIMES:

                        break;
                    default:
                        Console.WriteLine($"Please select a number from {Enum.GetValues(typeof(Choice)).Cast<int>().Min()} - {Enum.GetValues(typeof(Choice)).Cast<int>().Max()}");
                        break;
                }
            }

        }
    }
}
