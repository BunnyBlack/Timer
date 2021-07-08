using System;
using System.Collections.Generic;

namespace Core.Timer
{
    public class CallTimer
    {
        private readonly List<TimerTask> _timerTaskList = new List<TimerTask>();
        private readonly List<int> _tidList = new List<int>();
        // 计时器缓存列表
        private readonly List<TimerTask> _tempTaskList = new List<TimerTask>();
        // 计时器tid清除列表
        private readonly List<int> _recycleTidList = new List<int>();
        private readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);
        private readonly Action<string> _logHandler;
        private static readonly object TimerLocker = new object();


        // 加锁
        private int _increaseKey;
        private double _nowTimeStamp;
        
        public CallTimer(Action<string> logHandler)
        {
            _timerTaskList.Clear();
            _tidList.Clear();
            _tempTaskList.Clear();
            _recycleTidList.Clear();
            
            _logHandler = logHandler;
        }

        #region Public functions

        public void Update()
        {
            if (_tempTaskList.Count != 0)
            {
                CollectNewTimerTasks();
            }
            CheckTimerTasks();

            if (_recycleTidList.Count != 0)
            {
                RecycleTidList();
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
        public int SetInterval(Action callback, double delay, int callTimes = 1, TimerUnitEnum unit = TimerUnitEnum.Millisecond)
        {
            var tid = GetTid();
            delay = ConvertToMilliseconds(delay, unit);
            _nowTimeStamp = GetUTCMilliseconds();
            var task = new TimerTask
            {
                Tid = tid,
                DestTime = _nowTimeStamp + delay,
                Callback = callback,
                CallTimes = callTimes,
                Delay = delay
            };
            _tempTaskList.Add(task);

            return tid;
        }

        /// <summary>
        /// 取消tid所指向的定时器任务
        /// </summary>
        /// <param name="tid">计时器tid</param>
        /// <returns>移除结果</returns>
        public bool ClearInterval(int tid)
        {
            lock (TimerLocker)
            {
                if (!_tidList.Contains(tid))
                {
                    return false;
                }

                // 在计时器列表中寻找
                for (var i = 0; i < _timerTaskList.Count; i++)
                {
                    var task = _timerTaskList[i];
                    if (task.Tid != tid)
                        continue;

                    _timerTaskList.RemoveAt(i);
                    _tidList.Remove(tid);
                    return true;
                }

                // 在计时器缓存列表中寻找
                for (var i = 0; i < _tempTaskList.Count; i++)
                {
                    var task = _tempTaskList[i];
                    if (task.Tid != tid)
                        continue;

                    _tempTaskList.RemoveAt(i);
                    _tidList.Remove(tid);
                    return true;
                }

                return false;
            }
        }

        public double GetTimeStamp()
        {
            return _nowTimeStamp;
        }

        public DateTime GetLocalDateTime()
        {
            var dt = TimeZoneInfo.ConvertTimeFromUtc(_unixEpoch.AddMilliseconds(_nowTimeStamp), TimeZoneInfo.Local);
            return dt;
        }

        public int GetLocalYear()
        {
            return GetLocalDateTime().Year;
        }
        public int GetLocalMonth()
        {
            return GetLocalDateTime().Month;
        }
        public int GetLocalDay()
        {
            return GetLocalDateTime().Day;
        }
        public int GetLocalDayOfWeek()
        {
            return (int)GetLocalDateTime().DayOfWeek;
        }

        public string GetLocalTimeStr()
        {
            var dt = GetLocalDateTime();
            var timeStr = $"{ConvertTimeStr(dt.Hour)}:{ConvertTimeStr(dt.Minute)}:{ConvertTimeStr(dt.Second)}";
            return timeStr;
        }

        #endregion

        #region Private functions

         private void LogInfo(string info)
        {
            _logHandler?.Invoke($"CallTimer Log: {info}");
        }

        /// <summary>
        /// 检查计时器任务是否达到目标时间
        /// </summary>
        private void CheckTimerTasks()
        {

            _nowTimeStamp = GetUTCMilliseconds();
            for (var i = 0; i < _timerTaskList.Count; i++)
            {
                var task = _timerTaskList[i];
                if (_nowTimeStamp.CompareTo(task.DestTime) < 0)
                {
                    continue;
                }
                var callback = task.Callback;
                callback?.Invoke();
                if (task.CallTimes == 1)
                {
                    _timerTaskList.RemoveAt(i);
                    _recycleTidList.Add(task.Tid);
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
        /// 将新增的计时器任务从缓存中正式转移到任务列表
        /// </summary>
        private void CollectNewTimerTasks()
        {
            _timerTaskList.AddRange(_tempTaskList);
            _tempTaskList.Clear();
        }
        
        private void RecycleTidList()
        {
            foreach (var tid in _recycleTidList)
            {
                _tidList.Remove(tid);
            }

            _recycleTidList.Clear();
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

        private double ConvertToMilliseconds(double delay, TimerUnitEnum unit)
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
                    LogInfo("错误的时间类型，无法转换");
                    break;
            }
            return delay;
        }

        private double GetUTCMilliseconds()
        {
            var timeSpan = DateTime.UtcNow - _unixEpoch;
            return timeSpan.TotalMilliseconds;
        }

        private string ConvertTimeStr(int time)
        {
            return time < 10 ? $"0{time.ToString()}" : time.ToString();
        }
        
        #endregion
    }
}
