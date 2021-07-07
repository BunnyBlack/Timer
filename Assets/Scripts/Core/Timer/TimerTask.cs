using System;

namespace Core.Timer
{
    public class TimerTask
    {
        public int Tid { get; set; }
        public float DestTime { get; set; }
        public Action Callback { get; set; }
        public int CallTimes { get; set; }
        public float Delay { get; set; }
    }
}
