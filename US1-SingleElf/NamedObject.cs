using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SantasWorkshop2025
{
    /// <summary>
    /// Simple interface to allow for type limiting on objects with Names (Id)
    /// </summary>
    public interface NamedObject
    {
        string Name { get; set; }
    }
}
