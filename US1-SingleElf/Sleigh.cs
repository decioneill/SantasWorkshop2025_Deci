using System;
using System.Threading.Tasks;
using System.Web;

namespace US1_SingleElf
{
    public class Sleigh : IHttpModule
    {
        /// <summary>
        /// You will need to configure this module in the Web.config file of your
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: https://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpModule Members

        public void Dispose()
        {
            //clean-up code here.
        }

        public void Init(HttpApplication context)
        {
            // Below is an example of how you can handle LogRequest event and provide 
            // custom logging implementation for it
            context.LogRequest += new EventHandler(OnLogRequest);
        }

        #endregion

        Random numberRandomizer = new Random();

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
                Console.WriteLine($"{delayAmount}: Present \"{present.Name}\", created by {present.CreatedByMachine}, delivered by {present.DeliveredByElf}, packed and sent.");
            });
            return true;
        }
    }
}
