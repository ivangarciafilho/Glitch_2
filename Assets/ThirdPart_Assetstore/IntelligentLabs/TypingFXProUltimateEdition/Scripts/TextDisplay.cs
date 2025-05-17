using TMPro;
using TypingFXPro;
using UnityEngine;
using System.Collections;

namespace TypingFXPro
{
    public class TextDisplay : MonoBehaviour
    {
        [TextArea]
        public string fullText;
        public TextMeshProUGUI uiText;

        public float typingDuration = 2.0f;
        public float fadeInDuration = 2.0f;
        public float fadeOutDuration = 2.0f;

        public float zoomInDuration = 2.0f;
        public float zoomInStartDelay = 1.0f;
        public float zoomOutDuration = 2.0f;
        public float zoomOutStartDelay = 1.0f;

        public float jitterDuration = 2.0f;
        public float jitterAmount = 5.0f;
        public float startDelay = 1.0f;

        public float glitchDuration = 2.0f;
        public float glitchIntensity = 0.1f;

        public float blurInDuration = 2.0f;
        public float startBlurAmount = 1.0f;

        public float rippleDuration = 2.0f;
        public float returnCharsDuration = 2.0f;

        public float colorDuration = 3.0f;
        public float colorStartDelay = 1.0f;

        public float sweepDuration = 3.0f;
        public float sweepStartDelay = 1.0f;

        public float flashDuration = 2.0f;
        public float flashStartDelay = 1.0f;

        public Color shimmerColor = Color.white;
        public float shimmerStartDelay = 1.0f;
        public float shimmerSpeed = 5.0f;

        public float hurricaneDuration = 5.0f;
        public float hurricaneStartDelay = 1.0f;
        public float hurricaneRadius = 5.0f;

        public float slideInDuration = 2.0f;
        public float slideInStartDelay = 0.5f;

        public float zoomScale = 1.2f;
        public float blinkIntensity = 0.8f;

        public float initialSpeed = 0.5f;
        public float speedIncreaseRate = 0.1f;
        public float speedControl = 1.0f;

        public float minScaleCinematic = 1.75f;
        public float maxScaleCinematic = 2.25f;

        public float minScaleBlurNFade = 2.0f;
        public float maxScaleBlurNFade = 2.0f;

        public float fallDuration = 3.0f;
        public float alphaIntensity = 80.0f;

        public float typingEntryDuration = 35.0f;
        public float bounceDuration = 0.2f;
        public float bounceScale = 3.0f;
        public float delayPerCharacter = 0.05f;



        public enum EffectType
        {
            Typewriter, FadeIn, FadeOut, ZoomIn, ZoomOut, SlideIn, Jitter, Glitch,
            BlurIn, Ripple, GradientFade, LightSweep, FlashReveal, Shimmer, Hurricane,
            CinematicDistortion, MeteorShower, BlurNFall, Storm, TypingEntries, Rainbow
        }
        public EffectType effectType;

        public enum SlideDirection { FromTop, FromLeft, FromRight, FromBottom }
        public SlideDirection slideDirection;

        private EffectManager effectManager;

        void Start()
        {
            uiText.text = "";
            effectManager = GetComponent<EffectManager>();
            StartCoroutine(DisplayTextWithEffect());
        }

        IEnumerator DisplayTextWithEffect()
        {
            yield return new WaitForSeconds(GetStartDelay());

            effectManager.ApplyEffect(
                fullText, uiText, effectType, GetEffectDuration(), GetHurricaneRadius(),
                zoomScale, blinkIntensity, fadeInDuration, initialSpeed, speedIncreaseRate, minScaleCinematic,
                maxScaleCinematic, alphaIntensity, minScaleBlurNFade, maxScaleBlurNFade, speedControl, startDelay,
                typingDuration, typingEntryDuration, bounceDuration, bounceScale, delayPerCharacter
            );
        }

        private float GetEffectDuration()
        {
            switch (effectType)
            {
                case EffectType.Typewriter:
                    return typingDuration;

                case EffectType.FadeIn:
                    return fadeInDuration;

                case EffectType.FadeOut:
                    return fadeOutDuration;

                case EffectType.ZoomIn:
                    return zoomInDuration;

                case EffectType.ZoomOut:
                    return zoomOutDuration;

                case EffectType.SlideIn:
                    return slideInDuration;

                case EffectType.Jitter:
                    return jitterDuration;

                case EffectType.Glitch:
                    return glitchDuration;

                case EffectType.BlurIn:
                    return blurInDuration;

                case EffectType.Ripple:
                    return rippleDuration;

                case EffectType.GradientFade:
                    return colorDuration;

                case EffectType.LightSweep:
                    return sweepDuration;

                case EffectType.FlashReveal:
                    return flashDuration;

                case EffectType.Shimmer:
                    return 2.0f;

                case EffectType.Hurricane:
                    return hurricaneDuration;

                case EffectType.MeteorShower:
                    return fallDuration;

                default:
                    return typingDuration;
            }
        }

        private float GetStartDelay()
        {
            switch (effectType)
            {
                case EffectType.ZoomIn:
                    return zoomInStartDelay;

                case EffectType.ZoomOut:
                    return zoomOutStartDelay;

                case EffectType.SlideIn:
                    return slideInStartDelay;

                case EffectType.GradientFade:
                    return colorStartDelay;

                case EffectType.LightSweep:
                    return sweepStartDelay;

                case EffectType.FlashReveal:
                    return flashStartDelay;

                case EffectType.Shimmer:
                    return shimmerStartDelay;

                case EffectType.Hurricane:
                    return hurricaneStartDelay;

                default:
                    return startDelay;
            }
        }

        private float GetHurricaneRadius()
        {
            switch (effectType)
            {
                case EffectType.Hurricane:
                    return hurricaneRadius;

                default:
                    return 0f;
            }
        }

        public void TriggerSelectedEffect()
        {
            StopAllCoroutines();
            uiText.text = "";
            StartCoroutine(DisplayTextWithEffect());
        }
    }
}