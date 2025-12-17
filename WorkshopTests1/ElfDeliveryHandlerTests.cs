using Microsoft.VisualStudio.TestTools.UnitTesting;
using SantasWorkshop2025;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SantasWorkshop2025.Tests
{
    [TestClass()]
    public class ElfDeliveryHandlerTests
    {
        [TestMethod()]
        public async Task DeliverPresentsTest()
        {
            ISleigh sleigh = new Sleigh();
            ConcurrentQueue<Elf> elves = new ConcurrentQueue<Elf>();
            elves.Enqueue(new Elf());
            ElfDeliveryHandler handler = new ElfDeliveryHandler() { Elves = elves, Sleigh = sleigh };

            ConcurrentQueue<Present> presents = new ConcurrentQueue<Present>(new Present[] { new Present() { Family = "Family01" } });
            string[] naughtyList = new string[] { "Family02" };
            await handler.DeliverPresents(presents, naughtyList);

            Assert.IsTrue(sleigh.RemoveFamilyPresents("Family01") == 1);
        }

        /// <summary>
        /// Test Large number of Presents delivered by low number of elves
        /// </summary>
        /// <returns>Result</returns>
        [TestMethod()]
        public async Task DeliverPresentsManyTest()
        {
            ISleigh sleigh = new Sleigh();
            ConcurrentQueue<Elf> elves = new ConcurrentQueue<Elf>();
            for (int i = 0; i < 10; i++)
            {
                elves.Enqueue(new Elf() { Name = $"Elf{i}" });
            }
            ElfDeliveryHandler handler = new ElfDeliveryHandler() { Elves = elves, Sleigh = sleigh };
            Present[] presentArray = new Present[100];
            for (int i = 0; i < 100; i++)
            {
                presentArray[i] = new Present() { Family = $"Family{i}", CreatedByMachine = $"M{i}", Name = $"P{i}" };
            }
            ConcurrentQueue<Present> presents = new ConcurrentQueue<Present>(presentArray);
            string[] naughtyList = new string[] { "Family1", "Family99" };
            while (presents.Any())
            {
                await handler.DeliverPresents(presents, naughtyList);
            }

            Assert.IsTrue(presents.Count == 0);
        }

        /// <summary>
        /// Test that all presents are not delivered when Family is naughty
        /// </summary>
        /// <returns></returns>
        [TestMethod()]
        public async Task DeliverPresentsTestFamilyFail()
        {
            ISleigh sleigh = new Sleigh();
            ConcurrentQueue<Elf> elves = new ConcurrentQueue<Elf>();
            for (int i = 0; i < 100; i++)
            {
                elves.Enqueue(new Elf() { Name = $"Elf{i}" });
            }
            ElfDeliveryHandler handler = new ElfDeliveryHandler() { Elves = elves, Sleigh = sleigh };
            Present[] presentArray = new Present[100];
            for (int i = 0; i < 100; i++)
            {
                presentArray[i] = new Present() { Family = $"Family1", CreatedByMachine = $"M{i}", Name = $"P{i}" };
            }
            ConcurrentQueue<Present> presents = new ConcurrentQueue<Present>(presentArray);
            string[] naughtyList = new string[] { "Family1" };
            await handler.DeliverPresents(presents, naughtyList);

            Assert.IsTrue(presents.Count == 0 && sleigh.RemoveFamilyPresents("Family1") == 0);
        }

        /// <summary>
        /// Test Presents are Correctly Sorted into their Families
        /// </summary>
        /// <returns>Result</returns>
        [TestMethod()]
        public async Task DeliverPresentsFamilySortedTest()
        {
            ISleigh sleigh = new Sleigh();
            ConcurrentQueue<Elf> elves = new ConcurrentQueue<Elf>();
            for (int i = 0; i < 20; i++)
            {
                elves.Enqueue(new Elf() { Name = $"Elf{i}" });
            }
            ElfDeliveryHandler handler = new ElfDeliveryHandler() { Elves = elves, Sleigh = sleigh };
            Present[] presentArray = new Present[20];
            for (int i = 0; i < 20; i++)
            {
                bool isEven = i % 2 == 0 ? true : false;
                string familyName = isEven ? "Family1" : "Family2";
                presentArray[i] = new Present() { Family = familyName, CreatedByMachine = $"M{i}", Name = $"P{i}"};
            }
            ConcurrentQueue<Present> presents = new ConcurrentQueue<Present>(presentArray);
            string[] naughtyList = new string[0];
            await handler.DeliverPresents(presents, naughtyList);

            int removedFamily1Amount = sleigh.RemoveFamilyPresents("Family1");
            int removedFamily2Amount = sleigh.RemoveFamilyPresents("Family2");
            bool isSorted = removedFamily1Amount == 10 && removedFamily2Amount == 10;
            

            Assert.IsTrue(isSorted, $"Family 1 removed {removedFamily1Amount}, Family 2 removed {removedFamily2Amount}.");
        }
    }
}