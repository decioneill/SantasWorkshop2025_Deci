using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace US1_SingleElf
{
    public class ToyMachineHandler
    {
        public async Task<int> BuildPresents(object presentLock, object machinesLock, Queue<ToyMachine> toyMachines, Queue<Present> undeliveredPresents, int expectedTotal, int total)
        {
            List<Task> taskList = new List<Task>();

            for (int i = expectedTotal - total; i > 0; i--)
            {
                ToyMachine _nextMachine = null;
                lock (machinesLock)
                {
                    if (toyMachines.Any()) _nextMachine = toyMachines.Dequeue();
                }
                if (_nextMachine == null)
                {
                    break;
                }
                taskList.Add(Task.Run(() =>
                {
                    CreatePresentTask(presentLock, ++total, _nextMachine, undeliveredPresents); 
                    lock (machinesLock)
                    {
                        toyMachines.Enqueue(_nextMachine);
                    }
                }));
            }
            if (taskList.Any())
            {
                Task allPresentTasks = Task.WhenAll(taskList.ToArray());
                await allPresentTasks;
            }
            return total;
        }


        private void CreatePresentTask(object presentLock, int total, ToyMachine nextMachine, Queue<Present> undeliveredPresents)
        {
            Present _nextPresent = nextMachine.MakePresent($"Present-{total}");
            lock (presentLock)
            {
                undeliveredPresents.Enqueue(_nextPresent);
            }
        }
    }
}