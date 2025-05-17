using System;
using System.Collections;
using System.Collections.Generic;
using ChainInGame;
using UnityEngine;

namespace Chain
{
    public enum MotionState
    {
        Moving,
        Pause,
        Stop
    }

    public class ChainMover : MonoBehaviour, Mover
    {
        public float MachinerySpeed { get; set; }
        public int MachineryId { get; set; }
        public ChainEnums.ChainDirection MachineryDirection { get; set; }

        public ChainData Data;

        public AlteredState AlteredState;
        public OngoingState OngoingState;
        private BaseMovingChainState _currentState;

        public int _cogAmount;

        [SerializeField] private List<ChainLink> _links = new();
        [SerializeField] private List<Vector3> _points = new();
        private List<Quaternion> _rotations = new();
        
        public float LinearSpeed = 0;
        private float _rotationExtentPerLink;
        private float _speed;
        
        private List<Coroutine> _runningCoroutines = new List<Coroutine>();
        public bool pause;
        
        private void CreateStates()
        {
            if (AlteredState != null && OngoingState != null) return;
            AlteredState = new AlteredState(this);
            OngoingState = new OngoingState(this);
        }

        public void MachinerySetup(float machinerySpeed, int machineryId, IMachinePartData machinePartData,
            ChainEnums.ChainDirection direction)
        {
            MachinerySpeed = machinerySpeed;
            MachineryId = machineryId;
            Data = machinePartData as ChainData;
            MachineryDirection = direction;
        }

        public void Setup(List<ChainLink> links, int cogAmount)
        {
            CreateStates();
            SwitchState(AlteredState);
            _links = links;
            _cogAmount = cogAmount;
        }

        public float PrepareSpeedForChain() => 0;

        public void SetLinearSpeed(float totalCogSpeed)
        {
            LinearSpeed = totalCogSpeed / _cogAmount / _links.Count; // * 1.3f; 
        }
        public void SetCoroutineSpeed()
        {
            _speed = Data.SetMotionByGear ? LinearSpeed * Data.SpeedMultiplier : Data.SpeedMultiplier;
            _rotationExtentPerLink = _speed * Data.LinkRotationMultiplier;
        }

        void ResetPointsAndRotations()
        {
            _points.Clear();
            _rotations.Clear();
            foreach (var link in _links)
            {
                _rotations.Add(link.transform.localRotation);
                _points.Add(link.transform.localPosition);
            }
        }

        public IEnumerator MoveRoutine()
        {
            if (!Data.IsMoving) yield break;

            MoveChain();
        }

        void MoveChain()
        {
            if (Data.motionDirection == ChainEnums.ChainDirection.None)
            {
                Debug.LogWarning("Motion Direction is set to None");
                return;
            }

            ResetPointsAndRotations();
            SetCoroutineSpeed();

            for (int i = 0; i < _links.Count; i++)
            {
                Coroutine coroutine = StartCoroutine(LinkMotionRoutine(i));
                _runningCoroutines.Add(coroutine);
            }
        }

        IEnumerator LinkMotionRoutine(int Index)
        {
            int pointIndex = Index;

            while (true)
            {
                switch (Data.motionDirection)
                {
                    case ChainEnums.ChainDirection.Clockwise:
                        pointIndex++;
                        pointIndex %= _points.Count;
                        break;
                    case ChainEnums.ChainDirection.ReverseClock:
                        pointIndex--;
                        if (pointIndex < 0)
                            pointIndex = _points.Count - 1;
                        break;
                }

                while (Vector3.Distance(_links[Index].transform.localPosition, _points[pointIndex]) >
                       Data.LinkLagAmount)
                {
                    if (pause) yield return new WaitWhile(() => pause);

                    _links[Index].transform.localPosition = Vector3.MoveTowards(
                        _links[Index].transform.localPosition,
                        _points[pointIndex], _speed);

                    _links[Index].transform.localRotation = Quaternion.Slerp(
                        _links[Index].transform.localRotation,
                        _rotations[pointIndex], _rotationExtentPerLink);

                    yield return new WaitForFixedUpdate();
                }

                if (!pause)
                {
                    _links[Index].transform.localPosition = _points[pointIndex];
                }
            }
        }

        public void SwitchState(BaseMovingChainState newMovingChainState)
        {
            _currentState = newMovingChainState;
            _currentState.EnterState();
        }

        public void StartMotion()
        {
            _currentState.StartMotion();
        }

        public void StopLinkRoutines()
        {
            _runningCoroutines.ForEach(StopCoroutine);
            _runningCoroutines.Clear();
        }

        public void StopMotion()
        {
            _currentState.StopMotion();
        }
    }
}