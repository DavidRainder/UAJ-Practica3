using System.Collections.Generic;
using UnityEngine;
using Utils;
using System.Threading.Tasks;
using System.Threading;

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

            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _events = new Queue<Event>();
            _persistentEvents = new PriorityQueue<PersistentEvent, long>();
            _eventRegistry = GetComponent<EventRegistry>();

            // error handling de ^^

            mutEvents = new Mutex();
            mutPersistentEvents = new Mutex();
            mutDestroyed = new Mutex();

            Parallel.Invoke(TestingPersistentEvents, PersistentEventTracking);
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

        EventRegistry _eventRegistry = null;

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

                // pillas un evento
                _persistentEvents.TryDequeue(out PersistentEvent e, out long priority);
                
                // unlock
                mutPersistentEvents.ReleaseMutex();


                Debug.Log("Waiting for: " + (priority - currentTimeStamp).ToString() + "ms.");
                
                await Task.Run(async () => { await Task.Delay((int)(priority - currentTimeStamp)); });

                currentTimeStamp = priority;

                e.UpdateTimeStamp();

                // mutex
                mutEvents.WaitOne();

                PushEvent(e);
                
                // unlock
                mutEvents.ReleaseMutex();

                // mutex
                mutPersistentEvents.WaitOne();
                
                _persistentEvents.Enqueue(e, e.AdvanceTimer());
                
                // unlock
                mutPersistentEvents.ReleaseMutex();
            }
        }
        
        #region Parameters
        [SerializeField] float _timeToDumpQueue;

        #endregion

        #region Private Variables
        Queue<Event> _events;
        // el Long es el tiempo en POSIX en el que se debe ejecutar
        PriorityQueue<PersistentEvent, long> _persistentEvents;
        
        #endregion


        public void PushEvent(Event e)
        {
            if (!_eventRegistry.IsEventActive(e.GetID())) return;

            // LOCK MUTEX
            mutEvents.WaitOne();
            // Si está activo... 
                // lo metemos en la cola
            
            _events.Enqueue(e);

            // Si no está activa...
            // lo ignoramos

            // UNLOCK MUTEX
            mutEvents.ReleaseMutex();

        }

        public void TrackPersistentEvent(PersistentEvent e)
        {
            if (!_eventRegistry.IsEventActive(e.GetID())) return;

            // LOCK MUTEX
            mutPersistentEvents.WaitOne();

            // Si está activo... 
            // lo metemos en la cola
            e.UpdatePersistentTime();

            _persistentEvents.Enqueue(e, e._currentPersistentTime);

            // Si no está activa...
            // lo ignoramos

            // UNLOCK MUTEX
            mutPersistentEvents.ReleaseMutex();
        }
    }
}
