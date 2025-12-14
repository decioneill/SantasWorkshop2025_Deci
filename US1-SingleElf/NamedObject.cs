using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US1_SingleElf
{
    /// <summary>
    /// Simple interface to allow for type limiting on objects with Names (Id)
    /// </summary>
    public interface NamedObject
    {
        string Name { get; set; }
    }
}
