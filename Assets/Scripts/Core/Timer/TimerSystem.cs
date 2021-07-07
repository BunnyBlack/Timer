using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace Core.Timer
{
    /// <summary>
    /// 定时器
    /// </summary>
    public class TimerSystem : MonoBehaviour
    {
        public static TimerSystem Timer;

        private readonly List<TimerTask> _timerTaskList = new List<TimerTask>();
        private readonly List<TimerTask> _tempTaskList = new List<TimerTask>();
        private readonly List<int> _tidList = new List<int>();

        private int _increaseKey = 0;
        // 加锁
        private static readonly object TimerLocker = new object();

        public void InitTimer()
        {
            Timer = this;
            Debug.Log("InitTimer...");
        }

        private void Update()
        {
            _timerTaskList.AddRange(_tempTaskList);
            _tempTaskList.Clear();

            for (var i = 0; i < _timerTaskList.Count; i++)
            {
                var task = _timerTaskList[i];
                if (Time.realtimeSinceStartup * 1000 < task.DestTime)
                {
                    continue;
                }
                var callback = task.Callback;
                callback?.Invoke();
                if (task.CallTimes == 1)
                {
                    _timerTaskList.RemoveAt(i);
                    i--;
                }
                else
                {
                    if (task.CallTimes != 0)
                    {
                        task.CallTimes--;
                    }
                    task.DestTime += task.Delay;
                }

            }
        }

        /// <summary>
        /// 增加定时器回调，在不指定回调次数时，默认执行一次，传入 0 则无限次执行
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <param name="delay">延迟时间</param>
        /// <param name="callTimes">调用次数，传入0时无限执行</param>
        /// <param name="unit">延迟时间<paramref name="delay"/>的单位</param>
        /// <returns>计时器tid</returns>
        public int SetInterval(Action callback, float delay,int callTimes = 1, TimerUnitEnum unit = TimerUnitEnum.Millisecond)
        {
            var tid = GetTid();
            delay = ConvertToMilliseconds(delay, unit);
            var task = new TimerTask
            {
                Tid = tid,
                DestTime = Time.realtimeSinceStartup * 1000 + delay,
                Callback = callback,
                CallTimes = callTimes,
                Delay = delay
            };
            _tempTaskList.Add(task);
            
            return tid;
        }

        private int GetTid()
        {
            lock (TimerLocker)
            {
                _increaseKey++;
                if (_increaseKey == int.MaxValue)
                {
                    _increaseKey = 1;
                }
                while (_tidList.Contains(_increaseKey))
                {
                    _increaseKey++;
                }
                
                _tidList.Add(_increaseKey);
                return _increaseKey;
            }
        }

        private static float ConvertToMilliseconds(float delay, TimerUnitEnum unit)
        {
            switch (unit)
            {
                case TimerUnitEnum.Millisecond:
                    break;
                case TimerUnitEnum.Second:
                    delay *= 1000;
                    break;
                case TimerUnitEnum.Minute:
                    delay = delay * 1000 * 60;
                    break;
                case TimerUnitEnum.Hour:
                    delay = delay * 1000 * 60 * 60;
                    break;
                case TimerUnitEnum.Day:
                    delay = delay * 1000 * 60 * 60 * 24;
                    break;
                default:
                    Debug.Log("错误的时间类型，无法转换");
                    break;
            }
            return delay;
        }

    }
}
