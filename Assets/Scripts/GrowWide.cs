using System.Collections;
using UnityEngine;

public class GrowWide : MonoBehaviour
{
    public float duration = 0.15f;

    private Vector3 originalScale;
    private Coroutine routine;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Grow(float targetScale)
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(GrowRoutine(targetScale));
    }

    IEnumerator GrowRoutine(float targetScale)
    {
        float t = 0f;

        Vector3 target = new Vector3(
            originalScale.x * targetScale,
            originalScale.y * targetScale,
            originalScale.z
        );

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;

            transform.localScale = Vector3.Lerp(originalScale, target, p);
            yield return null;
        }

        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;

            transform.localScale = Vector3.Lerp(target, originalScale, p);
            yield return null;
        }

        transform.localScale = originalScale;
    }
}