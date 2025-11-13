using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace US1_SingleElf
{
    public class ElfDeliveryHandler
    {
        public async Task DeliverPresents(object presentLock, object elvesLock, Queue<Elf> elves, Queue<Present> undeliveredPresents)
        {
            List<Task> taskList = new List<Task>();
            List<Present> toBeDelivered = new List<Present>();
            lock (presentLock)
            {
                for (int i = 0; i < elves.Count; i++)
                {
                    if (undeliveredPresents.Any())
                        toBeDelivered.Add(undeliveredPresents.Dequeue());
                }
            }
            for (int i = 0; i < toBeDelivered.Count; i++)
            {
                Present present = toBeDelivered[i];
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
                    bool success = await DeliverPresentTask(_nextElf, present, elvesLock, elves);
                    if (!success)
                    {
                        undeliveredPresents.Enqueue(present);
                        Console.WriteLine($"Present \"{present.Name}\" not delivered.");
                    }
                }));
            }
            if (taskList.Any())
            {
                Task allDeliveryTasks = Task.WhenAll(taskList.ToArray());
                await allDeliveryTasks;
            }
        }

        private async Task<bool> DeliverPresentTask(Elf _nextElf, Present _nextPresent, object elvesLock, Queue<Elf> elves)
        {
            bool success = false;
            if (_nextElf != null)
            {
                success = await _nextElf.DeliverPresent(_nextPresent);

                lock (elvesLock)
                {
                    elves.Enqueue(_nextElf);
                }
            }
            return success;
        }
    }
}