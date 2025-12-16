using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace US1_SingleElf
{
    public class ElfDeliveryHandler
    {
        Random numberRandomizer = new Random();

        /// <summary>
        /// Current Working Elves
        /// </summary>
        public ConcurrentQueue<Elf> Elves
        {
            get => _elves;
            set
            {
                _elves = value;
                Console.WriteLine($"There are {_elves.Count} Elves working.");
            }
        }

        public Sleigh Sleigh = new Sleigh();

        private ConcurrentQueue<Elf> _elves = new ConcurrentQueue<Elf>();

        /// <summary>
        /// Organize Delivery of the Undelivered presents
        /// </summary>
        /// <param name="_elves">Current Elf Queue</param>
        /// <param name="undeliveredPresents">list of undelivered Presents</param>
        /// <returns>Async Task</returns>
        public async Task DeliverPresents(ConcurrentQueue<Present> undeliveredPresents, string[] naughtyList)
        {
            List<Task> taskList = new List<Task>();
            Dictionary<string, List<Present>> groupedPresentsToDeliver = new Dictionary<string, List<Present>>();

            // Gather 1 present per elf from the undelivered Presents.
            DequeueAndSortByFamily(undeliveredPresents, naughtyList, groupedPresentsToDeliver);

            // Assign each elf a Delivery task
            foreach (KeyValuePair<string, List<Present>> kvp in groupedPresentsToDeliver)
            {
                // Assign Family presents to deliver to sleigh at same time
                int delayAmount = numberRandomizer.Next(1, 1000);
                foreach (Present present in kvp.Value)
                {
                    Elf _nextElf = null;
                    if (!_elves.Any() || !_elves.TryDequeue(out _nextElf)) continue;

                    taskList.Add(Task.Run(async () =>
                    {
                        await Task.Delay(delayAmount);
                        bool success = await DeliverPresentTask(_nextElf, present, _elves, Sleigh);
                        if (!success)
                        {
                            undeliveredPresents.Enqueue(present);
                            Console.WriteLine($"Present \"{present.Name}\" not delivered.");
                        }
                    }));
                }
            }
            // Send the elves
            if (taskList.Any())
            {
                Task allDeliveryTasks = Task.WhenAll(taskList.ToArray());
                await allDeliveryTasks;
            }
        }

        /// <summary>
        /// Dequeue one Present per Elf, and sort into family groups
        /// </summary>
        /// <param name="undeliveredPresents">Full List of Undelivered presents</param>
        /// <param name="naughtyList">Naughty Families</param>
        /// <param name="groupedPresentsToDeliver">Dictionary of Presents By Family.</param>
        private void DequeueAndSortByFamily(ConcurrentQueue<Present> undeliveredPresents, string[] naughtyList, Dictionary<string, List<Present>> groupedPresentsToDeliver)
        {
            for (int i = 0; i < _elves.Count; i++)
            {
                if (undeliveredPresents.Any())
                {
                    // Sort these gathered presents into families.
                    undeliveredPresents.TryDequeue(out Present nextPresent);
                    if (naughtyList.Contains(nextPresent.Family))
                    {
                        i--;
                        continue;
                    }
                    if (groupedPresentsToDeliver.ContainsKey(nextPresent.Family))
                    {
                        groupedPresentsToDeliver[nextPresent.Family].Add(nextPresent);
                    }
                    else
                    {
                        groupedPresentsToDeliver.Add(nextPresent.Family, new List<Present>() { nextPresent });
                    }
                }
            }
        }

        /// <summary>
        /// Task assigned to each elf with details of the delivery
        /// </summary>
        /// <param name="elf">Elf assigned to</param>
        /// <param name="present">present to deliver</param>
        /// <param name="elves">Queue to return to</param>
        /// <param name="sleigh">Santas Sleigh</param>
        /// <returns>success</returns>
        private async Task<bool> DeliverPresentTask(Elf elf, Present present, ConcurrentQueue<Elf> elves, Sleigh sleigh)
        {
            bool success = false;
            if (elf != null)
            {
                success = await elf.DeliverPresent(present, sleigh);
                elves.Enqueue(elf);
            }
            return success;
        }
    }
}