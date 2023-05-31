
using System.Collections.Generic;
using System;

namespace ProjectManagementSystem
{
    internal class Task
    {
        public string ID { get; private set; }
        public uint TimeToCompletion { get; private set; }
        public HashSet<Task>? Dependencies { get; private set; }

        public uint NStart = 0;
        public uint NFinish = 0;

        public bool HasDependencies
        {
            get
            {
                return Dependencies.Count > 0;
            }
        }

        public Task(string id)
        {
            ID = id;
            TimeToCompletion = uint.MaxValue;
        }
        public Task(string id, uint timeToCompletion, HashSet<Task> dependencies)
        {
            ID = id;
            TimeToCompletion = timeToCompletion;
            Dependencies = dependencies;
        }
        public void Update(uint timeToCompletion, HashSet<Task> dependencies)
        {
            TimeToCompletion = timeToCompletion;
            Dependencies = dependencies;
        }
        public void DeleteDependency(Task taskToDelete)
        {
            Dependencies.Remove(taskToDelete);
        }
        public void ChangeTime(uint newTime)
        {
            TimeToCompletion = newTime;
        }
        public string FormatFileString()
        {
            return $"{ID}, {TimeToCompletion}, {String.Join(", ", Dependencies.Select(x => x.ID))}";
        }
        public string FormatDependenciesString()
        {
            return String.Join(',', Dependencies.Select(x => x.ID));
        }
        public void ClearNetwork()
        {
            NStart = 0;
            NFinish = 0;
        }

        public override string ToString()
        {
            return $"| ID: {ID} Time: ({TimeToCompletion})\n|\tDependencies: {String.Join(", ", Dependencies.Select(x => x.ID))}";
        }
    }
}
