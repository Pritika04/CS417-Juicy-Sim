using UnityEngine;

public class Trophies : MonoBehaviour
{
    public FarmManager farmManager; 

    [Header("Trophy Objects")]
    public GameObject object1;
    public GameObject object2;
    public GameObject object3;

    [Header("Thresholds")]
    public float score1 = 20;
    public float score2 = 120;
    public float score3 = 520;

    [Header("Juicy Feedback")]
    public ParticleSystem trophyParticles;
    public AudioSource trophySound;

    private bool object1Shown = false;
    private bool object2Shown = false;
    private bool object3Shown = false;

    void Start()
    {
        if (object1 != null) object1.SetActive(false);
        if (object2 != null) object2.SetActive(false);
        if (object3 != null) object3.SetActive(false);
    }

    void Update()
    {
        float currentScore = farmManager.seeds;

        if (currentScore < 1 && (object1Shown || object2Shown || object3Shown))
        {
            ResetTrophies();
            return; 
        }

        if (!object1Shown && currentScore >= score1)
        {
            ShowTrophy(object1);
            object1Shown = true;
        }

        if (!object2Shown && currentScore >= score2)
        {
            ShowTrophy(object2);
            object2Shown = true;
        }

        if (!object3Shown && currentScore >= score3)
        {
            ShowTrophy(object3);
            object3Shown = true;
        }
    }

    void ShowTrophy(GameObject trophy)
    {
        if (trophy == null) return;
        
        trophy.SetActive(true);
        trophyParticles.Emit(50);
        trophySound.Play();
        
        Debug.Log(trophy.name + " appeared with Juice!");
    }

    public void ResetTrophies()
    {
        if (object1) object1.SetActive(false);
        if (object2) object2.SetActive(false);
        if (object3) object3.SetActive(false);
        object1Shown = false;
        object2Shown = false;
        object3Shown = false;
    }
}