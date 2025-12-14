using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace US1_SingleElf
{
    internal class Program
    {
        static Random _random = new Random();
        static ConcurrentQueue<ToyMachine> _toyMachines;
        static ConcurrentQueue<Elf> _elves;
        static ConcurrentQueue<Present> UndeliveredPresents = new ConcurrentQueue<Present>();
        static int _expectedTotal = 0;
        static int _totalPresents = 0;

        static ElfDeliveryHandler _elvesDeliveryHandler = new ElfDeliveryHandler();
        static ToyMachineHandler _toyMakerHandler = new ToyMachineHandler();

        static MrsClaus mrsClaus = new MrsClaus();
        static List<Tuple<string, int>> naughtyList = new List<Tuple<string, int>>();

        static async Task Main(string[] args)
        {

            InitializeSantasWorkshop();
            await RunSantasWorkshopOperation();
        }

        private static async Task RunSantasWorkshopOperation()
        {
            // Get All Families
            List<string> _familyNames = new StreamReader("..\\..\\surnames.txt").ReadToEnd().Split(',').ToList();
            string naughtyFamily = null;

            // While running
            while (true)
            {
                if (_totalPresents >= _expectedTotal && !UndeliveredPresents.Any())
                {
                    // While no order, Show all presents delivered (or removed for being naughty)
                    if (_totalPresents > 0)
                    {
                        if (naughtyList.Any())
                        {
                            Console.WriteLine($"\nFamilies Removed:");
                            foreach (Tuple<string, int> tuple in naughtyList)
                                Console.WriteLine($"   Family {tuple.Item1}, {tuple.Item2} presents removed.");
                            Console.WriteLine();
                        }
                        Console.WriteLine($"All {_totalPresents} Presents packed and sent. Merry Christmas!");
                    }
                    // Prompt User to give an order amount
                    Console.WriteLine("Place an order amount?");
                    int.TryParse(Console.ReadLine(), out int _newOrderAmount);
                    _expectedTotal += _newOrderAmount;
                    Console.WriteLine($"Total Expected Presents: {_expectedTotal}");
                    Console.WriteLine($"Current Total Presents: {_totalPresents}");

                }
                // Making a List
                CheckNaughtyList(_familyNames, naughtyFamily);

                // Build Presents for the Families
                _totalPresents = await _toyMakerHandler.BuildPresents(_familyNames, _toyMachines, UndeliveredPresents, _expectedTotal, _totalPresents);
                // Have Elves deliver the presents
                await _elvesDeliveryHandler.DeliverPresents(_elves, UndeliveredPresents, naughtyList.Select(x => x.Item1).ToArray());

                //Checking it Twice
                CheckNaughtyList(_familyNames, naughtyFamily);
            }
        }

        /// <summary>
        /// Setup Initial values at Santas Workshop
        /// </summary>
        private static void InitializeSantasWorkshop()
        {
            _toyMachines = RandomQueueAmount<ToyMachine>(_random.Next(1, 10000), "Machine");
            _elves = RandomQueueAmount<Elf>(_random.Next(1, 10000), "Elf");
            Console.WriteLine($"There are {_toyMachines.Count} Toy Machines Operating.");
            Console.WriteLine($"There are {_elves.Count} Elves working.");
        }

        /// <summary>
        /// Check with Mrs Claus on any familys that have been naughty and are not to receive gifts
        /// </summary>
        /// <param name="_familyNames">Current Families</param>
        /// <param name="naughtyFamily">A new naughty Family</param>
        private static void CheckNaughtyList(List<string> _familyNames, string naughtyFamily)
        {
            // Check there is a family to remove
            naughtyFamily = mrsClaus.DoubleCheckNaughtyList(_familyNames);
            if (naughtyFamily != null)
            {
                // Remove from Family Names to avoid new gifts
                _familyNames.Remove(naughtyFamily);
                // unload their gifts
                if (_elvesDeliveryHandler.Sleigh.PackedPresents.ContainsKey(naughtyFamily))
                {
                    _elvesDeliveryHandler.Sleigh.PackedPresents.TryRemove(naughtyFamily, out ConcurrentQueue<Present> removedPresents);
                    _totalPresents -= removedPresents.Count;
                    naughtyList.Add(new Tuple<string, int>(naughtyFamily, removedPresents.Count));
                }
            }
        }


        /// <summary>
        /// Generate Random Number of Instances for NamedObjects
        /// </summary>
        /// <typeparam name="T">NamedObject</typeparam>
        /// <param name="number">Number to Instance</param>
        /// <param name="name">String Name for prefixing to its Name value</param>
        /// <returns>Queue of NamedObjects of number length</returns>
        private static ConcurrentQueue<T> RandomQueueAmount<T>(int number, string name)
            where T : NamedObject
        {
            ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
            for (int i = number; i > 0; i--)
            {
                T newItem = Activator.CreateInstance<T>();
                newItem.Name = $"{name}-{i}";
                _queue.Enqueue(newItem);
            }
            return _queue;
        }
    }
}
