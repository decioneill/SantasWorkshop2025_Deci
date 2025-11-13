using System;
using System.Threading.Tasks;

namespace US1_SingleElf
{
    public class Elf
    {
        Sleigh sleigh { get; set; } = new Sleigh();

        public async Task<bool> DeliverPresent(Present present)
        {
            if (sleigh != null)
            {
                await sleigh.Pack(present);
                return true;
            }
            return false;
        }
    }
}