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
        static List<Present> UndeliveredPresents = new List<Present>();
        static int _expectedTotal = 0;
        static int _total = 0;
        static object _machinesLock = new object();
        static object _elvesLock = new object();

        static async Task Main(string[] args)
        {

            _toyMachines = RandomQueueAmount<ToyMachine>(_random.Next(1, 10), "ToyMachines");
            _elves = RandomQueueAmount<Elf>(_random.Next(1, 10), "Elves");
            foreach (Elf elf in _elves)
            {
                elf.sleigh = _sleigh;
            }

            while (true)
            {
                if (_total >= _expectedTotal)
                {
                    int.TryParse(Console.ReadLine(), out _expectedTotal);
                    Console.WriteLine($"Total Expected Presents: {_expectedTotal}");
                    Console.WriteLine($"Current Total Presents: {_total}");
                }
                await BuildPresentsAndDeliver();
            }
        }

        private static async Task BuildPresentsAndDeliver()
        {
            List<Task> taskList = new List<Task>();

            for (int i = _expectedTotal - _total; i > 0; i--)
            {
                ToyMachine _nextMachine = null;
                Elf _nextElf = null;
                lock (_machinesLock)
                {
                    if (_toyMachines.Any()) _nextMachine = _toyMachines.Dequeue();
                }
                lock (_elvesLock)
                {
                    if (_elves.Any()) _nextElf = _elves.Dequeue();
                }
                if (_nextMachine == null || _nextElf == null)
                {
                    break;
                }
                taskList.Add(Task.Run(async () =>
                {
                    await CreatePresentTask(++_total, _nextMachine, _nextElf);
                }));
            }
            if (taskList.Any())
            {
                Task allPresentTasks = Task.WhenAll(taskList.ToArray());
                await allPresentTasks;
            }
            if (_total >= _expectedTotal) Console.WriteLine($"All {_total} Presents packed and sent. Merry Christmas!");
        }


        private static async Task CreatePresentTask(int _total, ToyMachine _nextMachine, Elf _nextElf)
        {
            Present _nextPresent = _nextMachine.MakePresent($"Present-{_total}");
            lock (_machinesLock)
            {
                _toyMachines.Enqueue(_nextMachine);
            }
            if (_nextElf != null)
            {
                if (!await _nextElf.DeliverPresent(_nextPresent))
                {
                    UndeliveredPresents.Add(_nextPresent);
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
            Console.WriteLine($"{name}; {number}");
            return _queue;
        }
    }
}
