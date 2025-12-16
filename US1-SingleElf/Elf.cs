using System;
using System.Threading.Tasks;

namespace SantasWorkshop2025
{
    /// <summary>
    /// Elves, the happy workers at Santas Workshop.
    /// </summary>
    public class Elf : NamedObject
    {
        /// <summary>
        /// Elf Name (Id)
        /// </summary>
        public string Name {  get; set; }

        /// <summary>
        /// Deliver a present to the Sleigh
        /// </summary>
        /// <param name="present">present</param>
        /// <param name="sleigh">sleigh</param>
        /// <returns>success</returns>
        public async Task<bool> DeliverPresent(Present present, Sleigh sleigh)
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