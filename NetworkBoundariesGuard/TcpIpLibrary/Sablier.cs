using System;

namespace TcpIpLibrary
{
    public class Sablier
    {
        // *** PUBLIC ************************

        public bool Running { get; private set; } = false;

        public void Start(float elapse_sec)
        {
            _elapsedTime_ms = (long)(elapse_sec * 1e3f);
            _timeWhenStart = Time_ms();
            Running = true;
        }

        public void Update()
        {
            if (false == Running)
            {
                return;
            }

            var deltaTime_ms = Time_ms() - _timeWhenStart;
            if (deltaTime_ms > _elapsedTime_ms)
            {
                Running = false;
            }
        }

        // *** RESTRICTED ********************

        private static long Time_ms()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        private float _elapsedTime_ms;

        private long _timeWhenStart;
    }
}
