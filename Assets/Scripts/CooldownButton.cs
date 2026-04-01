using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class CooldownButton : MonoBehaviour
{
    [Header("References")]
    public Image buttonImage;
    public FarmManager gameManager;
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable;

    [Header("Settings")]
    public float waterAmount = 10f;
    public float cooldownTime = 3f;

    [Header("Colors")]
    public Color readyColor = new Color(0f, 0.5f, 1f, 1f);
    public Color cooldownColor = Color.red;

    private bool isCoolingDown = false;

    public ParticleSystem waterParticles;
    public GrowWide eyeGrow;

    public AudioSource audioWaterFilled;

    private void Start()
    {
        if (buttonImage != null)
            buttonImage.color = readyColor;
    }

    public void OnSelectEntered(SelectEnterEventArgs args)
    {

        if (isCoolingDown) return;
        if (gameManager == null) return;

        gameManager.AddMoreWater(waterAmount);
        waterParticles.Emit(10);
        // eyeGrow.Grow(2f);
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        isCoolingDown = true;

        if (interactable != null)
            interactable.enabled = false;

        if (buttonImage != null)
        {
            buttonImage.color = cooldownColor;
            Debug.Log("Set cooldown color to: " + buttonImage.color);
        }

        float elapsed = 0f;

        while (elapsed < cooldownTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / cooldownTime);

            if (buttonImage != null)
                buttonImage.color = Color.Lerp(cooldownColor, readyColor, t);

            yield return null;
        }

        if (buttonImage != null)
            buttonImage.color = readyColor;

        if (interactable != null)
            interactable.enabled = true;

        isCoolingDown = false;

        if (audioWaterFilled != null)
            audioWaterFilled.Play();
    }
}