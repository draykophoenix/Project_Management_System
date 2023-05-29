using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagementSystem
{
    internal class CLI
    {
        public static void Main()
        {
            /*
            Ask the user to enter the name of a text file in which the information about the tasks in
            a project and the dependencies among the tasks are stored ...
            */
            Console.WriteLine("Enter the file name containing your tasks{|!|}:");
            string? fileName = Console.ReadLine();

            /*
            and read the information
            from the text file into the system.
            */
            string[] tasksText = File.ReadAllLines(@"C:\Users\drayk\source\repos\Project_Management_System\CAB301_Assignment_3\Tasks.txt"); // {|!|}

            /*
             Add a new task with time needed to complete the task and other tasks that the task
             depends on into the project.
            */
            List<Task> tasks = new List<Task>();

            // First loop - init all tasks
            foreach (string task in tasksText)
            {
                string[] taskNames = task.Split(new[] { ',' }, 2);

                tasks.Add(new Task(taskNames[0]));
            }

            // Second loop: updates details for all tasks
            for (int i = 0; i < tasksText.Length; i++)
            {
                string[] taskItems = tasksText[i].Split(new[] { ',' }, 3);
                Task task = tasks[i];

                uint timeToCompletion = uint.Parse(taskItems[1]); // Add try/catch {|!|}

                // Getting dependencies from text to obj
                Task[] dependencies;
                if (taskItems.Length == 3) // If the task has dependencies
                {
                    string[] dependenciesText = taskItems[2].Split(",");
                    dependencies = new Task[dependenciesText.Length];

                    int j = 0;
                    foreach (string dependencyString in dependenciesText)
                    {
                        dependencies[j] = (from dependencyObj in tasks where dependencyObj.ID == dependencyString.Trim() select dependencyObj).First();
                    }
                } else {
                    dependencies = Array.Empty<Task>();
                }

                task.Update(timeToCompletion, dependencies);
            }

        }
    }
}
