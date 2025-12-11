using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace US1_SingleElf
{
    public class Sleigh : IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.LogRequest += new EventHandler(OnLogRequest);
        }

        #endregion

        Random numberRandomizer = new Random();

        Dictionary<string, List<Present>> _packedPresents = new Dictionary<string, List<Present>>();

        public void OnLogRequest(Object source, EventArgs e)
        {
            //custom logging logic can go here
        }

        public async Task<bool> Pack(Present present)
        {
            int delayAmount = numberRandomizer.Next(1, 1000);
            await Task.Delay(delayAmount);
            if (present == null) return false;
            await Task.Run(() => {
                if (_packedPresents.ContainsKey(present.Family))
                {
                    _packedPresents[present.Family].Add(present);
                }
                else
                {
                    _packedPresents.Add(present.Family, new List<Present>() { present });
                }

                Console.WriteLine($"{delayAmount}: Present \"{present.Name}\", created by {present.CreatedByMachine}, delivered by {present.DeliveredByElf}, packed.");
            });
            return true;
        }
    }
}
