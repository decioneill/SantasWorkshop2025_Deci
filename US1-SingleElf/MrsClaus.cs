using System;
using System.Collections.Generic;

namespace SantasWorkshop2025
{
    /// <summary>
    /// Santa's Wife, checking the naughty list.
    /// </summary>
    public class MrsClaus
    {
        Random numberRandomizer = new Random();

        /// <summary>
        /// Run over the family names to ensure no naughty family is sneaking through.
        /// </summary>
        /// <param name="familyNames">List of families</param>
        /// <returns>name of a naughty family if found</returns>
        public string DoubleCheckNaughtyList(List<string> familyNames)
        {
            // 1 in 4 chance new family naughty to remove
            bool isNewNaughtyFamily = numberRandomizer.Next(1,4) == 1;
            return isNewNaughtyFamily ? familyNames[numberRandomizer.Next(familyNames.Count - 1)] : null;
        }
    }
}
