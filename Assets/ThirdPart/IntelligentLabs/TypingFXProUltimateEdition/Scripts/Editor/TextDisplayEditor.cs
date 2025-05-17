using TMPro;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace TypingFXPro
{
    [CustomEditor(typeof(TextDisplay))]
    public class TextDisplayEditor : Editor
    {
        private const string megaPackUrl = "https://assetstore.unity.com/packages/tools/game-toolkits/mega-pack-9-in-1-collection-308347";
        private Texture2D upgradeImage;

        void OnEnable()
        {
            upgradeImage = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/IntelligentLabs/TypingFXProUltimateEdition/Sprites/UpgradeCard.png", typeof(Texture2D));
        }

        public override void OnInspectorGUI()
        {
            TextDisplay textDisplay = (TextDisplay)target;

            textDisplay.fullText = EditorGUILayout.TextArea(textDisplay.fullText, GUILayout.Height(75));
            textDisplay.uiText = (TextMeshProUGUI)EditorGUILayout.ObjectField("UI Text", textDisplay.uiText, typeof(TextMeshProUGUI), true);

            // Sorting the EffectType enum values alphabetically
            var effectNames = System.Enum.GetNames(typeof(TextDisplay.EffectType)).OrderBy(n => n).ToArray();
            var currentEffectIndex = System.Array.IndexOf(effectNames, textDisplay.effectType.ToString());

            // Display dropdown with sorted enum values
            currentEffectIndex = EditorGUILayout.Popup("Effect Type", currentEffectIndex, effectNames);

            // Update the selected EffectType based on the sorted list
            textDisplay.effectType = (TextDisplay.EffectType)System.Enum.Parse(typeof(TextDisplay.EffectType), effectNames[currentEffectIndex]);

            // Apply effect-specific settings
            switch (textDisplay.effectType)
            {
                case TextDisplay.EffectType.Typewriter:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Typewriter Setup:", EditorStyles.boldLabel);
                    textDisplay.startDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.startDelay);
                    textDisplay.typingDuration = EditorGUILayout.FloatField("Typing Duration", textDisplay.typingDuration);
                    break;

                case TextDisplay.EffectType.FadeIn:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Fade In Setup:", EditorStyles.boldLabel);
                    textDisplay.startDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.startDelay);
                    textDisplay.fadeInDuration = EditorGUILayout.FloatField("Duration", textDisplay.fadeInDuration);
                    break;

                case TextDisplay.EffectType.FadeOut:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Fade Out Setup:", EditorStyles.boldLabel);
                    textDisplay.startDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.startDelay);
                    textDisplay.fadeOutDuration = EditorGUILayout.FloatField("Duration", textDisplay.fadeOutDuration);
                    break;

                case TextDisplay.EffectType.ZoomIn:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Zoom In Setup:", EditorStyles.boldLabel);
                    textDisplay.zoomInStartDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.zoomInStartDelay);
                    textDisplay.zoomInDuration = EditorGUILayout.FloatField("Duration", textDisplay.zoomInDuration);
                    break;

                case TextDisplay.EffectType.ZoomOut:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Zoom Out Setup:", EditorStyles.boldLabel);
                    textDisplay.zoomOutStartDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.zoomOutStartDelay);
                    textDisplay.zoomOutDuration = EditorGUILayout.FloatField("Duration", textDisplay.zoomOutDuration);
                    break;

                case TextDisplay.EffectType.SlideIn:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Slide In Setup:", EditorStyles.boldLabel);
                    textDisplay.slideInStartDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.slideInStartDelay);
                    textDisplay.slideDirection = (TextDisplay.SlideDirection)EditorGUILayout.EnumPopup("Slide Direction", textDisplay.slideDirection);
                    textDisplay.slideInDuration = EditorGUILayout.FloatField("Duration", textDisplay.slideInDuration);
                    break;

                case TextDisplay.EffectType.Jitter:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Jitter Setup:", EditorStyles.boldLabel);
                    textDisplay.startDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.startDelay);
                    textDisplay.jitterDuration = EditorGUILayout.FloatField("Duration", textDisplay.jitterDuration);
                    textDisplay.jitterAmount = EditorGUILayout.FloatField("Amount", textDisplay.jitterAmount);
                    break;

                case TextDisplay.EffectType.Glitch:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Glitch Setup:", EditorStyles.boldLabel);
                    textDisplay.startDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.startDelay);
                    textDisplay.glitchDuration = EditorGUILayout.FloatField("Duration", textDisplay.glitchDuration);
                    textDisplay.glitchIntensity = EditorGUILayout.FloatField("Intensity", textDisplay.glitchIntensity);
                    break;

                case TextDisplay.EffectType.BlurIn:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Blur In Setup:", EditorStyles.boldLabel);
                    textDisplay.startDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.startDelay);
                    textDisplay.blurInDuration = EditorGUILayout.FloatField("Duration", textDisplay.blurInDuration);
                    textDisplay.startBlurAmount = EditorGUILayout.FloatField("Start Blur Amount", textDisplay.startBlurAmount);
                    break;

                case TextDisplay.EffectType.Ripple:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Ripple Setup:", EditorStyles.boldLabel);
                    textDisplay.startDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.startDelay);
                    textDisplay.rippleDuration = EditorGUILayout.FloatField("Duration", textDisplay.rippleDuration);
                    textDisplay.returnCharsDuration = EditorGUILayout.FloatField("Return Chars Duration", textDisplay.returnCharsDuration);
                    break;

                case TextDisplay.EffectType.Hurricane:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Hurricane Setup:", EditorStyles.boldLabel);
                    textDisplay.hurricaneStartDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.hurricaneStartDelay);
                    textDisplay.hurricaneDuration = EditorGUILayout.FloatField("Duration", textDisplay.hurricaneDuration);
                    textDisplay.hurricaneRadius = EditorGUILayout.FloatField("Radius", textDisplay.hurricaneRadius);
                    break;

                case TextDisplay.EffectType.GradientFade:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Gradient Fade Setup:", EditorStyles.boldLabel);
                    textDisplay.colorStartDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.colorStartDelay);
                    textDisplay.colorDuration = EditorGUILayout.FloatField("Color Duration", textDisplay.colorDuration);
                    break;

                case TextDisplay.EffectType.LightSweep:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Light Sweep Setup:", EditorStyles.boldLabel);
                    textDisplay.sweepStartDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.sweepStartDelay);
                    textDisplay.sweepDuration = EditorGUILayout.FloatField("Duration", textDisplay.sweepDuration);
                    break;

                case TextDisplay.EffectType.FlashReveal:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Flash Reveal Setup:", EditorStyles.boldLabel);
                    textDisplay.flashStartDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.flashStartDelay);
                    textDisplay.flashDuration = EditorGUILayout.FloatField("Duration", textDisplay.flashDuration);
                    break;

                case TextDisplay.EffectType.Shimmer:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Shimmer Setup:", EditorStyles.boldLabel);
                    textDisplay.shimmerStartDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.shimmerStartDelay);
                    textDisplay.shimmerColor = EditorGUILayout.ColorField("Color", textDisplay.shimmerColor);
                    textDisplay.shimmerSpeed = EditorGUILayout.FloatField("Speed", textDisplay.shimmerSpeed);
                    break;

                case TextDisplay.EffectType.CinematicDistortion:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Cinematic Distortion Setup:", EditorStyles.boldLabel);
                    textDisplay.startDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.startDelay);
                    textDisplay.typingDuration = EditorGUILayout.FloatField("Duration", textDisplay.typingDuration);
                    textDisplay.zoomScale = EditorGUILayout.FloatField("Zoom Scale", textDisplay.zoomScale);
                    textDisplay.blinkIntensity = EditorGUILayout.FloatField("Blink Intensity", textDisplay.blinkIntensity);
                    textDisplay.minScaleCinematic = EditorGUILayout.FloatField("Min Scale", textDisplay.minScaleCinematic);
                    textDisplay.maxScaleCinematic = EditorGUILayout.FloatField("Max Scale", textDisplay.maxScaleCinematic);
                    break;

                case TextDisplay.EffectType.MeteorShower:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Meteor Shower Setup:", EditorStyles.boldLabel);
                    textDisplay.startDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.startDelay);
                    textDisplay.fallDuration = EditorGUILayout.FloatField("Fall Duration", textDisplay.fallDuration);
                    textDisplay.alphaIntensity = EditorGUILayout.FloatField("Alpha Intensity (0-255)", textDisplay.alphaIntensity);
                    break;

                case TextDisplay.EffectType.BlurNFall:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Blur N Fall Setup:", EditorStyles.boldLabel);
                    textDisplay.startDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.startDelay);
                    textDisplay.typingDuration = EditorGUILayout.FloatField("Duration", textDisplay.typingDuration);
                    textDisplay.zoomScale = EditorGUILayout.FloatField("Zoom Scale", textDisplay.zoomScale);
                    textDisplay.blinkIntensity = EditorGUILayout.FloatField("Blink Intensity", textDisplay.blinkIntensity);
                    textDisplay.minScaleBlurNFade = EditorGUILayout.FloatField("Min Scale", textDisplay.minScaleBlurNFade);
                    textDisplay.maxScaleBlurNFade = EditorGUILayout.FloatField("Max Scale", textDisplay.maxScaleBlurNFade);
                    break;

                case TextDisplay.EffectType.Storm:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Storm Setup:", EditorStyles.boldLabel);
                    textDisplay.startDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.startDelay);
                    textDisplay.typingDuration = EditorGUILayout.FloatField("Duration", textDisplay.typingDuration);
                    textDisplay.speedControl = EditorGUILayout.FloatField("Entry Speed", textDisplay.speedControl);
                    break;

                case TextDisplay.EffectType.TypingEntries:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Typing Bounce Setup:", EditorStyles.boldLabel);
                    textDisplay.startDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.startDelay);
                    textDisplay.typingEntryDuration = EditorGUILayout.FloatField("Typing Duration", textDisplay.typingEntryDuration);
                    textDisplay.bounceDuration = EditorGUILayout.FloatField("Bounce Duration", textDisplay.bounceDuration);
                    textDisplay.bounceScale = EditorGUILayout.FloatField("Bounce Scale", textDisplay.bounceScale);
                    textDisplay.delayPerCharacter = EditorGUILayout.FloatField("Delay Per Character", textDisplay.delayPerCharacter);
                    break;

                case TextDisplay.EffectType.Rainbow:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Rainbow Setup:", EditorStyles.boldLabel);
                    textDisplay.startDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.startDelay);
                    textDisplay.typingDuration = EditorGUILayout.FloatField("Typing Duration", textDisplay.typingDuration);
                    break;
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(textDisplay);
            }

            GUILayout.Space(20);

            if (upgradeImage != null)
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();

                GUIStyle whiteBoldLabel = new GUIStyle(EditorStyles.boldLabel);
                whiteBoldLabel.normal.textColor = Color.white;
                whiteBoldLabel.hover.textColor = Color.white;

                GUILayout.Label("Exclusive Upgrade Offer! 50% OFF!", whiteBoldLabel);
                GUILayout.Label("Because you own this asset, you can upgrade to the\nMEGA PACK: 9-IN-1 COLLECTION at 50% OFF!", EditorStyles.label);
                GUILayout.Space(5);
                GUILayout.Label("Click below to claim your upgrade!", EditorStyles.label);

                if (GUILayout.Button(upgradeImage, GUILayout.Width(300), GUILayout.Height(205)))
                {
                    Application.OpenURL(megaPackUrl);
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }
        }
    }
}
