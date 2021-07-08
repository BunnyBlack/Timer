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
            TimerSystem.Timer.ClearInterval(_tid);
            _tid = TimerSystem.Timer.SetInterval(TimerCallback, 1000, 0);
            Debug.Log($"Start Interval! tid:{_tid.ToString()}");
        }

        public void ClickCancelBtn()
        {
            TimerSystem.Timer.ClearInterval(_tid);
        }

        private void TimerCallback()
        {
            Debug.Log("Delay Log");
        }
    }
}
