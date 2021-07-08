using System;
using UnityEngine;

namespace Core.Timer
{
    /// <summary>
    /// 定时器
    /// </summary>
    public class TimerSystem : MonoBehaviour
    {
        public static TimerSystem Timer;
        private CallTimer _timer; 

        public void InitTimer()
        {
            Timer = this;
            _timer = new CallTimer(Debug.Log);
            Debug.Log("InitTimer...");
        }

        private void Update()
        {
            _timer.Update();
        }

        /// <summary>
        /// 增加定时器回调，在不指定回调次数时，默认执行一次，传入 0 则无限次执行
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <param name="delay">延迟时间</param>
        /// <param name="callTimes">调用次数，传入0时无限执行</param>
        /// <param name="unit">延迟时间<paramref name="delay"/>的单位</param>
        /// <returns>计时器tid</returns>
        public int SetInterval(Action callback, double delay, int callTimes = 1, TimerUnitEnum unit = TimerUnitEnum.Millisecond)
        {
            return _timer.SetInterval(callback, delay, callTimes, unit);
        }

        /// <summary>
        /// 取消tid所指向的定时器任务
        /// </summary>
        /// <param name="tid">计时器tid</param>
        /// <returns>移除结果</returns>
        public void ClearInterval(int tid)
        {
            _timer.ClearInterval(tid);
        }
    }
}
