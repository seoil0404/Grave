using UnityEngine;

public class UpgradeView : MonoBehaviour
{
    public void Destroy()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Destroy(gameObject);
    }
}
