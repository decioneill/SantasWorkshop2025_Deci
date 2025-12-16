using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantasWorkshop2025
{
    /// <summary>
    /// Handle the operations of the Toy Machines.
    /// </summary>
    public class ToyMachineHandler
    {
        static Random _random = new Random();
        private ConcurrentQueue<ToyMachine> _toyMachines;

        /// <summary>
        /// Current Operational Toy Machines
        /// </summary>
        public ConcurrentQueue<ToyMachine> ToyMachines
        {
            get => _toyMachines;
            set
            {
                _toyMachines = value;
                Console.WriteLine($"There are {_toyMachines.Count} Toy Machines Operating.");
            }
        }

        /// <summary>
        /// Assign Tasks to each Toy Machine to create a present.
        /// </summary>
        /// <param name="familyNames">List of Family Names</param>
        /// <param name="undeliveredPresents">Presents not yet delivered to sleigh</param>
        /// <param name="expectedTotal">Expected Final Total of Presents</param>
        /// <param name="total">Current Total of Presents</param>
        /// <returns>number of presents created</returns>
        public async Task<int> BuildPresents(List<string> familyNames, ConcurrentQueue<Present> undeliveredPresents, int expectedTotal, int total)
        {
            List<Task> taskList = new List<Task>();

            for (int i = expectedTotal - total; i > 0; i--)
            {
                ToyMachine _nextMachine = null;
                if (!_toyMachines.Any() || !_toyMachines.TryDequeue(out _nextMachine)) continue;

                taskList.Add(Task.Run(() =>
                {
                    CreatePresentTask(familyNames[_random.Next(0, familyNames.Count - 1)], total, _nextMachine, undeliveredPresents);
                    _toyMachines.Enqueue(_nextMachine);
                }));
            }
            if (taskList.Any())
            {
                Task allPresentTasks = Task.WhenAll(taskList.ToArray());
                await allPresentTasks;
            }
            return total + taskList.Count(x => x.IsCompleted);
        }

        /// <summary>
        /// Task assigned to each Machine to build a new Present
        /// </summary>
        /// <param name="familyName">Family it is to go to</param>
        /// <param name="total">Used for Id</param>
        /// <param name="machine">Machine assigned task</param>
        /// <param name="undeliveredPresents">List to add new present into</param>
        private void CreatePresentTask(string familyName, int total, ToyMachine machine, ConcurrentQueue<Present> undeliveredPresents)
        {
            lock (undeliveredPresents)
            {
                Present _nextPresent = machine.MakePresent($"{familyName} Family, ID:{total}", familyName);
                undeliveredPresents.Enqueue(_nextPresent);
            }
        }
    }
}