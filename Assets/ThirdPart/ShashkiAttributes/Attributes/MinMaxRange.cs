using System;

using UnityEngine;

namespace Shashki.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    [RequireAttribute(typeof(SerializeField))]
    public sealed class MinMaxRangeAttribute : PropertyAttribute
    {
        public readonly float Min;
        public readonly float Max;

        public MinMaxRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }

    [Serializable]
    public struct MinMaxPair
    {
        public float Min, Max;
        public float RandomValue => UnityEngine.Random.Range(Min, Max);
        public float Length => Max - Min;

        public MinMaxPair(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public float Clamp(float value) => Mathf.Clamp(value, Min, Max);

        public float Lerp(float t) => Mathf.Lerp(Min, Max, t);
        public float LerpUnclamped(float t) => Mathf.LerpUnclamped(Min, Max, t);
        public float InverseLerp(float t) => Mathf.InverseLerp(Min, Max, t);

        public float InterpolateRange(float t) => Mathf.MoveTowards(Min, Max, t);
        public float InverseInterpolateRange(float t) => Mathf.MoveTowards(Max, Min, t);

        public float SmoothStep(float t) => Mathf.SmoothStep(Min, Max, t);

        public float Repeat(float t, float lengthMultiplier = 1) => Mathf.Repeat(t, Length * lengthMultiplier) + Min;
        public float PingPong(float t)
        {
            t = Repeat(t, 2);
            return Length - Mathf.Abs(t - Max) + Min;
        }
    }
}
