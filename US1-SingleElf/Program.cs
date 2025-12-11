using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        static ElfDeliveryHandler _elvesDeliveryHandler = new ElfDeliveryHandler();
        static ToyMachineHandler _toyMakerHandler = new ToyMachineHandler();

        static async Task Main(string[] args)
        {
            string[] _familyNames = new StreamReader("..\\..\\surnames.txt").ReadToEnd().Split(',');
            _toyMachines = RandomQueueAmount<ToyMachine>(_random.Next(1, 10000), "Machine");
            _elves = RandomQueueAmount<Elf>(_random.Next(1, 10000), "Elf");
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
                    Console.WriteLine("Place an order amount?");
                    int.TryParse(Console.ReadLine(), out int _newOrderAmount);
                    _expectedTotal += _newOrderAmount;
                    Console.WriteLine($"Total Expected Presents: {_expectedTotal}");
                    Console.WriteLine($"Current Total Presents: {_total}");
                }
                _total = await _toyMakerHandler.BuildPresents(_familyNames, _presentLock, _machinesLock, _toyMachines, UndeliveredPresents, _expectedTotal, _total);
                await _elvesDeliveryHandler.DeliverPresents(_presentLock, _elvesLock, _elves, UndeliveredPresents);
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
            Console.WriteLine($"{name} Queue: {number}");
            return _queue;
        }
    }
}
