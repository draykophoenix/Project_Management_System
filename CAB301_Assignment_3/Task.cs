
using System.Collections.Generic;
using System;

namespace ProjectManagementSystem
{
    internal class Task
    {
        public string ID { get; private set; }
        private uint _timeToCompletion;
        private HashSet<Task>? _dependencies;

        public bool HasDependencies
        {
            get
            {
                return _dependencies.Count > 0;
            }
        }

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
        public void ChangeTime(uint newTime)
        {
            _timeToCompletion = newTime;
        }
        public string FormatFileString()
        {
            var ids = from dependency in _dependencies select dependency.ID;
            return $"{ID}, {_timeToCompletion}, {String.Join(", ", ids)}";
        }

        public override string ToString()
        {
            var ids = from dependency in _dependencies select dependency.ID;
            return $"| ID: {ID} Time: ({_timeToCompletion})\n|\tDependencies: {String.Join(", ", ids)}";
        }
    }
}
