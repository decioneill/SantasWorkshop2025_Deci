using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SantasWorkshop2025
{
    public interface ISleigh
    {
        Task<bool> Pack(Present present);

        int RemoveFamilyPresents(string familyName);
    }
}
