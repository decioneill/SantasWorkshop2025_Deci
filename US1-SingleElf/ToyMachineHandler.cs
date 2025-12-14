using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace US1_SingleElf
{
    /// <summary>
    /// Handle the operations of the Toy Machines.
    /// </summary>
    public class ToyMachineHandler
    {
        static Random _random = new Random();

        /// <summary>
        /// Assign Tasks to each Toy Machine to create a present.
        /// </summary>
        /// <param name="familyNames">List of Family Names</param>
        /// <param name="presentLock"></param>
        /// <param name="machinesLock"></param>
        /// <param name="toyMachines">Machines in operation</param>
        /// <param name="undeliveredPresents">Presents not yet delivered to sleigh</param>
        /// <param name="expectedTotal">Expected Final Total of Presents</param>
        /// <param name="total">Current Total of Presents</param>
        /// <returns>number of presents created</returns>
        public async Task<int> BuildPresents(List<string> familyNames, object presentLock, object machinesLock, Queue<ToyMachine> toyMachines, List<Present> undeliveredPresents, int expectedTotal, int total)
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
                    CreatePresentTask(familyNames[_random.Next(0,familyNames.Count-1)], presentLock, ++total, _nextMachine, undeliveredPresents); 
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

        /// <summary>
        /// Task assigned to each Machine to build a new Present
        /// </summary>
        /// <param name="familyName">Family it is to go to</param>
        /// <param name="presentLock"></param>
        /// <param name="total">Used for Id</param>
        /// <param name="machine">Machine assigned task</param>
        /// <param name="undeliveredPresents">List to add new present into</param>
        private void CreatePresentTask(string familyName, object presentLock, int total, ToyMachine machine, List<Present> undeliveredPresents)
        {
            Present _nextPresent = machine.MakePresent($"{familyName} Family, ID:{total}", familyName);
            lock (presentLock)
            {
                undeliveredPresents.Add(_nextPresent);
            }
        }
    }
}