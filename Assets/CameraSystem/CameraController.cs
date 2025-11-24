using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CameraController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnStartGame()
    {
        animator.SetTrigger("Start");
    }

    void Update()
    {
        
    }
}
