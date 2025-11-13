using System;
using System.Threading.Tasks;

namespace US1_SingleElf
{
    public class Elf : NamedObject
    {
        public Elf() 
        { 
        }

        public string Name {  get; set; }

        public Sleigh sleigh { get; set; }

        public async Task<bool> DeliverPresent(Present present)
        {
            if (present == null) return false;
            present.DeliveredByElf = Name;
            if (sleigh != null)
            {
                await sleigh.Pack(present);
                return true;
            }
            return false;
        }
    }
}