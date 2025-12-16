using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SantasWorkshop2025
{
    /// <summary>
    /// Toy Machine for producing Presents
    /// </summary>
    public class ToyMachine : NamedObject
    {
        /// <summary>
        /// Name of Toy Machine
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Make a brand new present
        /// </summary>
        /// <param name="presentName">Name to give present</param>
        /// <param name="familyName">Family to receive present</param>
        /// <returns>A new Present</returns>
        public Present MakePresent(string presentName, string familyName)
        {
            Present present = new Present() { Name = presentName, Family = familyName, CreatedByMachine = Name };
            return present;
        }
    }
}
