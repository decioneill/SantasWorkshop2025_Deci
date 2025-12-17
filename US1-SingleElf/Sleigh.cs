using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace SantasWorkshop2025
{
    /// <summary>
    /// Sleigh for Delivering Present
    /// </summary>
    public class Sleigh : ISleigh, IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.LogRequest += new EventHandler(OnLogRequest);
        }

        public void OnLogRequest(Object source, EventArgs e)
        {
            //custom logging logic can go here
        }

        #endregion

        /// <summary>
        /// Concurrent Dictionary for Threadsafe handling. All Packed Presents sorted in Family Groups.
        /// </summary>
        public ConcurrentDictionary<string, ConcurrentQueue<Present>> PackedPresents = new ConcurrentDictionary<string, ConcurrentQueue<Present>>();

        /// <summary>
        /// Pack a delivered Parcel for Christmas
        /// </summary>
        /// <param name="present">present to pack</param>
        /// <returns>success</returns>
        public async Task<bool> Pack(Present present)
        {
            if (present == null) return false;
            await Task.Run(() => {
                if (PackedPresents.ContainsKey(present.Family))
                {
                    PackedPresents[present.Family].Enqueue(present);
                }
                else
                {
                    ConcurrentQueue<Present> familyPresents = new ConcurrentQueue<Present>();
                    familyPresents.Enqueue(present);
                    if (!PackedPresents.TryAdd(present.Family, familyPresents))
                    {
                        PackedPresents[present.Family].Enqueue(present);
                    };
                }

                Console.WriteLine($"Present \"{present.Name}\", created by {present.CreatedByMachine}, delivered by {present.DeliveredByElf}, packed.");
            });
            return true;
        }

        public int RemoveFamilyPresents(string familyName)
        {
            if (PackedPresents.ContainsKey(familyName))
            {
                PackedPresents.TryRemove(familyName, out ConcurrentQueue<Present> removedPresents);
                return removedPresents.Count;
            }
            return 0;
        }
    }
}
