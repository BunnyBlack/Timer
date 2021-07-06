using System;

namespace Core.Timer
{
    public class TimerTask
    {
        public float DestTime { get; set; }
        public Action Callback { get; set; }
    }
}
