using System.Timers;
using Timer = System.Timers.Timer;

namespace UnmistakableAPKInstaller.Core.Controllers
{
    public class TimerController
    {
        List<Timer> activeTimers = new List<Timer>();

        public void InitTimers(Action<object, ElapsedEventArgs> UpdateDeviceListAction)
        {
            // Set timer intervals
            Timer updateDeviceListTimer = CreateUpdateDeviceListTimer(5000);
            activeTimers.Add(updateDeviceListTimer);

            foreach (var timer in activeTimers)
            {
                timer.Start();
            }

            Timer CreateUpdateDeviceListTimer(double interval)
            {
                Timer timer = new Timer();
                timer.Elapsed += new ElapsedEventHandler(UpdateDeviceListAction);
                timer.Interval = interval;
                return timer;
            }
        }

        public void StopAllTimers()
        {
            foreach (var activeTimer in activeTimers)
            {
                activeTimer.Stop();
            }
        }
    }
}
