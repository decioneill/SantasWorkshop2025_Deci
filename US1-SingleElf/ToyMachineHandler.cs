using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace US1_SingleElf
{
    public class ToyMachineHandler
    {
        static Random _random = new Random();

        public async Task<int> BuildPresents(string[] familyNames, object presentLock, object machinesLock, Queue<ToyMachine> toyMachines, Queue<Present> undeliveredPresents, int expectedTotal, int total)
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
                    CreatePresentTask(familyNames[_random.Next(0,familyNames.Length-1)], presentLock, ++total, _nextMachine, undeliveredPresents); 
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


        private void CreatePresentTask(string familyName, object presentLock, int total, ToyMachine nextMachine, Queue<Present> undeliveredPresents)
        {
            Present _nextPresent = nextMachine.MakePresent($"{familyName} Family, ID:{total}", familyName);
            lock (presentLock)
            {
                undeliveredPresents.Enqueue(_nextPresent);
            }
        }
    }
}