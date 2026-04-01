using System.Collections;
using UnityEngine;
using TMPro;

public class ScalePulse : MonoBehaviour
{
    [Header("Transform Pulse Settings")]
    [SerializeField] private float scaleMultiplier = 1.2f;
    [SerializeField] private float scaleUpTime = 0.12f;
    [SerializeField] private float scaleDownTime = 0.12f;

    [Header("TMP Text Pulse Settings")]
    [SerializeField] private float textSizeMultiplier = 1.2f;
    [SerializeField] private float textScaleUpTime = 0.12f;
    [SerializeField] private float textScaleDownTime = 0.12f;

    private Vector3 originalScale;
    private Coroutine pulseRoutine;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void PlayScalePulse()
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            transform.localScale = originalScale;
        }

        pulseRoutine = StartCoroutine(PulseRoutine());
    }

    public void PlayTextPulse(TMP_Text tmpText)
    {
        if (tmpText == null) return;
        StartCoroutine(TextPulseRoutine(tmpText));
    }

    private IEnumerator PulseRoutine()
    {
        Vector3 targetScale = originalScale * scaleMultiplier;

        yield return ScaleOverTime(transform.localScale, targetScale, scaleUpTime);
        yield return ScaleOverTime(transform.localScale, originalScale, scaleDownTime);

        transform.localScale = originalScale;
        pulseRoutine = null;
    }

    private IEnumerator TextPulseRoutine(TMP_Text tmpText)
    {
        float originalFontSize = tmpText.fontSize;
        float targetFontSize = originalFontSize * textSizeMultiplier;

        yield return FontSizeOverTime(tmpText, tmpText.fontSize, targetFontSize, textScaleUpTime);
        yield return FontSizeOverTime(tmpText, tmpText.fontSize, originalFontSize, textScaleDownTime);

        tmpText.fontSize = originalFontSize;
    }

    private IEnumerator ScaleOverTime(Vector3 from, Vector3 to, float duration)
    {
        if (duration <= 0f)
        {
            transform.localScale = to;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);

            transform.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }

        transform.localScale = to;
    }

    private IEnumerator FontSizeOverTime(TMP_Text tmpText, float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            tmpText.fontSize = to;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);

            tmpText.fontSize = Mathf.Lerp(from, to, t);
            yield return null;
        }

        tmpText.fontSize = to;
    }
}