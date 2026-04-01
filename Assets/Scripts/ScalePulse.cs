using System.Collections;
using UnityEngine;

public class ScalePulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] private float scaleMultiplier = 1.2f;
    [SerializeField] private float scaleUpTime = 0.12f;
    [SerializeField] private float scaleDownTime = 0.12f;

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

    private IEnumerator PulseRoutine()
    {
        Vector3 targetScale = originalScale * scaleMultiplier;

        yield return ScaleOverTime(transform.localScale, targetScale, scaleUpTime);
        yield return ScaleOverTime(transform.localScale, originalScale, scaleDownTime);

        transform.localScale = originalScale;
        pulseRoutine = null;
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
}