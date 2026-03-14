using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class FarmManager : MonoBehaviour
{
    [Header("Resource 1: Water")]
    public float water = 0;
    public float waterRate = 1.0f;
    public TextMeshProUGUI waterText;
    public ParticleSystem waterParticles;
    private float waterTimer;

    [Header("Resource 2: Seeds")]
    public float seeds = 0;
    public float seedRate = 0; 
    public TextMeshProUGUI seedText;
    public GameObject seedsUI;
    public ParticleSystem seedParticles;
    private float seedTimer;

    [Header("Feature 4: Unlock UI")]
    public Button unlockSeedsButton;
    public float unlockCost = 20f;
    // public AudioSource unlockSound;

    [Header("Feature 2: Generators")]
    public Button buyGenButton;
    public float genCost = 10f;
    public GameObject seedPacketPrefab;
    public Transform spawnPoint;
    public float spawnRadius = 0.2f;
    public TextMeshProUGUI genCountText;
    private int totalGenerators = 0;

    bool unlockSeedsButtonShown = false;
    bool generatorUnlocked = false;
    bool packsTextShown = false;

    void Start()
    {
        seedsUI.SetActive(false);
        unlockSeedsButton.gameObject.SetActive(false);
        buyGenButton.gameObject.SetActive(false);
        genCountText.gameObject.SetActive(false);
    }

    void CheckProgression()
    {
        if (!unlockSeedsButtonShown && water >= 20)
        {
            unlockSeedsButton.gameObject.SetActive(true);
            unlockSeedsButtonShown = true;
        }

        if (!generatorUnlocked && seedsUI.activeSelf && water >= 20 && seeds >= 5)
        {
            buyGenButton.gameObject.SetActive(true);
            generatorUnlocked = true;
        }
    }

    void Update()
    {
        water += waterRate * Time.deltaTime;
        seeds += seedRate * Time.deltaTime;

        waterText.text = "Water: " + Mathf.FloorToInt(water);
        if (seedsUI.activeSelf) {
            seedText.text = "Seeds: " + Mathf.FloorToInt(seeds);
        }

        CheckProgression();
        HandleParticles();
        UpdateButtons();
    }

    void HandleParticles()
    {
        waterTimer += waterRate * Time.deltaTime;
        if (waterTimer >= 1.0f) {
            waterParticles.Emit(1);
            waterTimer -= 1.0f;
        }

        if (seedRate > 0) {
            seedTimer += seedRate * Time.deltaTime;
            if (seedTimer >= 1.0f) {
                seedParticles.Emit(1);
                seedTimer -= 1.0f;
            }
        }
    }

    void UpdateButtons()
    {
        if (unlockSeedsButton.gameObject.activeSelf) {
            bool canUnlock = water >= unlockCost;
            unlockSeedsButton.interactable = canUnlock;
            unlockSeedsButton.image.color = canUnlock ? Color.green : Color.gray;
        }

        if (buyGenButton.gameObject.activeSelf) {
            bool canBuy = water >= genCost;
            buyGenButton.interactable = canBuy;
            buyGenButton.image.color = canBuy ? Color.green : Color.gray;
        }
    }

    public void UnlockSeeds()
    {
        if (water >= unlockCost) {
            water -= unlockCost;
            seedsUI.SetActive(true);
            seeds = 5;
            // if (unlockSound != null) unlockSound.Play(); -- TODO
            unlockSeedsButton.gameObject.SetActive(false);
        }
    }

    public void BuySeedGenerator()
    {
        if (water >= genCost) {
            water -= genCost;
            totalGenerators++;

            if (!packsTextShown)
            {
                genCountText.gameObject.SetActive(true);
                packsTextShown = true;
            }

            genCountText.text = "Packs: " + totalGenerators;

            seedRate += 0.5f;

            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                0,
                Random.Range(-spawnRadius, spawnRadius)
            );

            Vector3 spawnPos = spawnPoint.position + randomOffset;

            GameObject newPack = Instantiate(seedPacketPrefab, spawnPos, Quaternion.identity);
            StartCoroutine(GrowEase(newPack.transform));
        }
    }

    IEnumerator GrowEase(Transform target)
    {
        float duration = 0.5f;
        float elapsed = 0;
        Vector3 endScale = target.localScale;
        target.localScale = Vector3.zero;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            float s = 1.70158f;
            float ease = (t -= 1) * t * ((s + 1) * t + s) + 1;

            target.localScale = endScale * ease;
            yield return null;
        }
        target.localScale = endScale;
    }
}