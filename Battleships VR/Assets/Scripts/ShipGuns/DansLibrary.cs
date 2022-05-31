using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DansLibrary
{
    #region GameObjectMethods
    public class DansLibrary : MonoBehaviour
    {
        #region LibrarySingleton
        public static DansLibrary instance;
        private void Awake()
        {
            if (instance == null)
                instance = this;
        }
        #endregion


        private void Start()
        {
            Debug.Log("Do somethign");
        }
    }
    #endregion

    #region GlobalTimer
    public class GlobalTimer
    {
        private float maxTime;
        public float MaxTime { get { return maxTime; } private set { maxTime = value; } }

        public event System.Action timerCompleted;

        public GlobalTimer(float maxTimeAllowed)
        {
            MaxTime = maxTimeAllowed;
        }

        private IEnumerator coroutineRunning;
        private bool TimerIntermediary()
        {
            //Intermediary function to only have one instance of the coroutine running at once
            if(coroutineRunning == null)
            {
                return true;
            }
            return false;
        }

        //used when a timer is needed while the time scale is 0
        public IEnumerator UnscaledTimer()
        {
            if (TimerIntermediary())
            {
                float countdown = 0f;
                coroutineRunning = UnscaledTimer();
                do
                {
                    countdown += Time.unscaledDeltaTime;
                    yield return null;
                }
                while (countdown < MaxTime);

                timerCompleted.Invoke();

                yield return 0; //Wait one frame
                coroutineRunning = null;
            }

        }

        //used when a timer is needed while the time scale is normal
        public IEnumerator ScaledTimer()
        {
            if (TimerIntermediary())
            {
                float countdown = 0f;
                coroutineRunning = ScaledTimer();
                do
                {
                    countdown += Time.unscaledDeltaTime;
                    yield return null;
                }
                while (countdown < MaxTime);

                timerCompleted.Invoke();

                yield return 0; //Wait one frame
                coroutineRunning = null;
            }
        }

    }
    #endregion

}



