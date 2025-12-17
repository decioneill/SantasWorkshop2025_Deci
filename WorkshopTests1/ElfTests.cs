using Microsoft.VisualStudio.TestTools.UnitTesting;
using SantasWorkshop2025;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SantasWorkshop2025.Tests
{
    [TestClass()]
    public class ElfTests
    {
        [TestMethod()]
        public async Task DeliverPresentTest()
        {
            Present present = new Present() { CreatedByMachine = "M1", Family = "F1", Name = "P1" };
            ISleigh sleigh = new Sleigh();
            Elf elf = new Elf()
            {
                Name = "Elf01"
            };

            Assert.IsTrue(await elf.DeliverPresent(present, sleigh));
        }
    }
}