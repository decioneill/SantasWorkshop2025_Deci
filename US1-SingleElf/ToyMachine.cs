using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace US1_SingleElf
{
    public class ToyMachine : NamedObject
    {
        public string Name { get; set; }

        public ToyMachine()
        {
        }

        public Present MakePresent(string presentName, string familyName)
        {
            Present present = new Present() { Name = presentName, Family = familyName, CreatedByMachine = Name };
            return present;
        }
    }
}
