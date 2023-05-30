
namespace ProjectManagementSystem
{
    internal class Task
    {
        public string ID { get; private set; }
        private uint _timeToCompletion;
        private Task[]? _dependencies;

        public Task(string id)
        {
            ID = id;
            _timeToCompletion = uint.MaxValue;
        }
        public void Update(uint timeToCompletion, Task[] dependencies)
        {
            _timeToCompletion = timeToCompletion;
            _dependencies = dependencies;
        }

        /*
         Add a new task with time needed to complete the task and other tasks that the task
         depends on into the project.
        */

        public override string ToString()
        {
            return $"{ID} ({_timeToCompletion})";
        }
    }
}
