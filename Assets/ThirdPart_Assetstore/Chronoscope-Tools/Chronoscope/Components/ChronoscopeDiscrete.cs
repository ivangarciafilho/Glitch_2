using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using ChronoscopeToolsInternal;

namespace ChronoscopeTools
{
    [HelpURL("https://github.com/johnmjep/Chronoscope-Tools-Public/wiki/User-Guide.ChronoscopeDiscrete-Timer")]
    [AddComponentMenu("Chronoscope Tools/Chronoscope Discrete")]
    [ExecuteInEditMode]
    public class ChronoscopeDiscrete : MonoBehaviour
    {
        #region Editor Only Fields
#if UNITY_EDITOR
        [SerializeField]
        private ChronoscopeColorPresets.PresetSchemes _colorScheme;

#pragma warning disable 0414
        [SerializeField]
        private bool _showSettings = false;
#pragma warning restore 0414

#endif
        #endregion Editor Only Fields

        #region Fields
        [Tooltip("Descriptive name")]
        [SerializeField]
        private string _name = "CHRONOSCOPE DISCRETE";
        public string Name { get { return _name; } }

        [Tooltip("Is the timer currently running")]
        [SerializeField]
        private bool _running = false;
        public bool Running { get { return _running; } }

        [Tooltip("Run continually")]
        [SerializeField]
        private bool _loop = false;
        public bool Loop { get { return _loop; } }

        [Tooltip("Used to reverse the timer when it reaches the end")]
        [SerializeField]
        private bool _pingPong = false;
        public bool PingPong { get { return _loop; } }

        [Tooltip("Evaluates some functions on start, rather than every update")]
        [SerializeField]
        private bool _optimise = false;

        [SerializeField]
        private bool _runOnAwake = false;
        public bool RunOnAwake { get { return _runOnAwake; } }

        [SerializeField]
        private float _duration = 1.0f;
        public float Duration { get { return _duration; } set { _duration = value; } }

        [SerializeField]
        private float _lastNormalisedEventTime = 0.0f;
        public float LastNormalisedEventTime { get { return _lastNormalisedEventTime; } }

        [Tooltip("Determines which time source is added to the timer value each update")]
        [SerializeField]
        private WaitSourceEnum _source = WaitSourceEnum.scaledTime;
        public WaitSourceEnum Source { get { return _source; } set { _source = value; } }
        
        private Action TimerCompleteAction;        

        private Dictionary<float, List<DiscreteListener>> _discreteListeners = new Dictionary<float, List<DiscreteListener>>();

        [SerializeField]
        private List<float> _discreteListenerTimes = new List<float>();
        [SerializeField]
        private TimerDirection _timerDirection = TimerDirection.up;

        public event DiscreteListener OnTimerStart;
        #endregion Fields

        #region Unity Specific Methods
        void Awake()
        {
            ResetTimer();
            OptimiseTimerCompletion();
            OrganizeEvents();
        }

        void Start()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                ResetTimer();
            }
#endif
            if (RunOnAwake && Application.isPlaying)
            {
                StartTimer();
            }
        }
        #endregion Unity Specific Methods

        #region Optimisation Methods
        /// <summary>
        /// Optimisation function to predetermine action to take on timer completion based on settings
        /// </summary>
        private void OptimiseTimerCompletion()
        {
            if (_optimise)
            {
                if (_loop)
                {
                    if (_pingPong)
                    {
                        TimerCompleteAction = TimerCompleteLoopPP;
                    }
                    else
                    {
                        TimerCompleteAction = TimerCompleteLoopNoPP;
                    }
                }
                else
                {
                    if (_pingPong)
                    {
                        TimerCompleteAction = TimerCompleteNoLoopPP;
                    }
                    else
                    {
                        TimerCompleteAction = TimerCompleteNoLoopNoPP;
                    }
                }
            }
            else
            {
                TimerCompleteAction = TimerCompleteNoOptimisation;
            }
        }
        #endregion Optimisation Methods

        #region Timer Completion Code
        // Only one of these methods will be executed, depending on the settings and optimisation

        /// <summary>
        /// Timer completion routine when Loop & Ping-Pong are disabled
        /// </summary>
        private void TimerCompleteNoLoopNoPP()
        {
            StopTimer();
        }

        /// <summary>
        /// Timer completion routine when Loop is enabled
        /// </summary>
        private void TimerCompleteLoopNoPP()
        {
            ResetTimer();
        }

        /// <summary>
        /// Timer completion routine when Ping-Pong enabled
        /// </summary>
        private void TimerCompleteNoLoopPP()
        {
            if (_timerDirection == TimerDirection.up)
            {
                _timerDirection = TimerDirection.down;
                ResetTimer();
            }
            else
            {
                _timerDirection = TimerDirection.up;
                StopTimer();
            }
        }

        /// <summary>
        /// Timer completion routine when Loop & Ping-Pong are enabled
        /// </summary>
        private void TimerCompleteLoopPP()
        {
            if (_timerDirection == TimerDirection.up)
            {
                _timerDirection = TimerDirection.down;
                ResetTimer();
            }
            else
            {
                _timerDirection = TimerDirection.up;
                ResetTimer();
            }
        }

        /// <summary>
        /// Determines action on timer completion without any optimisation (i.e. determines what action to take)
        /// </summary>
        private void TimerCompleteNoOptimisation()
        {
            if (_pingPong)
            {
                if (_timerDirection == TimerDirection.up)
                {
                    _timerDirection = TimerDirection.down;
                    ResetTimer();
                }
                else
                {
                    _timerDirection = TimerDirection.up;
                    if (_loop)
                    {
                        ResetTimer();
                    }
                    else
                    {
                        StopTimer();
                    }
                }
            }
            else
            {
                if (_loop)
                {
                    ResetTimer();
                }
                else
                {
                    StopTimer();
                }
            }
        }
        #endregion Timer Completion Code

        #region Listener/Event Methods
        /// <summary>
        /// Adda discrete time listener at the specified normalized time
        /// </summary>
        /// <param name="normalizedEventTime">Normalized time for the event to trigger at</param>
        /// <param name="listener">Listener to add</param>
        public void AddDiscreteListener(float normalisedEventTime, DiscreteListener listener)
        {
            normalisedEventTime = Mathf.Clamp(normalisedEventTime, 0.0f, 1.0f);
            if (_discreteListeners.ContainsKey(normalisedEventTime))
            {
                _discreteListeners[normalisedEventTime].Add(listener);
            }
            else
            {
                _discreteListeners.Add(normalisedEventTime, new List<DiscreteListener>() { listener });
            }
            OrganizeEvents();
        }

        /// <summary>
        /// Removes a discrete time listener at the specified normalized time
        /// </summary>
        /// <param name="normalizedEventTime">Event time to remove listener from</param>
        /// <param name="listener">Lsitener to remove</param>
        /// <returns>True if the normalized time event exists</returns>
        public bool RemoveDiscreteListener(float normalisedEventTime, DiscreteListener listener)
        {
            if (_discreteListeners.ContainsKey(normalisedEventTime))
            {
                if (_discreteListeners[normalisedEventTime].Remove(listener))
                {
                    OrganizeEvents();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes all discrete listeners
        /// </summary>
        public void RemoveAllDiscreteListeners()
        {
            _discreteListeners.Clear();
            OrganizeEvents();
        }

        /// <summary>
        /// Puts the events in the correct order and resets the event index
        /// </summary>
        private void OrganizeEvents()
        {
            if (_discreteListeners.Count > 0)
            {
                _discreteListenerTimes.Clear();
                foreach (KeyValuePair<float, List<DiscreteListener>> kVP in _discreteListeners)
                {
                    _discreteListenerTimes.Add(kVP.Key);
                }
                _discreteListenerTimes.Sort((f1, f2) => f1.CompareTo(f2));
            }
            ResetTimer();
        }

        /// <summary>
        /// Trigger all applicable events
        /// </summary>
        private void TriggerEvent(float normalisedEventTime)
        {
            _lastNormalisedEventTime = normalisedEventTime;
            List<DiscreteListener> listeners = _discreteListeners[normalisedEventTime];
            foreach (DiscreteListener listener in listeners)
            {
                listener();
            }
        }
        #endregion Listener/Event Methods

        #region Coroutines
        /// <summary>
        /// Starts a coroutine to trigger an event after the specified period
        /// </summary>
        /// <param name="normalisedDelay">Normalized time delay</param>
        /// <returns></returns>
        private IEnumerator WaitForEvent(float normalisedDelay)
        {
            if (_source == WaitSourceEnum.scaledTime)
            {
                yield return new WaitForSeconds(normalisedDelay * _duration);
            }
            else
            {
                yield return new WaitForSecondsRealtime(normalisedDelay * _duration);
            }
            TriggerEvent(normalisedDelay);            
        }

        /// <summary>
        /// Starts a coroutine to trigger an event after the inverse of the specifed period
        /// </summary>
        /// <param name="normalisedDelay">Normalized time delay</param>
        /// <returns></returns>
        private IEnumerator WaitForEventReverse(float normalisedDelay)
        {
            if (_source == WaitSourceEnum.scaledTime)
            {
                yield return new WaitForSeconds((1 - normalisedDelay) * _duration);
            }
            else
            {
                yield return new WaitForSecondsRealtime((1 - normalisedDelay) * _duration);
            }
            TriggerEvent(normalisedDelay);
        }

        /// <summary>
        /// Starts a coroutine to execute the time complete actions
        /// </summary>
        /// <returns></returns>
        private IEnumerator TimerCompleteDelay()
        {
            if (_source == WaitSourceEnum.scaledTime)
            {
                yield return new WaitForSeconds(_duration);
            }
            else
            {
                yield return new WaitForSecondsRealtime(_duration);
            }
            _lastNormalisedEventTime = (_timerDirection == TimerDirection.up) ? 1.0f : 0.0f;
            TimerCompleteAction();            
        }
        #endregion Coroutines

        #region Misc Methods
        /// <summary>
        /// Converts a timer value to a normalized timer value
        /// </summary>
        /// <param name="actualTimeValue">Value expressed in seconds</param>
        /// <returns>Timer value as a precentage (between 0 and 1)</returns>
        public float ActualToNormalisedTime(float actualTimeValue)
        {
            if (actualTimeValue <= 0)
            {
                return 0.0f;
            }
            else if (actualTimeValue >= _duration)
            {
                return 1.0f;
            }
            return _duration / actualTimeValue;
        }

        /// <summary>
        /// Converts a normalized value to a value in seconds
        /// </summary>
        /// <param name="normalisedTimeValue">Value expressed in percent (between 0 and 1)</param>
        /// <returns>Timer value in seconds</returns>
        public float NormalisedToActualTime(float normalisedTimerValue)
        {
            if (normalisedTimerValue <= 0)
            {
                return 0.0f;
            }
            else if (normalisedTimerValue >= _duration)
            {
                return _duration;
            }
            return _duration * normalisedTimerValue;
        }
        #endregion Misc Methods

        #region Timer Control
        /// <summary>
        /// Starts the timer running with optional reset
        /// </summary>
        /// <param name="reset">Reset the timer</param>
        public void StartTimer()
        {
            if (_running)
            {
                StopTimer();
            }
            _timerDirection = TimerDirection.up;
            _running = true;
            ResetTimer();
            if (OnTimerStart != null)
            {
                OnTimerStart();
            }
        }

        /// <summary>
        /// Begins coroutines to handle all events
        /// </summary>
        private void StartAllEvents()
        {
            // TODO: Refactor
            if (_timerDirection == TimerDirection.up)
            {
                foreach (float f in _discreteListenerTimes)
                {
                    StartCoroutine("WaitForEvent", f);
                }
            }
            else
            {
                foreach (float f in _discreteListenerTimes)
                {
                    StartCoroutine("WaitForEventReverse", f);
                }
            }
            StartCoroutine("TimerCompleteDelay");
        }

        /// <summary>
        /// Stops and resets the timer
        /// </summary>
        public void StopTimer()
        {            
            _running = false;
            ResetTimer();
        }

        /// <summary>
        /// Resets the timer and reinitializes the event list
        /// </summary>
        public void ResetTimer()
        {
            StopAllCoroutines();
            if (_running)
            {
                StartAllEvents();
            }
        }
        #endregion Timer Control
    }
}
