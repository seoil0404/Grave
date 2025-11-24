using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class EndView : MonoBehaviour
{
    [SerializeField] private List<Text> texts;
    [SerializeField] private List<Image> images;

    [SerializeField] private Text stageCountText;

    public void Restart()
    {
        SceneManager.LoadScene("MainScene");
        GameManager.Instance.ReStart();
    }

    private void Start()
    {
        stageCountText.text = "Stage " + GameManager.StageCount;

        texts.ForEach(text => 
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            text.DOColor(new Color(text.color.r, text.color.g, text.color.b, 1), 2);
        });

        images.ForEach(image =>
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            image.DOColor(new Color(image.color.r, image.color.g, image.color.b, 0.7f), 2);
        });
    }
}
