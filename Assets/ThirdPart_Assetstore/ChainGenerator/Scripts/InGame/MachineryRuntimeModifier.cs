using System;
using System.Collections;
using System.Collections.Generic;
using Chain;
using ChainInGame;
using UnityEngine;
using UnityEngine.Events;


namespace ChainInGame
{
    public static class MachineryRuntimeEvents
    {
        public static Action<float> OnRuntimeSpeedChangeRequest;
        public static Action OnMotionStartRequest;
        public static Action OnMotionStopRequest;
    }
    public class MachineryRuntimeModifier : MonoBehaviour
    {
        private MachineryInGame _runTimeMachinery;
    
        public float runTimeSpeed = 20f;

        private void Awake()
        {
            _runTimeMachinery = new MachineryInGame(GetComponentInParent<Machinery>());
        }

        private void OnEnable() //You can call these events to trigger runtime actions on machinery
        {
            MachineryRuntimeEvents.OnRuntimeSpeedChangeRequest += ChangeSpeedAtRunTime;
            MachineryRuntimeEvents.OnMotionStartRequest += StartMotion;
            MachineryRuntimeEvents.OnMotionStopRequest += StopMotion;
        }

        public void ChangeSpeedAtRunTime(float speed)
        {
            runTimeSpeed = speed;
            _runTimeMachinery.SetSpeedAtRuntime(runTimeSpeed);
        }
    
        public void StartMotion()
        {
            _runTimeMachinery.StartMotion();
        }
    
        public void StopMotion()
        {
            _runTimeMachinery.StopMotion();
        }

        private void OnDisable()
        {
            MachineryRuntimeEvents.OnRuntimeSpeedChangeRequest -= ChangeSpeedAtRunTime;
            MachineryRuntimeEvents.OnMotionStartRequest -= StartMotion;
            MachineryRuntimeEvents.OnMotionStopRequest -= StopMotion;
        }
    }

}

