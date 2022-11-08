using System.Timers;
using Timer = System.Timers.Timer;

namespace UnmistakableAPKInstaller.Core.Controllers
{
    /// <summary>
    /// Container to start/stop timers.
    /// With specific timers for UI
    /// </summary>
    public class TimerController
    {
        List<Timer> activeTimers = new List<Timer>();

        /// <summary>
        /// Method to init specific timers
        /// </summary>
        /// <param name="UpdateDeviceListAction"></param>
        public void InitTimers(Action<object, ElapsedEventArgs> UpdateDeviceListAction)
        {
            // Create timers
            Timer updateDeviceListTimer = CreateUpdateDeviceListTimer();
            activeTimers.Add(updateDeviceListTimer);

            foreach (var timer in activeTimers)
            {
                timer.Start();
            }

            // TODO: remove default interval
            Timer CreateUpdateDeviceListTimer(double interval = 5000)
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
