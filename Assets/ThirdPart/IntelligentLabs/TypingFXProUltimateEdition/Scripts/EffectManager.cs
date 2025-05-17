using UnityEngine;
using TMPro;
using System.Collections;

namespace TypingFXPro
{
    public class EffectManager : MonoBehaviour
    {
        public void ApplyEffect(string fullText, TextMeshProUGUI uiText,
        TextDisplay.EffectType effectType, float effectDuration, float hurricaneRadius, float zoomScale, float blinkIntensity,
        float fadeInDuration, float initialSpeed, float speedIncreaseRate, float minScaleCinematic,
        float maxScaleCinematic, float alphaValue, float minScaleBlurNFade, float maxScaleBlurNFade,
        float speedControl, float startDelay, float typingDuration, float typingEntryDuration, float bounceDuration,
        float bounceScale, float delayPerCharacter)
        {
            // Reset text properties before applying any effect
            ResetTextProperties(uiText);

            switch (effectType)
            {
                case TextDisplay.EffectType.Typewriter:
                    StartCoroutine(TypewriterEffect(fullText, uiText, effectDuration));
                    break;

                case TextDisplay.EffectType.FadeIn:
                    StartCoroutine(FadeInEffect(fullText, uiText, effectDuration));
                    break;

                case TextDisplay.EffectType.FadeOut:
                    StartCoroutine(FadeOutEffect(fullText, uiText, effectDuration));
                    break;

                case TextDisplay.EffectType.Hurricane:
                    StartCoroutine(HurricaneEffect(fullText, uiText, effectDuration, hurricaneRadius));
                    break;

                case TextDisplay.EffectType.ZoomIn:
                    StartCoroutine(ZoomInEffect(fullText, uiText, effectDuration));
                    break;

                case TextDisplay.EffectType.ZoomOut:
                    StartCoroutine(ZoomOutEffect(fullText, uiText, GetComponent<TextDisplay>().zoomOutDuration));
                    break;

                case TextDisplay.EffectType.SlideIn:
                    StartCoroutine(SlideInEffect(fullText, uiText, effectDuration, GetComponent<TextDisplay>().slideDirection));
                    break;

                case TextDisplay.EffectType.Jitter:
                    StartCoroutine(JitterEffect(fullText, uiText, effectDuration, GetComponent<TextDisplay>().jitterAmount));
                    break;

                case TextDisplay.EffectType.Glitch:
                    StartCoroutine(GlitchEffect(fullText, uiText, GetComponent<TextDisplay>().glitchDuration, GetComponent<TextDisplay>().glitchIntensity));
                    break;

                case TextDisplay.EffectType.BlurIn:
                    StartCoroutine(BlurInEffect(fullText, uiText, GetComponent<TextDisplay>().blurInDuration));
                    break;

                case TextDisplay.EffectType.Ripple:
                    StartCoroutine(RippleEffect(fullText, uiText, GetComponent<TextDisplay>().rippleDuration));
                    break;

                case TextDisplay.EffectType.GradientFade:
                    StartCoroutine(GradientFadeEffect(fullText, uiText, GetComponent<TextDisplay>().colorDuration));
                    break;

                case TextDisplay.EffectType.LightSweep:
                    StartCoroutine(LightSweepEffect(fullText, uiText, effectDuration));
                    break;

                case TextDisplay.EffectType.FlashReveal:
                    StartCoroutine(FlashRevealEffect(fullText, uiText, effectDuration));
                    break;

                case TextDisplay.EffectType.Shimmer:
                    StartCoroutine(ShimmerEffect(fullText, uiText, effectDuration, GetComponent<TextDisplay>().shimmerColor, GetComponent<TextDisplay>().shimmerSpeed));
                    break;

                case TextDisplay.EffectType.CinematicDistortion:
                    StartCoroutine(CinematicDistortionEffect(fullText, uiText, effectDuration, zoomScale, blinkIntensity, fadeInDuration, initialSpeed, speedIncreaseRate, minScaleCinematic, maxScaleCinematic));
                    break;

                case TextDisplay.EffectType.MeteorShower:
                    StartCoroutine(MeteorShowerEffect(fullText, uiText, effectDuration, blinkIntensity, alphaValue));
                    break;

                case TextDisplay.EffectType.BlurNFall:
                    StartCoroutine(BlurNFallEffect(fullText, uiText, effectDuration, zoomScale, blinkIntensity, fadeInDuration, initialSpeed, speedIncreaseRate, minScaleBlurNFade, maxScaleBlurNFade));
                    break;

                case TextDisplay.EffectType.Storm:
                    StartCoroutine(StormEffect(fullText, uiText, effectDuration, speedControl, startDelay));
                    break;

                case TextDisplay.EffectType.TypingEntries:
                    StartCoroutine(TypingEntriesEffect(fullText, uiText, typingEntryDuration, bounceDuration, bounceScale, delayPerCharacter));
                    break;

                case TextDisplay.EffectType.Rainbow:
                    StartCoroutine(RainbowEffect(fullText, uiText, typingDuration));
                    break;
            }
        }

        private void ResetTextProperties(TextMeshProUGUI uiText)
        {
            // Reset scale and other properties to defaults
            uiText.transform.localScale = Vector3.one;
            uiText.enabled = true;

            uiText.color = Color.white;
            StopAllCoroutines();
        }

        // Typewriter Effect
        private IEnumerator TypewriterEffect(string fullText, TextMeshProUGUI uiText, float typingDuration)
        {
            uiText.text = "";
            float delayPerCharacter = typingDuration / fullText.Length;

            foreach (char c in fullText.ToCharArray())
            {
                uiText.text += c;
                yield return new WaitForSeconds(delayPerCharacter);
            }
        }

        // Fade In Effect
        private IEnumerator FadeInEffect(string fullText, TextMeshProUGUI uiText, float fadeDuration)
        {
            uiText.text = fullText;
            Color originalColor = uiText.color;
            Color fadeColor = originalColor;
            fadeColor.a = 0;
            uiText.color = fadeColor;

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                fadeColor.a = alpha;
                uiText.color = fadeColor;
                yield return null;
            }

            uiText.color = originalColor;
        }

        // Fade Out Effect
        private IEnumerator FadeOutEffect(string fullText, TextMeshProUGUI uiText, float fadeDuration)
        {
            uiText.text = fullText;
            Color originalColor = uiText.color;

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
                originalColor.a = alpha;
                uiText.color = originalColor;
                yield return null;
            }

            originalColor.a = 0f;
            uiText.color = originalColor;
        }

        // Hurricane Effect
        private IEnumerator HurricaneEffect(string fullText, TextMeshProUGUI uiText, float effectDuration, float hurricaneRadius)
        {
            uiText.text = fullText;

            float elapsedTime = 0f;

            uiText.ForceMeshUpdate();
            TMP_TextInfo textInfo = uiText.textInfo;

            Vector3[][] originalVertices = new Vector3[textInfo.meshInfo.Length][];
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                originalVertices[i] = new Vector3[textInfo.meshInfo[i].vertices.Length];
                for (int j = 0; j < originalVertices[i].Length; j++)
                {
                    originalVertices[i][j] = textInfo.meshInfo[i].vertices[j];
                }
            }

            // Animate the characters in a circular tornado-like motion
            while (elapsedTime < effectDuration)
            {
                elapsedTime += Time.deltaTime;
                float angle = elapsedTime * 1f;  // Adjust the speed of rotation

                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    if (!textInfo.characterInfo[i].isVisible) continue;

                    Vector3[] vertices = textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].vertices;
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                    // Circular (hurricane-like) motion
                    float radius = hurricaneRadius + Mathf.Sin(elapsedTime + i) * 0.1f;  // Use hurricaneRadius here
                    float xOffset = Mathf.Cos(angle + i) * radius;
                    float yOffset = Mathf.Sin(angle + i) * radius;

                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] += new Vector3(xOffset, yOffset, 0);
                    }
                }

                // Update the mesh with the new vertex positions
                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    uiText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }

                yield return null;
            }

            // Smoothly return characters to their original positions (optional reset logic)
            float returnDuration = 5.0f;  // Time for returning to original positions
            elapsedTime = 0f;

            while (elapsedTime < returnDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / returnDuration;  // Normalized time for interpolation

                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    if (!textInfo.characterInfo[i].isVisible) continue;

                    Vector3[] vertices = textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].vertices;
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                    // Lerp from current position back to original position
                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] = Vector3.Lerp(vertices[vertexIndex + j], originalVertices[textInfo.characterInfo[i].materialReferenceIndex][vertexIndex + j], t);
                    }
                }

                // Update the mesh again
                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    uiText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }

                yield return null;
            }

            // Ensure everything is back to its original position at the end
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;

                Vector3[] vertices = textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].vertices;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                for (int j = 0; j < 4; j++)
                {
                    vertices[vertexIndex + j] = originalVertices[textInfo.characterInfo[i].materialReferenceIndex][vertexIndex + j];
                }
            }

            // Final update to the mesh
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                uiText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
        }


        // Zoom In Effect
        private IEnumerator ZoomInEffect(string fullText, TextMeshProUGUI uiText, float zoomDuration)
        {
            uiText.text = fullText;
            uiText.transform.localScale = new Vector3(0.1f, 0.1f, 1f);

            float elapsedTime = 0f;
            while (elapsedTime < zoomDuration)
            {
                elapsedTime += Time.deltaTime;
                float scale = Mathf.Lerp(0.1f, 1f, elapsedTime / zoomDuration);
                uiText.transform.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }

            uiText.transform.localScale = Vector3.one;
        }

        // Zoom Out Effect
        private IEnumerator ZoomOutEffect(string fullText, TextMeshProUGUI uiText, float zoomOutDuration)
        {
            uiText.text = fullText;
            uiText.transform.localScale = Vector3.one;

            float elapsedTime = 0f;
            while (elapsedTime < zoomOutDuration)
            {
                elapsedTime += Time.deltaTime;
                float scale = Mathf.Lerp(1f, 0f, elapsedTime / zoomOutDuration);
                uiText.transform.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }

            uiText.transform.localScale = Vector3.zero;
        }

        // Slide In Effect
        private IEnumerator SlideInEffect(string fullText, TextMeshProUGUI uiText, float slideDuration, TextDisplay.SlideDirection direction)
        {
            uiText.text = fullText;
            Vector3 originalPosition = uiText.transform.localPosition;

            Vector3 startPosition = Vector3.zero;
            RectTransform rectTransform = uiText.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                switch (direction)
                {
                    case TextDisplay.SlideDirection.FromTop:
                        startPosition = new Vector3(originalPosition.x, Screen.height, originalPosition.z);
                        break;
                    case TextDisplay.SlideDirection.FromLeft:
                        startPosition = new Vector3(-Screen.width, originalPosition.y, originalPosition.z);
                        break;
                    case TextDisplay.SlideDirection.FromRight:
                        startPosition = new Vector3(Screen.width, originalPosition.y, originalPosition.z);
                        break;
                    case TextDisplay.SlideDirection.FromBottom:
                        startPosition = new Vector3(originalPosition.x, -Screen.height, originalPosition.z);
                        break;
                }
            }

            uiText.transform.localPosition = startPosition;

            float elapsedTime = 0f;
            while (elapsedTime < slideDuration)
            {
                elapsedTime += Time.deltaTime;
                uiText.transform.localPosition = Vector3.Lerp(startPosition, originalPosition, elapsedTime / slideDuration);
                yield return null;
            }

            uiText.transform.localPosition = originalPosition;
        }

        // Jitter Effect
        private IEnumerator JitterEffect(string fullText, TextMeshProUGUI uiText, float jitterDuration, float jitterAmount)
        {
            uiText.text = fullText;
            uiText.ForceMeshUpdate();
            TMP_TextInfo textInfo = uiText.textInfo;

            Vector3[][] originalVertices = new Vector3[textInfo.meshInfo.Length][];
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                originalVertices[i] = new Vector3[textInfo.meshInfo[i].vertices.Length];
                for (int j = 0; j < originalVertices[i].Length; j++)
                {
                    originalVertices[i][j] = textInfo.meshInfo[i].vertices[j];
                }
            }

            float elapsedTime = 0f;

            while (elapsedTime < jitterDuration)
            {
                elapsedTime += Time.deltaTime;

                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    if (!textInfo.characterInfo[i].isVisible) continue;

                    Vector3[] vertices = textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].vertices;
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                    Vector3 jitterOffset = new Vector3(Random.Range(-jitterAmount, jitterAmount), Random.Range(-jitterAmount, jitterAmount), 0);

                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] += jitterOffset;
                    }
                }

                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    uiText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }

                yield return null;
            }

            float smoothDuration = 2.0f;
            float smoothElapsedTime = 0f;

            while (smoothElapsedTime < smoothDuration)
            {
                smoothElapsedTime += Time.deltaTime;
                float t = smoothElapsedTime / smoothDuration;

                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    if (!textInfo.characterInfo[i].isVisible) continue;

                    Vector3[] vertices = textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].vertices;
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] = Vector3.Lerp(vertices[vertexIndex + j], originalVertices[textInfo.characterInfo[i].materialReferenceIndex][vertexIndex + j], t);
                    }
                }

                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    uiText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }

                yield return null;
            }

            uiText.ForceMeshUpdate();
        }

        // Glitch Effect
        private IEnumerator GlitchEffect(string fullText, TextMeshProUGUI uiText, float glitchDuration, float glitchIntensity)
        {
            uiText.text = fullText;

            float elapsedTime = 0f;
            while (elapsedTime < glitchDuration)
            {
                elapsedTime += Time.deltaTime;
                if (Random.value < glitchIntensity)
                {
                    uiText.enabled = !uiText.enabled;  // Random flicker
                }
                yield return null;
            }

            uiText.enabled = true;
        }

        // Blur In Effect
        private IEnumerator BlurInEffect(string fullText, TextMeshProUGUI uiText, float duration)
        {
            uiText.text = fullText;
            Material material = uiText.fontSharedMaterial;

            float startBlurAmount = GetComponent<TextDisplay>().startBlurAmount;
            material.SetFloat(ShaderUtilities.ID_FaceDilate, startBlurAmount);

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                float currentBlur = Mathf.Lerp(startBlurAmount, 0f, t);
                material.SetFloat(ShaderUtilities.ID_FaceDilate, currentBlur);
                yield return null;
            }

            material.SetFloat(ShaderUtilities.ID_FaceDilate, 0f);
        }

        // Ripple Effect
        private IEnumerator RippleEffect(string fullText, TextMeshProUGUI uiText, float rippleDuration)
        {
            uiText.text = fullText;
            uiText.ForceMeshUpdate();
            TMP_TextInfo textInfo = uiText.textInfo;
            Vector3[][] originalVertices = new Vector3[textInfo.meshInfo.Length][];

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                originalVertices[i] = new Vector3[textInfo.meshInfo[i].vertices.Length];
                for (int j = 0; j < originalVertices[i].Length; j++)
                {
                    originalVertices[i][j] = textInfo.meshInfo[i].vertices[j];
                }
            }

            float elapsedTime = 0f;
            while (elapsedTime < rippleDuration)
            {
                elapsedTime += Time.deltaTime;
                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    if (!textInfo.characterInfo[i].isVisible) continue;

                    Vector3[] vertices = textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].vertices;
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                    float wave = Mathf.Sin((elapsedTime * 2f) + i) * 10f;
                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] += new Vector3(0, wave, 0);
                    }
                }

                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    uiText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }

                yield return null;
            }

            // Smoothly return to original positions
            elapsedTime = 0f;
            float returnCharsDuration = GetComponent<TextDisplay>().returnCharsDuration;
            while (elapsedTime < returnCharsDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / returnCharsDuration;

                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    if (!textInfo.characterInfo[i].isVisible) continue;

                    Vector3[] vertices = textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].vertices;
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] = Vector3.Lerp(vertices[vertexIndex + j], originalVertices[textInfo.characterInfo[i].materialReferenceIndex][vertexIndex + j], t);
                    }
                }

                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    uiText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }

                yield return null;
            }

            uiText.ForceMeshUpdate();
        }

        // Gradient Fade Effect
        private IEnumerator GradientFadeEffect(string fullText, TextMeshProUGUI uiText, float colorDuration)
        {
            uiText.text = fullText;
            Color currentColor = Color.white;

            while (true)
            {
                Color newColor = new Color(Random.value, Random.value, Random.value);
                float elapsedTime = 0f;

                while (elapsedTime < colorDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / colorDuration;
                    uiText.color = Color.Lerp(currentColor, newColor, t);
                    yield return null;
                }

                currentColor = newColor;

                // Stop if another effect is selected (handled by ResetTextProperties)
                if (uiText.color == Color.white)
                {
                    yield break;  // Exit the loop if reset condition occurs
                }
            }
        }

        // Light Sweep Effect
        private IEnumerator LightSweepEffect(string fullText, TextMeshProUGUI uiText, float sweepDuration)
        {
            uiText.text = fullText;
            Material material = uiText.fontSharedMaterial;
            material.EnableKeyword("UNDERLAY_ON");
            material.SetColor(ShaderUtilities.ID_UnderlayColor, Color.white);

            float elapsedTime = 0f;
            while (elapsedTime < sweepDuration)
            {
                elapsedTime += Time.deltaTime;
                float offset = Mathf.PingPong(elapsedTime * 2, 1);
                material.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, offset * 10);
                yield return null;
            }

            material.DisableKeyword("UNDERLAY_ON");
            material.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0);
        }

        // Flash Reveal Effect
        private IEnumerator FlashRevealEffect(string fullText, TextMeshProUGUI uiText, float flashDuration)
        {
            uiText.text = fullText;
            float elapsedTime = 0f;

            while (elapsedTime < flashDuration)
            {
                elapsedTime += Time.deltaTime;
                float flash = Mathf.PingPong(elapsedTime * 10, 1);
                uiText.color = Color.Lerp(Color.clear, Color.white, flash);
                yield return null;
            }

            uiText.color = Color.white;
        }

        // Shimmer Effect
        private IEnumerator ShimmerEffect(string fullText, TextMeshProUGUI uiText, float shimmerDuration, Color shimmerColor, float shimmerSpeed)
        {
            uiText.text = fullText;
            float elapsedTime = 0f;

            // Save the original color of the text
            Color originalColor = uiText.color;

            while (elapsedTime < shimmerDuration)
            {
                elapsedTime += Time.deltaTime;
                float shimmer = Mathf.PingPong(elapsedTime * shimmerSpeed, 1);
                uiText.color = Color.Lerp(originalColor, shimmerColor, shimmer);
                yield return null;
            }

            uiText.color = originalColor;
        }

        // Cinematic Distortion Effect
        private IEnumerator CinematicDistortionEffect(string fullText, TextMeshProUGUI uiText, float effectDuration, float zoomScale, float blinkIntensity, float fadeInDuration, float initialSpeed, float speedIncreaseRate, float minScale, float maxScale)
        {
            uiText.text = fullText;
            uiText.ForceMeshUpdate();
            TMP_TextInfo textInfo = uiText.textInfo;

            Vector3[] initialScales = new Vector3[fullText.Length];
            float[] fadeTimers = new float[fullText.Length];
            bool[] blinks = new bool[fullText.Length];
            bool[] isKilled = new bool[fullText.Length];

            // Initialize scales and fade timers for each letter using minScale and maxScale
            for (int i = 0; i < fullText.Length; i++)
            {
                initialScales[i] = Vector3.one * Random.Range(minScale, maxScale) / 2;
                fadeTimers[i] = Random.Range(0f, fadeInDuration);
                blinks[i] = false;
                isKilled[i] = false;
            }

            float elapsedTime = 0f;
            float speedFactor = initialSpeed;
            float initialSlowDuration = 0.5f;

            // Start fading out (2 seconds before the effect ends)
            float fadeOutStartTime = effectDuration - 2.0f;

            while (elapsedTime < effectDuration)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime < initialSlowDuration)
                {
                    speedFactor = Mathf.Lerp(1f, initialSpeed, elapsedTime / initialSlowDuration);
                }
                else
                {
                    speedFactor += speedIncreaseRate * Time.deltaTime;
                }

                for (int i = 0; i < fullText.Length; i++)
                {
                    if (isKilled[i]) continue;

                    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                    if (!charInfo.isVisible) continue;

                    int materialIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;

                    Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                    Color32[] newVertexColors = textInfo.meshInfo[materialIndex].colors32;

                    // If the effect is in the final 2 seconds, start fading out the alpha of the letters
                    float alpha;
                    if (elapsedTime >= fadeOutStartTime)
                    {
                        float fadeOutTime = Mathf.Clamp01((effectDuration - elapsedTime) / 2.0f); // Normalize fade out time over 2 seconds
                        alpha = Mathf.Lerp(0, 255, fadeOutTime);
                    }
                    else
                    {
                        alpha = Mathf.Lerp(0, 255, Mathf.PingPong(elapsedTime + fadeTimers[i], fadeInDuration) / fadeInDuration);
                    }

                    for (int j = 0; j < 4; j++)
                    {
                        newVertexColors[vertexIndex + j].a = (byte)alpha;
                    }

                    // Apply individual zooms with speed control
                    Vector3 scale = Vector3.Lerp(initialScales[i], Vector3.one * zoomScale, Mathf.PingPong(elapsedTime * speedFactor, effectDuration) / effectDuration);
                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] = (vertices[vertexIndex + j] - new Vector3(charInfo.origin, 0, 0)) * scale.x + new Vector3(charInfo.origin, 0, 0);
                    }

                    Vector3 letterPosition = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 8;
                    if (letterPosition.y < -Screen.height || letterPosition.x < -Screen.width || letterPosition.x > Screen.width)
                    {
                        isKilled[i] = true;
                        for (int j = 0; j < 4; j++)
                        {
                            newVertexColors[vertexIndex + j].a = 0;
                        }
                        continue;
                    }

                    if (Mathf.PingPong(elapsedTime * blinkIntensity, 1f) > 0.9f && !blinks[i])
                    {
                        blinks[i] = true;
                        for (int j = 0; j < 4; j++)
                        {
                            newVertexColors[vertexIndex + j].a = 0;
                        }
                    }
                    else if (Mathf.PingPong(elapsedTime * blinkIntensity, 1f) <= 0.9f && blinks[i])
                    {
                        blinks[i] = false;
                    }

                    textInfo.meshInfo[materialIndex].mesh.vertices = vertices;
                    textInfo.meshInfo[materialIndex].mesh.colors32 = newVertexColors;
                    uiText.UpdateGeometry(textInfo.meshInfo[materialIndex].mesh, materialIndex);
                }

                yield return null;
            }

            // After the effect, move letters off-screen
            for (int i = 0; i < fullText.Length; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                Vector3 letterPosition = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2;
                if (letterPosition.y >= -Screen.height && letterPosition.x >= -Screen.width && letterPosition.x <= Screen.width)
                {
                    Vector3 offScreenPosition = new Vector3(Random.Range(-Screen.width, Screen.width), -Screen.height - 200, 0);
                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] = offScreenPosition;
                    }
                    textInfo.meshInfo[materialIndex].mesh.vertices = vertices;
                    uiText.UpdateGeometry(textInfo.meshInfo[materialIndex].mesh, materialIndex);
                }
            }
        }

        // Meteor Shower Effect
        private IEnumerator MeteorShowerEffect(string fullText, TextMeshProUGUI uiText, float fallDuration, float blinkIntensity, float alphaValue)
        {
            uiText.text = fullText;
            uiText.ForceMeshUpdate();
            TMP_TextInfo textInfo = uiText.textInfo;

            Vector3[] originalPositions = new Vector3[textInfo.characterCount];
            Vector3[] startingPositions = new Vector3[textInfo.characterCount];
            float[] randomSpeeds = new float[textInfo.characterCount];
            float[] blinkTimers = new float[textInfo.characterCount]; // Randomized timers for independent blinking
            bool[] blinkStates = new bool[textInfo.characterCount]; // Store the blink states for each letter

            // Set up random initial positions, speeds, and blink timers for each letter
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                // Save the original positions
                originalPositions[i] = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2;

                // Set random starting positions slightly above the screen and random horizontal offsets
                startingPositions[i] = new Vector3(Random.Range(-Screen.width, Screen.width), Random.Range(Screen.height, Screen.height * 0.15f), 0);

                // Assign random speed to each letter
                randomSpeeds[i] = Random.Range(1.0f, 2.5f); // Randomize speed for each letter

                // Initialize the blink timer with a random start time for each letter
                blinkTimers[i] = Random.Range(0f, 1f); // Each letter blinks at different times
                blinkStates[i] = false;
            }

            float elapsedTime = 0f;

            // Random shooting effect loop where each letter flies to its original position from a random start position
            while (elapsedTime < fallDuration)
            {
                elapsedTime += Time.deltaTime;

                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                    if (!charInfo.isVisible) continue;

                    int materialIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;

                    Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                    // Move each letter towards its original position at a random speed
                    Vector3 newPosition = Vector3.Lerp(startingPositions[i], originalPositions[i], Mathf.Clamp01(elapsedTime / (fallDuration / randomSpeeds[i])));
                    Vector3 offset = newPosition - ((vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2);

                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] += offset;
                    }

                    // Independent blinking logic for each letter based on blinkIntensity and the random blinkTimers
                    blinkTimers[i] += Time.deltaTime * blinkIntensity;
                    Color32[] newVertexColors = textInfo.meshInfo[materialIndex].colors32;

                    if (Mathf.PingPong(blinkTimers[i], 1f) > 0.5f && !blinkStates[i])
                    {
                        blinkStates[i] = true; // Blink out (invisible)
                        for (int j = 0; j < 4; j++)
                        {
                            newVertexColors[vertexIndex + j].a = (byte)alphaValue;  // Set alpha to the Inspector-controlled value
                        }
                    }
                    else if (Mathf.PingPong(blinkTimers[i], 1f) <= 0.5f && blinkStates[i])
                    {
                        blinkStates[i] = false; // Blink in (visible)
                        for (int j = 0; j < 4; j++)
                        {
                            newVertexColors[vertexIndex + j].a = 255;  // Set alpha to 255 (fully visible)
                        }
                    }

                    textInfo.meshInfo[materialIndex].mesh.vertices = vertices;
                    textInfo.meshInfo[materialIndex].mesh.colors32 = newVertexColors;
                    uiText.UpdateGeometry(textInfo.meshInfo[materialIndex].mesh, materialIndex);
                }

                yield return null;
            }

            // Ensure the letters stay in their final positions (with no further movement)
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                // Move each letter precisely to its final position (to avoid any minor offset)
                Vector3 finalPosition = originalPositions[i] - ((vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2);
                for (int j = 0; j < 4; j++)
                {
                    vertices[vertexIndex + j] += finalPosition;
                }

                // Ensure letters are fully visible at the end
                Color32[] finalVertexColors = textInfo.meshInfo[materialIndex].colors32;
                for (int j = 0; j < 4; j++)
                {
                    finalVertexColors[vertexIndex + j].a = 255;  // Set alpha to fully visible
                }

                textInfo.meshInfo[materialIndex].mesh.vertices = vertices;
                textInfo.meshInfo[materialIndex].mesh.colors32 = finalVertexColors;
                uiText.UpdateGeometry(textInfo.meshInfo[materialIndex].mesh, materialIndex);
            }
        }

        // Blur N Fall Effect
        private IEnumerator BlurNFallEffect(string fullText, TextMeshProUGUI uiText, float effectDuration, float zoomScale, float blinkIntensity, float fadeInDuration, float initialSpeed, float speedIncreaseRate, float minScale, float maxScale)
        {
            uiText.text = fullText;
            uiText.ForceMeshUpdate();
            TMP_TextInfo textInfo = uiText.textInfo;

            Vector3[] initialScales = new Vector3[fullText.Length];
            float[] fadeTimers = new float[fullText.Length];
            bool[] blinks = new bool[fullText.Length];
            bool[] isKilled = new bool[fullText.Length];

            // Initialize scales and fade timers for each letter using minScale and maxScale
            for (int i = 0; i < fullText.Length; i++)
            {
                initialScales[i] = Vector3.one * Random.Range(minScale, maxScale) / 2;
                fadeTimers[i] = Random.Range(0f, fadeInDuration);
                blinks[i] = false;
                isKilled[i] = false;
            }

            float elapsedTime = 0f;
            float speedFactor = initialSpeed;
            float initialSlowDuration = 0.5f;

            // Start fading out (2 seconds before the effect ends)
            float fadeOutStartTime = effectDuration - 2.0f;

            while (elapsedTime < effectDuration)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime < initialSlowDuration)
                {
                    speedFactor = Mathf.Lerp(1f, initialSpeed, elapsedTime / initialSlowDuration);
                }
                else
                {
                    speedFactor += speedIncreaseRate * Time.deltaTime;
                }

                for (int i = 0; i < fullText.Length; i++)
                {
                    if (isKilled[i]) continue;

                    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                    if (!charInfo.isVisible) continue;

                    int materialIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;

                    Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                    Color32[] newVertexColors = textInfo.meshInfo[materialIndex].colors32;

                    // If the effect is in the final 2 seconds, start fading out the alpha of the letters
                    float alpha;
                    if (elapsedTime >= fadeOutStartTime)
                    {
                        float fadeOutTime = Mathf.Clamp01((effectDuration - elapsedTime) / 2.0f); // Normalize fade out time over 2 seconds
                        alpha = Mathf.Lerp(0, 255, fadeOutTime);
                    }
                    else
                    {
                        alpha = Mathf.Lerp(0, 255, Mathf.PingPong(elapsedTime + fadeTimers[i], fadeInDuration) / fadeInDuration);
                    }

                    for (int j = 0; j < 4; j++)
                    {
                        newVertexColors[vertexIndex + j].a = (byte)alpha;
                    }

                    // Apply individual zooms with speed control
                    Vector3 scale = Vector3.Lerp(initialScales[i], Vector3.one * zoomScale, Mathf.PingPong(elapsedTime * speedFactor, effectDuration) / effectDuration);
                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] = (vertices[vertexIndex + j] - new Vector3(charInfo.origin, 0, 0)) * scale.x + new Vector3(charInfo.origin, 0, 0);
                    }

                    Vector3 letterPosition = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 8;
                    if (letterPosition.y < -Screen.height || letterPosition.x < -Screen.width || letterPosition.x > Screen.width)
                    {
                        isKilled[i] = true;
                        for (int j = 0; j < 4; j++)
                        {
                            newVertexColors[vertexIndex + j].a = 0;
                        }
                        continue;
                    }

                    if (Mathf.PingPong(elapsedTime * blinkIntensity, 1f) > 0.9f && !blinks[i])
                    {
                        blinks[i] = true;
                        for (int j = 0; j < 4; j++)
                        {
                            newVertexColors[vertexIndex + j].a = 0;
                        }
                    }
                    else if (Mathf.PingPong(elapsedTime * blinkIntensity, 1f) <= 0.9f && blinks[i])
                    {
                        blinks[i] = false;
                    }

                    textInfo.meshInfo[materialIndex].mesh.vertices = vertices;
                    textInfo.meshInfo[materialIndex].mesh.colors32 = newVertexColors;
                    uiText.UpdateGeometry(textInfo.meshInfo[materialIndex].mesh, materialIndex);
                }

                yield return null;
            }

            // After the effect, move letters off-screen
            for (int i = 0; i < fullText.Length; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                Vector3 letterPosition = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2;
                if (letterPosition.y >= -Screen.height && letterPosition.x >= -Screen.width && letterPosition.x <= Screen.width)
                {
                    Vector3 offScreenPosition = new Vector3(Random.Range(-Screen.width, Screen.width), -Screen.height - 200, 0);
                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] = offScreenPosition;
                    }
                    textInfo.meshInfo[materialIndex].mesh.vertices = vertices;
                    uiText.UpdateGeometry(textInfo.meshInfo[materialIndex].mesh, materialIndex);
                }
            }
        }

        // Storm Effect
        private IEnumerator StormEffect(string fullText, TextMeshProUGUI uiText, float effectDuration, float entrySpeed, float startDelay)
        {
            // Set the text to the full text to ensure it's visible
            uiText.text = fullText;
            uiText.ForceMeshUpdate();
            TMP_TextInfo textInfo = uiText.textInfo;

            // Cache the character count
            int characterCount = textInfo.characterCount;

            // Precompute positions and delays
            Vector3[] startPositions = new Vector3[characterCount];
            Vector3[] targetPositions = new Vector3[characterCount];
            float[] delays = new float[characterCount];

            // Initialize the starting positions offscreen and set target positions for each character
            for (int i = 0; i < characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                // Set random offscreen starting positions closer to the screen edges
                startPositions[i] = new Vector3(
                    Random.Range(-Screen.width * 1.5f, Screen.width * 1.5f),
                    Random.Range(-Screen.height * 3.5f, Screen.height * 3.5f),
                    0);

                // Set target positions based on character bounds instead of origin/baseline
                targetPositions[i] = (textInfo.meshInfo[charInfo.materialReferenceIndex].vertices[charInfo.vertexIndex] +
                                      textInfo.meshInfo[charInfo.materialReferenceIndex].vertices[charInfo.vertexIndex + 2]) / 2;

                // Set a random delay for each letter's entry
                delays[i] = Random.Range(0f, startDelay);
            }

            float elapsedTime = 0f;

            // Perform updates in batches rather than per frame
            while (elapsedTime < effectDuration)
            {
                elapsedTime += Time.deltaTime;

                for (int i = 0; i < characterCount; i++)
                {
                    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                    if (!charInfo.isVisible || elapsedTime < delays[i]) continue;

                    int materialIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;

                    Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                    // Smoothly move each letter from its starting position to the target position
                    Vector3 position = Vector3.Lerp(startPositions[i], targetPositions[i], Mathf.Clamp01((elapsedTime - delays[i]) * entrySpeed));

                    // Calculate the displacement and apply it to all four vertices of the character
                    Vector3 displacement = position - (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2;
                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] += displacement;
                    }

                    // Update the mesh for the current character
                    textInfo.meshInfo[materialIndex].mesh.vertices = vertices;
                    uiText.UpdateGeometry(textInfo.meshInfo[materialIndex].mesh, materialIndex);
                }

                yield return null;
            }

            // Force final update to ensure everything is positioned correctly
            uiText.ForceMeshUpdate();
        }

        // Typing Entries Effect with Bounce Animation for Each Character
        private IEnumerator TypingEntriesEffect(string fullText, TextMeshProUGUI uiText, float typingEntryDuration,
                                                float bounceDuration, float bounceScale, float delayPerCharacter)
        {
            // Clear the text before starting the effect
            uiText.text = "";

            // Loop through each character in the full text
            for (int i = 0; i < fullText.Length; i++)
            {
                // Append the character to the text
                uiText.text += fullText[i];

                // Start the bounce effect for the last character
                StartCoroutine(BounceCharacter(uiText, i, bounceDuration, bounceScale));

                // Wait for the delayPerCharacter before typing the next character
                yield return new WaitForSeconds(delayPerCharacter);
            }
        }

        // Bounce animation for each character
        private IEnumerator BounceCharacter(TextMeshProUGUI uiText, int index, float bounceDuration, float bounceScale)
        {
            float originalScale = 1f;

            // Cache the original text to access each character
            uiText.ForceMeshUpdate();
            var textInfo = uiText.textInfo;

            // Bounce the character by increasing and decreasing its scale
            for (float t = 0; t < bounceDuration; t += Time.deltaTime)
            {
                float scale = Mathf.Lerp(originalScale, bounceScale, t / bounceDuration);  // Scale up
                ModifyCharacterScale(uiText, index, scale);
                yield return null;
            }

            // Scale back to the original size
            ModifyCharacterScale(uiText, index, originalScale);
        }

        // Helper method to modify the scale of a specific character
        private void ModifyCharacterScale(TextMeshProUGUI uiText, int charIndex, float scale)
        {
            uiText.ForceMeshUpdate();
            var textInfo = uiText.textInfo;

            if (charIndex < textInfo.characterCount)
            {
                var charInfo = textInfo.characterInfo[charIndex];
                if (charInfo.isVisible)
                {
                    var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                    Vector3 charMidBaseline = (verts[charInfo.vertexIndex + 0] + verts[charInfo.vertexIndex + 2]) / 2;

                    // Scale the character
                    for (int i = 0; i < 4; i++)
                    {
                        verts[charInfo.vertexIndex + i] = (verts[charInfo.vertexIndex + i] - charMidBaseline) * scale + charMidBaseline;
                    }
                    uiText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                }
            }
        }

        // Rainbow Effect
        private IEnumerator RainbowEffect(string fullText, TextMeshProUGUI uiText, float typingDuration)
        {
            // Clear the text before starting the effect
            uiText.text = "";

            // Calculate how much time to wait between each character
            float delayPerCharacter = typingDuration / fullText.Length;

            // Loop through each character in the full text
            foreach (char letter in fullText.ToCharArray())
            {
                uiText.text += $"<color=#{RandomColor()}>{letter}</color>";  // Append the character with a random color
                yield return new WaitForSeconds(delayPerCharacter);  // Wait before typing the next character
            }
        }

        // Helper method to generate a random hex color
        private string RandomColor()
        {
            Color color = new Color(Random.value, Random.value, Random.value);
            return ColorUtility.ToHtmlStringRGB(color); // Convert to hex string
        }

    }
}