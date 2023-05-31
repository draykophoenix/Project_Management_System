
using System.Collections.Generic;
using System;

namespace ProjectManagementSystem
{
    internal class Task
    {
        public string ID { get; private set; }
        private uint _timeToCompletion;
        private HashSet<Task>? _dependencies;

        public Task(string id)
        {
            ID = id;
            _timeToCompletion = uint.MaxValue;
        }
        public Task(string id, uint timeToCompletion, HashSet<Task> dependencies)
        {
            ID = id;
            _timeToCompletion = timeToCompletion;
            _dependencies = dependencies;
        }
        public void Update(uint timeToCompletion, HashSet<Task> dependencies)
        {
            _timeToCompletion = timeToCompletion;
            _dependencies = dependencies;
        }
        public void DeleteDependency(Task taskToDelete)
        {
            _dependencies.Remove(taskToDelete);
        }

        public override string ToString()
        {
            var ids = from dependency in _dependencies select dependency.ID;
            string dependencyIds = String.Join(", ", ids);
            return $"ID: {ID} Time: ({_timeToCompletion})\n\tDependencies: {dependencyIds}";
        }
    }
}
