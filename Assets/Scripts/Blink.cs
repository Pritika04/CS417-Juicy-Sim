using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkSimple : MonoBehaviour
{
    public Transform eye;
    public List<Renderer> eyeRenderers;

    public float minInterval = 2f;
    public float maxInterval = 5f;

    public float closeDuration = 0.08f;
    public float openDuration = 0.08f;

    public float minClosedWait = 0.05f;
    public float maxClosedWait = 0.1f;

    private Vector3 originalScale;

    void Start()
    {
        if (eye == null) eye = transform;

        originalScale = eye.localScale;
        StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            yield return StartCoroutine(LerpY(1f, 0f, closeDuration));

            foreach (Renderer eyeRenderer in eyeRenderers)
            {
                eyeRenderer.enabled = false;
            }

            yield return new WaitForSeconds(Random.Range(minClosedWait, maxClosedWait));

            foreach (Renderer eyeRenderer in eyeRenderers)
            {
                eyeRenderer.enabled = true;
            }

            yield return StartCoroutine(LerpY(0f, 1f, openDuration));
        }
    }

    IEnumerator LerpY(float from, float to, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);

            Vector3 s = originalScale;
            s.y = originalScale.y * Mathf.Lerp(from, to, p);
            eye.localScale = s;

            yield return null;
        }

        Vector3 final = originalScale;
        final.y = originalScale.y * to;
        eye.localScale = final;
    }
}