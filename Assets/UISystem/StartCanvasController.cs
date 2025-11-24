using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class StartCanvasController : MonoBehaviour
{
    [SerializeField] private List<Text> texts;
    [SerializeField] private Button button;

    [SerializeField] private TutorialView tutorialViewPrefab;

    public void Destroy()
    {
        texts.ForEach(text => text.DOColor(new Color(text.color.r, text.color.g, text.color.b, 0), 1f));
        Destroy(button.gameObject);
        Destroy(gameObject, 1f);
    }

    public void OpenTutorial()
    {
        Instantiate(tutorialViewPrefab);
    }
}
