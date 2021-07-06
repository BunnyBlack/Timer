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
                if (Time.realtimeSinceStartup < task.DestTime)
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
        public void SetInterval(Action callback, float delay)
        {
            var task = new TimerTask { Callback = callback, DestTime = Time.realtimeSinceStartup + delay };
            _tempTaskList.Add(task);
        }
    }
}
