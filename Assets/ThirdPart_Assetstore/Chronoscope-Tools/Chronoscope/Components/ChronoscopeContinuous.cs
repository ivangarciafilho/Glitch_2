using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using ChronoscopeToolsInternal;

namespace ChronoscopeTools
{
    [HelpURL("https://github.com/johnmjep/Chronoscope-Tools-Public/wiki/User-Guide.ChronoscopeContinuous-Timer")]
    [AddComponentMenu("Chronoscope Tools/Chronoscope Continuous")]
    [ExecuteInEditMode]
    public class ChronoscopeContinuous : MonoBehaviour
    {
        #region Editor Only Fields
#if UNITY_EDITOR
        [SerializeField]
        private ChronoscopeColorPresets.PresetSchemes _colorScheme;

#pragma warning disable 0414
        [SerializeField]
        private bool _showSettings = false;
#pragma warning restore 0414

        public delegate void RepaintAction();
        public event RepaintAction WantRepaint;
#endif
        #endregion Editor Only Fields

        #region Fields
        [Tooltip("Descriptive name")]
        [SerializeField]
        private string _name = "CHRONOSCOPE CONTINUOUS";
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

        [Tooltip("Use this to start the timer automatically with the scene")]
        [SerializeField]
        private bool _runOnAwake = false;
        public bool RunOnAwake { get { return _runOnAwake; } }

        [Tooltip("How long the timer will run for")]
        [SerializeField]
        private float _duration = 1.0f;
        public float Duration { get { return _duration; } set { _duration = value; } }
                
        [SerializeField]
        private float _value = 0.0f;
        public float Value { get { return _value; } }
                
        [SerializeField]
        private float _normalisedValue = 0.0f;
        public float NormalisedValue { get { return _normalisedValue; } }

        [Tooltip("Determines which time source is added to the timer value each update")]
        [SerializeField]
        private TimerSourceEnum _source = TimerSourceEnum.deltaTime;
        public TimerSourceEnum Source { get { return _source; } set { _source = value; } }

        [Tooltip("Use if this timer affects physics objects")]
        [SerializeField] private bool _fixedUpdate = false;
        public bool FixedUpdate { get { return _fixedUpdate; } set { _fixedUpdate = value; } }

        private Func<float> GetElapsedTime;
        private Action TimerCompleteAction;        

        private List<ContinuousListener> _continuousListeners = new List<ContinuousListener>();

        [SerializeField]
        private TimerDirection _timerDirection = TimerDirection.up;
        public TimerDirection TimerDirectionFlag { get { return _timerDirection; } }

        public event DiscreteListener OnTimerStart;
        public event DiscreteListener OnTimerComplete;
        #endregion Fields

        #region Unity Specific Methods
        void Awake()
        {
            ResetTimer();
            EvaluateElapsedTimeFunc();
            OptimiseTimerCompletion();
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
                StartTimer(false);
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

        /// <summary>
        /// Optimization function to pre-evaluate which time source to use
        /// </summary>
        private void EvaluateElapsedTimeFunc()
        {
            switch (_source)
            {
                case TimerSourceEnum.deltaTime:
                    GetElapsedTime = () => Time.deltaTime;
                    break;
                case TimerSourceEnum.fixedDeltaTime:
                    GetElapsedTime = () => Time.fixedDeltaTime;
                    break;
                case TimerSourceEnum.fixedUnscaledDeltaTime:
                    GetElapsedTime = () => Time.fixedUnscaledDeltaTime;
                    break;
                case TimerSourceEnum.smoothDeltaTime:
                    GetElapsedTime = () => Time.smoothDeltaTime;
                    break;
                case TimerSourceEnum.unscaledDeltaTime:
                    GetElapsedTime = () => Time.unscaledDeltaTime;
                    break;
                default:
                    Debug.LogError("Invalid Timer Source!");
                    GetElapsedTime = () => 0;
                    break;
            }
        }
        #endregion Optimisation Methods

        #region Coroutines
        /// <summary>
        /// Coroutine to update the timer every frame
        /// </summary>
        /// <returns>IEnumerator</returns>
        private IEnumerator UpdateTimer()
        {
            while (_value < _duration)
            {
                _value += GetElapsedTime();
                if (_value >= _duration)
                {
                    _value = _duration;
                    UpdateNormalisedValue();
                    TriggerContinuousEvents();                    
                    TimerCompleteAction();
                    if (OnTimerComplete != null)
                    {
                        OnTimerComplete();
                    }
                }
                else
                {
                    UpdateNormalisedValue();
                    TriggerContinuousEvents();
                }
                yield return _fixedUpdate ? new WaitForFixedUpdate() : null;
            }
        }
        #endregion Coroutines

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
        /// Add a continuous (every frame) listener to the timer
        /// </summary>
        /// <param name="listener">The listener to add</param>
        public void AddContinuousListener(ContinuousListener listener)
        {
            _continuousListeners.Add(listener);
        }

        /// <summary>
        /// Removes a continuous listener
        /// </summary>
        /// <param name="listener">The listener to be removed</param>
        /// <returns>True if successful</returns>
        public bool RemoveContinuousListener(ContinuousListener listener)
        {
            return _continuousListeners.Remove(listener);
        }

        /// <summary>
        /// Removes all continuous listeners
        /// </summary>
        public void RemoveAllContinuousListeners()
        {
            _continuousListeners.Clear();
        }

        /// <summary>
        /// Trigger all continuous listeners
        /// </summary>
        private void TriggerContinuousEvents()
        {
            foreach (ContinuousListener cL in _continuousListeners)
            {
                cL(_normalisedValue);
            }
        }
        #endregion Listener/Event Methods

        #region Misc Methods
        /// <summary>
        /// Recalculate the normalised timer value
        /// </summary>
        private void UpdateNormalisedValue()
        {
            if (_timerDirection == TimerDirection.up)
            {
                _normalisedValue = _value / _duration;
            }
            else
            {
                _normalisedValue = 1 - (_value / _duration);
            }

#if UNITY_EDITOR
            if (Application.isEditor)
            {
                if (WantRepaint != null)
                {
                    WantRepaint();
                }
            }
#endif
        }

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
        public void StartTimer(bool reset)
        {
            StopAllCoroutines();
            if (reset)
            {
                ResetTimer();
            }
            StartCoroutine("UpdateTimer");
            _running = true;
            if (OnTimerStart != null)
            {
                OnTimerStart();
            }
        }

        /// <summary>
        /// Pause the timer
        /// </summary>
        public void PauseTimer()
        {
            StopAllCoroutines();
            _running = false;
        }

        /// <summary>
        /// Stops and resets the timer
        /// </summary>
        public void StopTimer()
        {
            StopAllCoroutines();
            _running = false;
            ResetTimer();
        }

        /// <summary>
        /// Resets the timer and reinitializes the event list
        /// </summary>
        public void ResetTimer()
        {
            _value = 0.0f;
            UpdateNormalisedValue();
        }
        #endregion Timer Control
    }
}
