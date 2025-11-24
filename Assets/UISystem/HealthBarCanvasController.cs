using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarCanvasController : MonoBehaviour
{
    public static HealthBarCanvasController Instance { get; private set; }

    [SerializeField] private Image healthBarBackground;
    [SerializeField] private Image healthBar;

    private void Awake()
    {
        if (Instance != null)
            return;


        Instance = this;
    }

    private void Start()
    {
        healthBarBackground.color = new Color(1, 1, 1, 0);
        healthBar.color = new Color(1, 1, 1, 0);
    }

    public void EnableHealthBar()
    {
       StartCoroutine(EnableHealthBarDelay());
    }

    private IEnumerator EnableHealthBarDelay()
    {
        
        yield return new WaitForSeconds(2f);
        healthBarBackground.color = Color.white;
        healthBar.color = Color.white;
    }
}
