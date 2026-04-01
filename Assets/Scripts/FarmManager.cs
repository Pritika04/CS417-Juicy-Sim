using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class FarmManager : MonoBehaviour
{
    public HapticImpulsePlayer hapticPlayer; //haptic. Drag right controller here

    [Header("Resource 1: Water")]
    public float water = 0;
    public float waterRate = 1.0f;
    public TextMeshProUGUI waterText;
    public ParticleSystem waterParticles;
    public AudioSource waterSound;
    public ScalePulse waterScalePulse;
    private float waterTimer;

    [Header("Resource 2: Seeds")]
    public float seeds = 0;
    public float seedRate = 0; 
    public TextMeshProUGUI seedText;
    public GameObject seedsUI;
    public ParticleSystem seedParticles;
    public AudioSource seedSound;
    public ScalePulse seedScalePulse;
    private float seedTimer;

    [Header("Feature 3: Power-ups (Haptics)")]
    public Button buyPowerUpButton;
    public float powerUpCost = 50f;
    public float multiplier = 1.0f;
    public ParticleSystem powerupParticles;
    public AudioSource powerupSound;
    public ScalePulse powerupScalePulse;
    public TMP_Text powerupText;

    [Header("Feature 4: Unlock UI")]
    public Button unlockSeedsButton;
    public float unlockCost = 20f;
    public AudioSource unlockSound;
    public ParticleSystem unlockParticles;
    public ScalePulse unlockScalePulse;
    public TMP_Text unlockText;

    [Header("Feature 2: Generators")]
    public Button buyGenButton;
    public float genCost = 10f;
    public GameObject seedPacketPrefab;
    public Transform spawnPoint;
    public float spawnRadius = 1.0f;
    public TextMeshProUGUI genCountText;
    public GameObject genUI;
    private int totalGenerators = 0;

    [Header("UI")]
    public Popup popup;

    [Header("Eye")]
    public GrowWide eyeGrow;

    bool unlockSeedsButtonShown = false;
    bool generatorUnlocked = false;
    bool packsTextShown = false;

    void Start()
    {
        seedsUI.SetActive(false);
        unlockSeedsButton.gameObject.SetActive(false);
        buyGenButton.gameObject.SetActive(false);
        genUI.SetActive(false);
        buyPowerUpButton.gameObject.SetActive(false);
    }

    void SendHaptic(float amplitude, float duration) {
        if (hapticPlayer != null) {
            hapticPlayer.SendHapticImpulse(amplitude, duration);
        } else {
            Debug.LogWarning("no haptic player");
        }
    }

    void CheckProgression()
    {
        if (!unlockSeedsButtonShown && water >= unlockCost)
        {
            unlockSeedsButton.gameObject.SetActive(true);
            unlockSeedsButtonShown = true;
            unlockScalePulse.PlayTextPulse(unlockText);
            unlockParticles.Emit(30);
            unlockSound.Play();
            popup.Show("Seeds unlocked - Spend some water and plant!");
            SendHaptic(1f, 5);
            eyeGrow.Grow(2.5f);
        }

        if (!generatorUnlocked && seedsUI.activeSelf && water >= unlockCost && seeds >= 5)
        {
            buyGenButton.gameObject.SetActive(true);
            generatorUnlocked = true;
            unlockParticles.Emit(30);
            unlockSound.Play();
            popup.Show("Generator unlocked");
            eyeGrow.Grow(2.5f);
        }
        
        if (generatorUnlocked && !buyPowerUpButton.gameObject.activeSelf && seeds >= 10)
        {
            buyPowerUpButton.gameObject.SetActive(true);
            unlockParticles.Emit(30);
            unlockSound.Play();
            popup.Show("Fertilizer unlocked");
            eyeGrow.Grow(2.5f);
        }
    }

    void Update()
    {
        water += waterRate * Time.deltaTime;
        seeds += (seedRate * multiplier) * Time.deltaTime;

        waterText.text = "Water: " + Mathf.FloorToInt(water);
        if (seedsUI.activeSelf) {
            seedText.text = "Seeds: " + Mathf.FloorToInt(seeds);
        }

        CheckProgression();
        HandleParticlesAndSounds();
        UpdateButtons();
    }

    void HandleParticlesAndSounds()
    {
        waterTimer += waterRate * Time.deltaTime;
        if (waterTimer >= 1.0f) {
            waterParticles.Emit(1);
            waterSound.Play();
            waterScalePulse.PlayScalePulse();
            waterTimer -= 1.0f;
        }

        if (seedRate > 0) {
            seedTimer += seedRate * Time.deltaTime;
            if (seedTimer >= 1.0f) {
                seedParticles.Emit(1);
                seedSound.Play();
                seedScalePulse.PlayScalePulse();
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

        if (buyPowerUpButton.gameObject.activeSelf) {
            bool canAfford = seeds >= powerUpCost;
            buyPowerUpButton.interactable = canAfford;
            buyPowerUpButton.image.color = canAfford ? Color.green : Color.gray;
        }
    }

    public void AddMoreWater(float amount)
    {
        water += amount;
    }

    public void UnlockSeeds()
    {
        if (water >= unlockCost) {
            water -= unlockCost;
            seedsUI.SetActive(true);
            seeds = 5;

            if (seedParticles != null) {
                seedParticles.Emit(20); 
            }

            Vector3 randomOffset = new Vector3(
                    Random.Range(-spawnRadius, spawnRadius),
                    0,
                    Random.Range(-spawnRadius, spawnRadius)
                );
            Vector3 spawnPos = spawnPoint.position + randomOffset;
            GameObject newPack = Instantiate(seedPacketPrefab, spawnPos, Quaternion.identity);
            StartCoroutine(GrowEase(newPack.transform));

            if (unlockSound != null) unlockSound.Play();
            unlockSeedsButton.gameObject.SetActive(false);
        }
    }

    public void BuyPowerUp()
    {
        if (seeds >= powerUpCost)
        {
            seeds -= powerUpCost;
            multiplier *= 2.0f;

            powerupParticles.Emit(50);
            powerupSound.Play();
            powerupScalePulse.PlayTextPulse(powerupText);
            SendHaptic(0.3f, 3);
            Debug.Log("Power-up Purchased! Rate Multiplied.");
        }
    }

    public void BuySeedGenerator()
    {
        if (water >= genCost) {
            water -= genCost;
            totalGenerators++;

            if (!packsTextShown)
            {
                genUI.SetActive(true);
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

            seedSound.Play();
            seedParticles.Emit(50);
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