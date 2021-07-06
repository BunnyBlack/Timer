using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
                if (Time.realtimeSinceStartup* 1000 < task.DestTime)
                {
                    continue;
                }
                var callback = task.Callback;
                callback?.Invoke();
                
                _timerTaskList.RemoveAt(i);
                i--;
            }
        }

        /// <summary>
        /// 增加定时器回调，在不指定回调次数时，默认执行一次，传入 0 则无限次执行
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <param name="delay">延迟时间</param>
        /// <param name="unit">延迟时间<paramref name="delay"/>的单位，默认为毫秒</param>
        public void SetInterval(Action callback, float delay, TimerUnitEnum unit = TimerUnitEnum.Millisecond)
        {
            delay = ConvertToMilliseconds(delay, unit);
            var task = new TimerTask { Callback = callback, DestTime = Time.realtimeSinceStartup * 1000 + delay };
            _tempTaskList.Add(task);
        }

        private float ConvertToMilliseconds(float delay, TimerUnitEnum unit)
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
