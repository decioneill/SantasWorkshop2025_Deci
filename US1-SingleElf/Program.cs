using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US1_SingleElf
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ToyMachine _toyMachine = new ToyMachine();
            Elf singleElf = new Elf();
            while (true)
            {
                string name = Console.ReadLine();
                await Task.Run(async () =>
                {
                    Present present = _toyMachine.MakePresent(name);
                    bool isDelivered = await singleElf.DeliverPresent(present);
                    if (!isDelivered)
                    {
                        Console.WriteLine("Present \"{name}\" not delivered.");
                    }
                });
            }
        }
    }
}
