using Core.Timer;
using UnityEditor;
using UnityEngine;

namespace Core
{
    public class GameRoot : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("Game Start...");

            GetComponent<TimerSystem>().InitTimer();
        }

        public void ClickAddBtn()
        {
            TimerSystem.Timer.SetInterval(TimerCallback, 1000f);
        }

        private void TimerCallback()
        {
            Debug.Log("Delay Log");
        }
    }
}
