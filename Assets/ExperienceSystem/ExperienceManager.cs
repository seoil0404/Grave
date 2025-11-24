using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance { get; private set; }

    [Header("Offsets")]
    [SerializeField] private int requiredExperience;

    [Header("MonoBehaviors")]
    [SerializeField] private Image experienceBar;

    [Header("Prefab")]
    [SerializeField] private UpgradeView upgradeViewPrefab;

    private int currentExperience = 0;

    private float targetFillAmount = 0;

    public int CurrentExperience
    {
        get => currentExperience;
        set
        {
            currentExperience = value;
            SyncBar();
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CurrentExperience = 0;
    }

    private void SyncBar()
    {
        targetFillAmount = Mathf.Clamp((float)CurrentExperience / (float)requiredExperience, 0f, 1f);
    }

    private void Update()
    {
        experienceBar.fillAmount = Mathf.Lerp(experienceBar.fillAmount, targetFillAmount, 0.1f);
    }

    public void AddExperience(int experience = 1)
    {
        CurrentExperience++;
        if(CurrentExperience >= requiredExperience)
        {
            CurrentExperience %= requiredExperience;
            OnUpgrade();
        }
    }

    private void OnUpgrade()
    {
        requiredExperience++;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Instantiate(upgradeViewPrefab);
    }

    public void OnReStart()
    {
        CurrentExperience = 0;
    }
}
