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
        static int _expectedTotal = 0;
        static int _totalPresents = 0;

        static ConcurrentQueue<Present> _undeliveredPresents = new ConcurrentQueue<Present>();
        static List<string> _familyNames = new List<string>();

        static ElfDeliveryHandler _elvesDeliveryHandler = new ElfDeliveryHandler();
        static ToyMachineHandler _toyMakerHandler = new ToyMachineHandler();
        static MrsClaus _mrsClaus = new MrsClaus();

        /// <summary>
        /// Tuples of Family Name and amount of presents removed
        /// </summary>
        static List<Tuple<string, int>> _naughtyList = new List<Tuple<string, int>>();

        /// <summary>
        /// Main Program
        /// </summary>
        /// <param name="args">args</param>
        /// <returns>completed</returns>
        static async Task Main(string[] args)
        {
            InitializeSantasWorkshop();
            await RunSantasWorkshopOperation();
        }

        /// <summary>
        /// Run Santas Workshop Operation
        /// </summary>
        /// <returns>Completed</returns>
        private static async Task RunSantasWorkshopOperation()
        {
            // While running
            while (true)
            {
                if (_totalPresents >= _expectedTotal && !_undeliveredPresents.Any())
                {
                    // While no order, Show all presents delivered (or removed for being naughty)
                    ShowRemovedFamilies();

                    // Prompt User to give an order amount
                    Console.WriteLine("Place an order amount?");
                    int.TryParse(Console.ReadLine(), out int _newOrderAmount);
                    _expectedTotal += _newOrderAmount;

                    Console.WriteLine($"Total Expected Presents: {_expectedTotal}");
                    Console.WriteLine($"Current Total Presents: {_totalPresents}");

                }
                // Making a List
                CheckNaughtyList(_familyNames);

                // Build Presents for the Families
                _totalPresents = await _toyMakerHandler.BuildPresents(_familyNames, _undeliveredPresents, _expectedTotal, _totalPresents);

                // Have Elves deliver the presents
                await _elvesDeliveryHandler.DeliverPresents(_undeliveredPresents, _naughtyList.Select(x => x.Item1).ToArray());

                //Checking it Twice
                CheckNaughtyList(_familyNames);
            }
        }

        /// <summary>
        /// Display in Console Names of Families Removed and Presents removed from Sleigh / delivery.
        /// </summary>
        private static void ShowRemovedFamilies()
        {
            if (_totalPresents > 0)
            {
                if (_naughtyList.Any())
                {
                    Console.WriteLine($"\nFamilies Removed:");
                    foreach (Tuple<string, int> tuple in _naughtyList)
                    {
                        Console.WriteLine($"   Family {tuple.Item1}, {tuple.Item2} presents removed.");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine($"All {_totalPresents} Presents packed and sent. Merry Christmas!");
            }
        }

        /// <summary>
        /// Setup Initial values at Santas Workshop
        /// </summary>
        private static void InitializeSantasWorkshop()
        {
            _familyNames = new StreamReader("..\\..\\surnames.txt").ReadToEnd().Split(',').ToList();
            _toyMakerHandler.ToyMachines = RandomQueueAmount<ToyMachine>(_random.Next(1, 10000), "Machine");
            _elvesDeliveryHandler.Elves = RandomQueueAmount<Elf>(_random.Next(1, 10000), "Elf");
        }

        /// <summary>
        /// Check with Mrs Claus on any familys that have been naughty and are not to receive gifts
        /// </summary>
        /// <param name="_familyNames">Current Families</param>
        /// <param name="naughtyFamily">A new naughty Family</param>
        private static void CheckNaughtyList(List<string> _familyNames)
        {
            // Check there is a family to remove
            string naughtyFamily = _mrsClaus.DoubleCheckNaughtyList(_familyNames);
            if (naughtyFamily != null)
            {
                // Remove from Family Names to avoid new gifts
                _familyNames.Remove(naughtyFamily);
                // unload their gifts
                if (_elvesDeliveryHandler.Sleigh.PackedPresents.ContainsKey(naughtyFamily))
                {
                    _elvesDeliveryHandler.Sleigh.PackedPresents.TryRemove(naughtyFamily, out ConcurrentQueue<Present> removedPresents);
                    _totalPresents -= removedPresents.Count;
                    _naughtyList.Add(new Tuple<string, int>(naughtyFamily, removedPresents.Count));
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
