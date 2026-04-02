using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using System;

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

    [Header("Feature: Prestige")]
    public int prestigeLevel = 0;
    public float prestigeMultiplier = 1.0f;
    public Button prestigeButton;
    public ParticleSystem prestigeParticles;
    public AudioSource prestigeSound;
    private string lastSaveTimeKey = "LastSaveTime";
    public TextMeshProUGUI prestigeCountText;

    void Start() {
        LoadGame();

        seedsUI.SetActive(unlockSeedsButtonShown);
        genUI.SetActive(packsTextShown);
        unlockSeedsButton.gameObject.SetActive(!unlockSeedsButtonShown);
        buyGenButton.gameObject.SetActive(unlockSeedsButtonShown);
        buyPowerUpButton.gameObject.SetActive(generatorUnlocked);

        CalculateOfflineGains();
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
        if (prestigeButton && !prestigeButton.gameObject.activeSelf && seeds >= 1000)
        {
            prestigeButton.gameObject.SetActive(true);
            unlockParticles.Emit(30);
            popup.Show("The path to Prestige has opened...");
            SendHaptic(1f, 5);
            eyeGrow.Grow(2.5f);
            SaveGame();
        }

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
            SaveGame();
        }

        if (!generatorUnlocked && seedsUI.activeSelf && water >= unlockCost && seeds >= 5)
        {
            buyGenButton.gameObject.SetActive(true);
            generatorUnlocked = true;
            unlockParticles.Emit(30);
            unlockSound.Play();
            popup.Show("Seed Generator unlocked!");
            eyeGrow.Grow(2.5f);
            SaveGame();
        }
        
        if (generatorUnlocked && !buyPowerUpButton.gameObject.activeSelf && seeds >= 10)
        {
            buyPowerUpButton.gameObject.SetActive(true);
            unlockParticles.Emit(30);
            unlockSound.Play();
            popup.Show("Fertilizer unlocked - Buy and Power up!");
            eyeGrow.Grow(2.5f);
            SaveGame();
        }
    }

    void Update()
    {
        water += waterRate * Time.deltaTime;
        seeds += (seedRate * multiplier * prestigeMultiplier) * Time.deltaTime;

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
            SendHaptic(0.3f, 3);
        }

        if (buyGenButton.gameObject.activeSelf) {
            bool canBuy = water >= genCost;
            buyGenButton.interactable = canBuy;
            buyGenButton.image.color = canBuy ? Color.green : Color.gray;
            SendHaptic(0.3f, 3);
        }

        if (buyPowerUpButton.gameObject.activeSelf) {
            bool canAfford = seeds >= powerUpCost;
            buyPowerUpButton.interactable = canAfford;
            buyPowerUpButton.image.color = canAfford ? Color.green : Color.gray;
            SendHaptic(0.3f, 3);
        }

        if (prestigeButton.gameObject.activeSelf) {
            bool canAfford = seeds >= 1000;
            prestigeButton.interactable = canAfford;
            prestigeButton.image.color = canAfford ? Color.green : Color.gray;
            SendHaptic(0.3f, 3);
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
                    UnityEngine.Random.Range(-spawnRadius, spawnRadius),
                    0,
                    UnityEngine.Random.Range(-spawnRadius, spawnRadius)
                );
            Vector3 spawnPos = spawnPoint.position + randomOffset;
            GameObject newPack = Instantiate(seedPacketPrefab, spawnPos, Quaternion.identity);
            StartCoroutine(GrowEase(newPack.transform));

            if (unlockSound != null) unlockSound.Play();
            unlockSeedsButton.gameObject.SetActive(false);
        }

        SaveGame();
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
        SaveGame();
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
                UnityEngine.Random.Range(-spawnRadius, spawnRadius),
                0,
                UnityEngine.Random.Range(-spawnRadius, spawnRadius)
            );

            Vector3 spawnPos = spawnPoint.position + randomOffset;

            GameObject newPack = Instantiate(seedPacketPrefab, spawnPos, Quaternion.identity);
            StartCoroutine(GrowEase(newPack.transform));
            SendHaptic(0.3f, 3);
            seedSound.Play();
            seedParticles.Emit(50);
        }
        SaveGame();
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

    public void SaveGame()
    {
        PlayerPrefs.SetFloat("Water", water);
        PlayerPrefs.SetFloat("Seeds", seeds);
        PlayerPrefs.SetInt("TotalGenerators", totalGenerators);
        PlayerPrefs.SetFloat("Multiplier", multiplier);
        PlayerPrefs.SetInt("PrestigeLevel", prestigeLevel);
        PlayerPrefs.SetFloat("PrestigeMultiplier", prestigeMultiplier);

        PlayerPrefs.SetInt("UnlockSeedsShown", unlockSeedsButtonShown ? 1 : 0);
        PlayerPrefs.SetInt("GeneratorUnlocked", generatorUnlocked ? 1 : 0);
        PlayerPrefs.SetInt("PacksTextShown", packsTextShown ? 1 : 0);

        PlayerPrefs.SetString(lastSaveTimeKey, DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        water = PlayerPrefs.GetFloat("Water", 0);
        seeds = PlayerPrefs.GetFloat("Seeds", 0);
        totalGenerators = PlayerPrefs.GetInt("TotalGenerators", 0);
        multiplier = PlayerPrefs.GetFloat("Multiplier", 1.0f);
        prestigeLevel = PlayerPrefs.GetInt("PrestigeLevel", 0);
        prestigeMultiplier = PlayerPrefs.GetFloat("PrestigeMultiplier", 1.0f);

        unlockSeedsButtonShown = PlayerPrefs.GetInt("UnlockSeedsShown", 0) == 1;
        generatorUnlocked = PlayerPrefs.GetInt("GeneratorUnlocked", 0) == 1;
        packsTextShown = PlayerPrefs.GetInt("PacksTextShown", 0) == 1;

        seedRate = totalGenerators * 0.5f;
        genCountText.text = "Packs: " + totalGenerators;
        prestigeCountText.text = "Prestige: " + prestigeLevel;
        seedText.text = "Seeds: " + Mathf.FloorToInt(seeds);
    }

    public void CalculateOfflineGains()
    {
        if (PlayerPrefs.HasKey(lastSaveTimeKey))
        {
            DateTime lastTime = DateTime.Parse(PlayerPrefs.GetString(lastSaveTimeKey));
            TimeSpan ts = DateTime.Now - lastTime;
            float secondsAway = (float)ts.TotalSeconds;

            float waterGained = waterRate * secondsAway;
            float seedsGained = (seedRate * multiplier * prestigeMultiplier) * secondsAway;

            water += waterGained;
            seeds += seedsGained;

            if (waterGained > 0 || seedsGained > 0)
            {
                popup.Show($"Welcome back! You earned {Mathf.FloorToInt(waterGained)} Water while away.");
                waterParticles.Emit(30);
                SendHaptic(0.5f, 0.5f);
            }
        }
    }

    public void PerformPrestige()
    {
        prestigeLevel++;
        prestigeMultiplier += 0.5f; 
        Debug.Log("Prestige Level: " + prestigeLevel);

        water = 0;
        seeds = 0;
        totalGenerators = 0;
        seedRate = 0;        
        waterRate = 1.0f; 
        multiplier = 1.0f;      
        waterTimer = 0;        
        seedTimer = 0;          
        packsTextShown = false; 
        unlockSeedsButtonShown = false;
        generatorUnlocked = false;

        prestigeCountText.text = "Prestige: " + prestigeLevel;

        waterText.text = "Water: " + Mathf.FloorToInt(water);
        seedText.text = "Seeds: " + Mathf.FloorToInt(seeds);
        genCountText.text = "Packs: " + totalGenerators;
        
        seedsUI.SetActive(false);
        genUI.SetActive(false);
        buyGenButton.gameObject.SetActive(false);
        buyPowerUpButton.gameObject.SetActive(false);
        prestigeButton.gameObject.SetActive(false);
        unlockSeedsButton.gameObject.SetActive(true);

        Trophies trophyScript = FindFirstObjectByType<Trophies>();
        if (trophyScript != null)
        {
            trophyScript.ResetTrophies();
        }

        if (prestigeParticles != null) prestigeParticles.Play();
        if (prestigeSound != null) prestigeSound.Play();
        SendHaptic(1.0f, 1.0f);
        
        SaveGame();
    }

    private void OnApplicationQuit() { 
        SaveGame(); 
    }
    private void OnApplicationPause(bool pause) { 
        if (pause) SaveGame(); 
    }
}