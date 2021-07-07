using Core.Timer;
using UnityEditor;
using UnityEngine;

namespace Core
{
    public class GameRoot : MonoBehaviour
    {
        private int _tid;

        private void Start()
        {
            Debug.Log("Game Start...");

            GetComponent<TimerSystem>().InitTimer();
        }

        public void ClickAddBtn()
        {
            _tid = TimerSystem.Timer.SetInterval(TimerCallback, 1000f, 0);
            Debug.Log($"Start Interval! tid:{_tid.ToString()}");
        }

        public void ClickCancelBtn()
        {
            Debug.Log(TimerSystem.Timer.ClearInterval(_tid)
                ? $"Remove succeed! tid: {_tid.ToString()}"
                : $"Remove failed! Please Check tid: {_tid.ToString()}");
        }

        private void TimerCallback()
        {
            Debug.Log("Delay Log");
        }
    }
}
