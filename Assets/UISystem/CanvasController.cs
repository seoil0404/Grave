using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private List<Image> images;

    public static CanvasController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        images.ForEach(image => image.color = new Color(image.color.r, image.color.g, image.color.b, 0));
    }

    public void EnableCanvas()
    {
        StartCoroutine(EnableHealthBarDelay());
        
    }

    private IEnumerator EnableHealthBarDelay()
    {
        yield return new WaitForSeconds(2f);
        images.ForEach(image => image.color = new Color(image.color.r, image.color.g, image.color.b, 1));
    }
}