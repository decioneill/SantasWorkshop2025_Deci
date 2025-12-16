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
    public class ToyMachineHandlerTests
    {
        [TestMethod()]
        public async Task BuildPresentsManyMachinesTest()
        {
            ConcurrentQueue<Present> presents = new ConcurrentQueue<Present>();
            ConcurrentQueue<ToyMachine> machines = new ConcurrentQueue<ToyMachine>();
            for (int i = 0; i < 100; i++)
            {
                machines.Enqueue(new ToyMachine());
            }
            ToyMachineHandler handler = new ToyMachineHandler() { ToyMachines = machines };
            List<string> familynames = new List<string>() { "Family01", "Family02", "Family03", "Family04" };
            int targetAmount = 1000;
            int startingAmount = 0;
            while (presents.Count < targetAmount)
            {
                startingAmount = await handler.BuildPresents(familynames, presents, targetAmount, startingAmount);
            }
            Assert.IsTrue(presents.Count == targetAmount, $"There are {presents.Count} Presents Built");
        }

        [TestMethod()]
        public async Task BuildPresentsOneMachineTest()
        {
            ConcurrentQueue<Present> presents = new ConcurrentQueue<Present>();
            ConcurrentQueue<ToyMachine> machines = new ConcurrentQueue<ToyMachine>(new ToyMachine[] { new ToyMachine() });
            ToyMachineHandler handler = new ToyMachineHandler() { ToyMachines = machines };
            List<string> familynames = new List<string>() { "Family01", "Family02" };
            int targetAmount = 10;
            int startingAmount = 0;
            while (presents.Count < targetAmount)
            {
                startingAmount = await handler.BuildPresents(familynames, presents, targetAmount, startingAmount);
            }
            Assert.IsTrue(presents.Count == targetAmount, $"There are {presents.Count} Presents Built");
        }
    }
}