using System.Collections;
using UnityEngine;
using TMPro;

public class Popup : MonoBehaviour
{
    public TextMeshProUGUI text;

    public float fadeInTime = 0.3f;
    public float holdTime = 2f;
    public float fadeOutTime = 0.5f;

    private Coroutine routine;

    void Awake()
    {
        SetAlpha(0f);
    }

    public void Show(string msg)
    {
        text.text = msg;
        routine = StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            SetAlpha(t / fadeInTime);
            yield return null;
        }

        SetAlpha(1f);

        yield return new WaitForSeconds(holdTime);

        t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            SetAlpha(1f - t / fadeOutTime);
            yield return null;
        }

        SetAlpha(0f);
    }

    void SetAlpha(float a)
    {
        Color c = text.color;
        c.a = a;
        text.color = c;
    }
}