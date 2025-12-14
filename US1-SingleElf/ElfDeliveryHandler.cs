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

        public Sleigh Sleigh = new Sleigh();

        /// <summary>
        /// Organize Delivery of the Undelivered presents
        /// </summary>
        /// <param name="presentLock"></param>
        /// <param name="elvesLock"></param>
        /// <param name="elves">Current Elf Queue</param>
        /// <param name="undeliveredPresents">list of undelivered Presents</param>
        /// <returns>Async Task</returns>
        public async Task DeliverPresents(object elvesLock, Queue<Elf> elves, ConcurrentQueue<Present> undeliveredPresents, string[] naughtyList)
        {
            List<Task> taskList = new List<Task>();
            Dictionary<string, List<Present>> groupedPresentsToDeliver = new Dictionary<string, List<Present>>();
            // Gather 1 present per elf from the undelivered Presents.
            for (int i = 0; i < elves.Count; i++)
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
            // Assign each elf a Delivery task
            foreach (KeyValuePair<string, List<Present>> kvp in groupedPresentsToDeliver)
            {
                // Assign Family presents to deliver to sleigh at same time
                int delayAmount = numberRandomizer.Next(1, 1000);
                foreach (Present present in kvp.Value)
                {
                    Elf _nextElf = null;
                    lock (elvesLock)
                    {
                        if (elves.Any()) _nextElf = elves.Dequeue();
                    }
                    if (_nextElf == null)
                    {
                        break;
                    }
                    taskList.Add(Task.Run(async () =>
                    {
                        await Task.Delay(delayAmount);
                        bool success = await DeliverPresentTask(_nextElf, present, elvesLock, elves, Sleigh);
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
        /// Task assigned to each elf with details of the delivery
        /// </summary>
        /// <param name="elf">Elf assigned to</param>
        /// <param name="present">present to deliver</param>
        /// <param name="elvesLock"></param>
        /// <param name="elves">Queue to return to</param>
        /// <param name="sleigh">Santas Sleigh</param>
        /// <returns>success</returns>
        private async Task<bool> DeliverPresentTask(Elf elf, Present present, object elvesLock, Queue<Elf> elves, Sleigh sleigh)
        {
            bool success = false;
            if (elf != null)
            {
                success = await elf.DeliverPresent(present, sleigh);

                lock (elvesLock)
                {
                    elves.Enqueue(elf);
                }
            }
            return success;
        }
    }
}