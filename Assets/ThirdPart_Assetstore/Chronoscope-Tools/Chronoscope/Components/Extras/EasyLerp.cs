using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChronoscopeTools
{
    [HelpURL("https://github.com/johnmjep/Chronoscope-Tools-Public/wiki/User-Guide.EasyLerp")]
    [AddComponentMenu("Chronoscope Tools/Extras/EasyLerp")]
    public class EasyLerp : MonoBehaviour
    {
        #region Fields
        [Tooltip("Descriptive Name")]
        [SerializeField]
        private string _name = "Linear";
        public string Name { get { return _name; } }
                
        [SerializeField]
        private AnimationCurve _profile = new AnimationCurve(new Keyframe(0, 0, 0.981876f, 0.981876f), new Keyframe(1, 1, 0.981876f, 0.981876f));
        #endregion Fields

        #region Unity Specific Methods
        void Awake()
        {
            _profile.preWrapMode = WrapMode.Clamp;
            _profile.postWrapMode = WrapMode.Clamp;
        }
        #endregion Unity Specific Methods

        #region Lerp Methods
        /// <summary>
        /// Returns the evaluated value from the profile based on the provided alpha
        /// </summary>
        /// <param name="alpha">Provided alpha value (clamped between 0 and 1</param>
        /// <returns>Profiled value</returns>
        public float Apply(float alpha)
        {
            alpha = Mathf.Clamp(alpha, 0.0f, 1.0f);
            return _profile.Evaluate(alpha);
        }

        /// <summary>
        /// Lerps between start and end based on the profile and the provided alpha
        /// </summary>
        /// <param name="start">Start value</param>
        /// <param name="end">End value</param>
        /// <param name="alpha">Alpha value between 0 and 1</param>
        /// <returns>Lerped value</returns>
        public float Apply(float start, float end, float alpha)
        {
            return start + ((end - start) * Apply(alpha));
        }

        /// <summary>
        /// Lerps between start and end based on the profile and the provided alpha
        /// </summary>
        /// <param name="start">Start value</param>
        /// <param name="end">End value</param>
        /// <param name="alpha">Alpha value between 0 and 1</param>
        /// <returns>Lerped value</returns>
        public Vector2 Apply(Vector2 start, Vector2 end, float alpha)
        {
            float x = Apply(start.x, end.x, alpha);
            float y = Apply(start.y, end.y, alpha);

            return new Vector2(x, y);
        }

        /// <summary>
        /// Lerps between start and end based on the profile and the provided alpha
        /// </summary>
        /// <param name="start">Start value</param>
        /// <param name="end">End value</param>
        /// <param name="alpha">Alpha value between 0 and 1</param>
        /// <returns>Lerped value</returns>
        public Vector3 Apply(Vector3 start, Vector3 end, float alpha)
        {
            float x = Apply(start.x, end.x, alpha);
            float y = Apply(start.y, end.y, alpha);
            float z = Apply(start.z, end.z, alpha);

            return new Vector3(x, y, z);
        }
        #endregion Lerp Methods
    } 
}
