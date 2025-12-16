using Microsoft.VisualStudio.TestTools.UnitTesting;
using US1_SingleElf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace US1_SingleElf.Tests
{
    [TestClass]
    public class ToyMachineHandlerTests
    {
        [TestMethod()]
        public async Task BuildPresentsManyMachinesTest()
        {
            ConcurrentQueue<Present> presents = new ConcurrentQueue<Present>();
            ConcurrentQueue<ToyMachine> machines = new ConcurrentQueue<ToyMachine>();
            for(int i = 0; i < 10; i++)
            {
                machines.Enqueue(new ToyMachine());
            }
            ToyMachineHandler handler = new ToyMachineHandler() { ToyMachines = machines };
            List<string> familynames = new List<string>() { "Family01", "Family02", "Family03", "Family04" };
            int targetAmount = 100;
            int startingAmount = 0;
            while (presents.Count < targetAmount)
            {
                await handler.BuildPresents(familynames, presents, targetAmount, startingAmount);
            }
            Assert.IsTrue(presents.Count == targetAmount);
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
                await handler.BuildPresents(familynames, presents, targetAmount, startingAmount);
            }
            Assert.IsTrue(presents.Count == targetAmount);
        }
    }
}