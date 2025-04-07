using System.Collections.Generic;
using UnityEngine;
using Utils;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace TelemetrySystem {

    public class Tracker : MonoBehaviour
    {
        #region Singleton
        private static Tracker _instance = null;
        public static Tracker Instance { get { return _instance; } }
        #endregion

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);

                _events = new Queue<Event>();
                _persistentEvents = new PriorityQueue<PersistentEvent, long>();

                mutEvents = new Mutex();
                mutPersistentEvents = new Mutex();
                mutDestroyed = new Mutex();

                TrackPersistentEvent(new MiauEvent(EventType.APPLICATION_START, 500));
                TrackPersistentEvent(new MiauEvent(EventType.SCENE_CHANGED, 200));
                TrackPersistentEvent(new MiauEvent(EventType.APPLICATION_END, 300));

                Parallel.Invoke(TestingPersistentEvents, PersistentEventTracking);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            mutDestroyed.WaitOne();
            destroyed = true;
            mutDestroyed.ReleaseMutex();

        }

        private Mutex mutEvents;
        private Mutex mutPersistentEvents;
        private Mutex mutDestroyed;

        static bool destroyed = false;

        // ESTO DE MOMENTO ES UNA PRUEBA PARA LOS MUTEX UN SALUDO
        async void TestingPersistentEvents()
        {
            while (!destroyed)
            {
                mutEvents.WaitOne();
                await Task.Run(async () => { await Task.Delay(1); });
                mutEvents.ReleaseMutex();

                mutPersistentEvents.WaitOne();
                await Task.Run(async () => { await Task.Delay(1); });
                mutPersistentEvents.ReleaseMutex();
            }
        }
        
        async void PersistentEventTracking()
        {
            bool empty = !_persistentEvents.TryPeek(out PersistentEvent _, out long firstPrio);
            
            if (empty)
                return;
            
            long currentTimeStamp = firstPrio;
            while(!destroyed)
            {
                // mutex
                mutPersistentEvents.WaitOne();
                _persistentEvents.TryDequeue(out PersistentEvent e, out long priority);
                mutPersistentEvents.ReleaseMutex();
                // unlock


                Debug.Log("Waiting for: " + (priority - currentTimeStamp).ToString() + "ms.");
                await Task.Run(async () => { await Task.Delay((int)(priority - currentTimeStamp)); });

                currentTimeStamp = priority;

                e.UpdateTimeStamp();

                // mutex
                mutEvents.WaitOne();
                PushEvent(e);
                mutEvents.ReleaseMutex();
                // unlock

                // mutex
                mutPersistentEvents.WaitOne();
                _persistentEvents.Enqueue(e, e.AdvanceTimer());
                mutPersistentEvents.ReleaseMutex();
                // unlock
            }
        }
        
        #region Parameters
        [SerializeField] float _timeToDumpQueue;

        #endregion

        #region Private Variables
        Queue<Event> _events;
        PriorityQueue<PersistentEvent, long> _persistentEvents;
        
        #endregion


        public void PushEvent(Event e)
        {
            // LOCK MUTEX

            // Si está activo... 
                // lo metemos en la cola
            
            _events.Enqueue(e);

            // Si no está activa...
                // lo ignoramos
            
            // UNLOCK MUTEX
        }

        public void TrackPersistentEvent(PersistentEvent e)
        {
            // LOCK MUTEX

            // Si está activo... 
            // lo metemos en la cola
            e.UpdatePersistentTime();

            _persistentEvents.Enqueue(e, e._currentPersistentTime);

            // Si no está activa...
            // lo ignoramos

            // UNLOCK MUTEX
        }
    }
}
