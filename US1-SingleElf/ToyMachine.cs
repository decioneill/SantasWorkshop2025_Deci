using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace US1_SingleElf
{
    public class ToyMachine
    {
        public Present MakePresent(string name)
        {
            Present present = new Present(name);
            return present;
        }
    }
}
