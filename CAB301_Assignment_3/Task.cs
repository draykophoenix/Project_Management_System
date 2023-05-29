using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override string ToString()
        {
            return $"{ID} ({_timeToCompletion})";
        }

    }
}
