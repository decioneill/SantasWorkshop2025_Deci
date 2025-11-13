using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace US1_SingleElf
{
    internal class Program
    {
        static Sleigh _sleigh = new Sleigh();
        static Random _random = new Random();
        static Queue<ToyMachine> _toyMachines;
        static Queue<Elf> _elves;
        static Queue<Present> UndeliveredPresents = new Queue<Present>();
        static int _expectedTotal = 0;
        static int _total = 0;
        static object _machinesLock = new object();
        static object _elvesLock = new object();
        static object _presentLock = new object();

        static async Task Main(string[] args)
        {

            _toyMachines = RandomQueueAmount<ToyMachine>(_random.Next(1, 10000), "ToyMachines Active");
            _elves = RandomQueueAmount<Elf>(_random.Next(1, 10000), "Elves on Duty");
            foreach (Elf elf in _elves)
            {
                elf.sleigh = _sleigh;
            }

            while (true)
            {
                if (_total >= _expectedTotal && !UndeliveredPresents.Any())
                {
                    if (_total > 0)
                    {
                        Console.WriteLine($"All {_total} Presents packed and sent. Merry Christmas!");
                    }
                    int.TryParse(Console.ReadLine(), out _expectedTotal);
                    Console.WriteLine($"Total Expected Presents: {_expectedTotal}");
                    Console.WriteLine($"Current Total Presents: {_total}");
                }
                await BuildPresents();
                await DeliverPresents();
            }
        }

        private static async Task DeliverPresents()
        {
            List<Task> taskList = new List<Task>();
            List<Present> toBeDelivered = new List<Present>();
            lock (_presentLock)
            {
                for (int i = 0; i < _elves.Count; i++)
                {
                    if (UndeliveredPresents.Any()) 
                        toBeDelivered.Add(UndeliveredPresents.Dequeue());
                }
            }
            for (int i = 0; i < toBeDelivered.Count; i++)
            {
                Present present = toBeDelivered[i];
                Elf _nextElf = null;
                lock (_elvesLock)
                {
                    if (_elves.Any()) _nextElf = _elves.Dequeue();
                }
                if (_nextElf == null)
                {
                    break;
                }
                taskList.Add(Task.Run(async () =>
                {
                    await DeliverPresentTask(_nextElf, present);
                }));
            }
            if (taskList.Any())
            {
                Task allDeliveryTasks = Task.WhenAll(taskList.ToArray());
                await allDeliveryTasks;
            }
        }

        private static async Task BuildPresents()
        {
            List<Task> taskList = new List<Task>();

            for (int i = _expectedTotal - _total; i > 0; i--)
            {
                ToyMachine _nextMachine = null;
                lock (_machinesLock)
                {
                    if (_toyMachines.Any()) _nextMachine = _toyMachines.Dequeue();
                }
                if (_nextMachine == null)
                {
                    break;
                }
                taskList.Add(Task.Run(() =>
                {
                    CreatePresentTask(++_total, _nextMachine);
                }));
            }
            if (taskList.Any())
            {
                Task allPresentTasks = Task.WhenAll(taskList.ToArray());
                await allPresentTasks;
            }
        }


        private static void CreatePresentTask(int _total, ToyMachine _nextMachine)
        {
            Present _nextPresent = _nextMachine.MakePresent($"Present-{_total}");
            lock (_machinesLock)
            {
                _toyMachines.Enqueue(_nextMachine);
            }
            lock (_machinesLock)
            {
                UndeliveredPresents.Enqueue(_nextPresent);
            }
        }

        private static async Task DeliverPresentTask(Elf _nextElf, Present _nextPresent)
        {
            if (_nextElf != null)
            {
                if (!await _nextElf.DeliverPresent(_nextPresent))
                {
                    UndeliveredPresents.Enqueue(_nextPresent);
                    Console.WriteLine("Present \"{name}\" not delivered.");
                }

                lock (_elvesLock)
                {
                    _elves.Enqueue(_nextElf);
                }
            }
        }

        private static Queue<T> RandomQueueAmount<T>(int number, string name)
            where T : NamedObject
        {
            Queue<T> _queue = new Queue<T>(number);
            for (int i = number; i > 0; i--)
            {
                T newItem = Activator.CreateInstance<T>();
                newItem.Name = $"{name}-{i}";
                _queue.Enqueue(newItem);
            }
            Console.WriteLine($"{name}: {number}");
            return _queue;
        }
    }
}
