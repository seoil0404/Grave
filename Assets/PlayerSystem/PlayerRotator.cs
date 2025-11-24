using Unity.VisualScripting;
using UnityEngine;

public class PlayerRotator : MonoBehaviour
{
    [SerializeField] private float sensitivity = 100f;

    private float yRotation = 0f;

    private void Start()
    {
        
    }

    private void Update()
    {
        if(PlayerController.PlayerState == PlayerState.Dead || PlayerController.PlayerState == PlayerState.Start || PlayerController.PlayerState == PlayerState.Cinematic)
            return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        yRotation += mouseX;

        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
