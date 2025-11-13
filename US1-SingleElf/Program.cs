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

        static async Task Main(string[] args)
        {
            int _expectedTotal = 0;
            Sleigh _sleigh = new Sleigh();
            Random _random = new Random();
            int _total = 0;
            Queue<ToyMachine> _toyMachines;
            Queue<Elf> _elves;

            List<Present> UndeliveredPresents = new List<Present>();

            _toyMachines = RandomQueueAmount<ToyMachine>(_random.Next(1, 100), "ToyMachines");
            _elves = RandomQueueAmount<Elf>(_random.Next(1, 100), "Elves");
            foreach (Elf elf in _elves)
            {
                elf.sleigh = _sleigh;
            }

            while (true)
            {
                if (_total >= _expectedTotal) 
                    int.TryParse(Console.ReadLine(), out _expectedTotal);
                List<Task> taskList = new List<Task>();
                if(_expectedTotal > _total)
                {
                    for(int i = _expectedTotal - _total; i > 0; i--)
                    {
                        taskList.Add(Task.Run(async () =>
                        {
                            await CreateTaskForPresent(_total++, _toyMachines, _elves, UndeliveredPresents);
                        }));
                    }
                    await Task.WhenAll(taskList.ToArray());
                }
            }
        }

        private static async Task CreateTaskForPresent(int _total, Queue<ToyMachine> _toyMachines, Queue<Elf> _elves, List<Present> UndeliveredPresents)
        {
            ToyMachine _nextMachine = _toyMachines.Dequeue();
            Present _nextPresent = _nextMachine.MakePresent($"Present-{_total}");
            _toyMachines.Enqueue(_nextMachine);
            Elf _nextElf = _elves.Dequeue();
            if (_nextElf != null)
            {
                if (!await _nextElf.DeliverPresent(_nextPresent))
                {
                    UndeliveredPresents.Add(_nextPresent);
                    Console.WriteLine("Present \"{name}\" not delivered.");
                }
                _elves.Enqueue(_nextElf);
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
