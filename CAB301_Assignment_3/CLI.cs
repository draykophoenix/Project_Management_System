using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ProjectManagementSystem
{
    internal class CLI
    {
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
        private const string TITLE = "================ Project Management System ================================";
        private const string PAD = "> ";
        private static bool s_isChanged = false;
        public static void IncorrectInput()
        {
            Console.WriteLine(" ~ The provided input was of the incorrect type.\n ~ Press any key to return to the home screen \n...");
            Console.ReadKey();
            Console.Clear();
        }
        public static void ListTasks()
        {
            Console.Clear();
            Console.WriteLine(TITLE);
            Console.WriteLine($"== File: {TaskFunctions.s_fileName}{(s_isChanged ? "*" : "")}\n");
            Console.WriteLine("------------ Tasks ------------>");
            TaskFunctions.Tasks.ForEach(Console.WriteLine);
            Console.WriteLine("------------------------------->");
            Console.WriteLine();
        }

        public static void Main()
        {
            string[]? tasksText = null;

            while (tasksText is null)
            {
                Console.Clear();
                Console.WriteLine(TITLE + "\n");
                Console.WriteLine(PAD + "Enter the full name of the file that contains your tasks:");
                Console.Write("Name:");
                TaskFunctions.s_fileName = Console.ReadLine();

                try
                {
                    tasksText = File.ReadAllLines(TaskFunctions.s_fileName);
                }
                catch
                {
                    Console.WriteLine(" ~ Could not find a file with that name.\n ~ This path is relative to:\n\t" + Directory.GetCurrentDirectory() + "\n ~ Press any key to continue\n...");
                    Console.ReadKey();
                }
            }

            /*
            and read the information
            from the text file into the system.
            */
            TaskFunctions.LoadTasks(tasksText);

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

                ListTasks();

                switch (choice)
                {
                    case (int)Choice.EXIT:
                        if (s_isChanged)
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
                                TaskFunctions.Save();
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
                        Console.WriteLine(PAD + "What is the ID of the task you want to add?");
                        Console.Write("ID:");
                        string? idToAdd = Console.ReadLine();

                        Console.WriteLine(PAD + "How long does it take to complete this task?");
                        Console.Write("Time:");
                        if (!uint.TryParse(Console.ReadLine(), out uint timeToCompletion))
                        {
                            IncorrectInput();
                            break;
                        }

                        Console.WriteLine(PAD + "List the tasks that would need to be completed before you can start this task.\nEnter each task ID seperated by a comma (',') ");
                        Console.Write("Tasks:");
                        string[] dependenciesIDs = Console.ReadLine().Split(',');

                        TaskFunctions.AddTask(idToAdd, timeToCompletion, (dependenciesIDs[0] == " " ? dependenciesIDs : null));
                        s_isChanged = true;
                        break;

                    case (int)Choice.DELETE:
                        Console.WriteLine(PAD + "What is the ID of the task you want to remove?");
                        Console.Write("ID:");
                        string? idToDelete = Console.ReadLine();

                        TaskFunctions.DeleteTask(idToDelete);
                        s_isChanged = true;
                        break;

                    case (int)Choice.CHANGETIME:
                        Console.WriteLine(PAD + "What is the ID of the task you want to update?");
                        Console.Write("ID:");
                        string? idToChange = Console.ReadLine();
                        Console.WriteLine(PAD + "What is the new time to complete this task?");
                        Console.Write("Time:");
                        if (!uint.TryParse(Console.ReadLine(), out uint newTime))
                        {
                            IncorrectInput();
                            break;
                        }

                        TaskFunctions.ChangeTime(idToChange, newTime);
                        s_isChanged = true;
                        break;

                    case (int)Choice.SAVE:
                        TaskFunctions.Save();
                        s_isChanged = false;
                        Console.WriteLine("Task saved successfully\nPress any key to continue\n...");
                        Console.ReadKey();
                        break;

                    case (int)Choice.SEQUENCE:
                        List<Task>? sequence = TaskFunctions.Sequence();
                        if (sequence is null)
                        {
                            continue;
                        }
                        string topOrderingString = String.Join(",", sequence.Select(x => x.ID));
                        File.WriteAllText("Sequence.txt", topOrderingString);

                        Console.WriteLine($"Sequence: [ {topOrderingString} ]\n\thas been saved to Sequence.txt\nPress any key to continue\n...");
                        Console.ReadKey();
                        break;

                    case (int)Choice.EARLIESTTIMES:
                        try
                        {
                            TaskFunctions.EarliestTimes();
                        }
                        catch
                        {
                            break;
                        }
                        Console.WriteLine("-------- Earliest Times --------");
                        List<string> earliestTimes = new List<string>();
                        TaskFunctions.Tasks.ForEach(x => earliestTimes.Add($"{x.ID}, {x.NStart}"));
                        Console.WriteLine(String.Join('\n', earliestTimes));
                        Console.WriteLine("--------------------------------");
                        File.WriteAllLines("EarliestTimes.txt", earliestTimes);
                        Console.WriteLine($"Earliest times have been saved to EarliestTimes.txt\nPress any key to continue\n...");
                        Console.ReadKey();
                        TaskFunctions.EarliestTimes();

                        break;
                    default:
                        Console.WriteLine($"Please select a number from {Enum.GetValues(typeof(Choice)).Cast<int>().Min()} - {Enum.GetValues(typeof(Choice)).Cast<int>().Max()}");
                        break;
                }
            }

        }
    }
}
