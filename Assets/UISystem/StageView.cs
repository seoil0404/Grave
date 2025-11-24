using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StageView : MonoBehaviour
{
    [SerializeField] private Text stageCountText;
    [SerializeField] private Text stageCountBackgrouondText;
    [SerializeField] private float fadeTime;

    private void Start()
    {
        stageCountText.text = "Stage " + GameManager.StageCount;
        stageCountBackgrouondText.text = "Stage " + GameManager.StageCount;
        stageCountText.color = new Color(stageCountText.color.r, stageCountText.color.g, stageCountText.color.b, 0);
        stageCountBackgrouondText.color = new Color(stageCountBackgrouondText.color.r, stageCountBackgrouondText.color.g, stageCountBackgrouondText.color.b, 0);
        StartCoroutine(SetStageCountText());
    }

    private IEnumerator SetStageCountText()
    {
        stageCountText.DOColor(new Color(stageCountText.color.r, stageCountText.color.g, stageCountText.color.b, 1), fadeTime);
        stageCountBackgrouondText.DOColor(new Color(stageCountBackgrouondText.color.r, stageCountBackgrouondText.color.g, stageCountBackgrouondText.color.b, 1), fadeTime);
        yield return new WaitForSeconds(fadeTime);
        stageCountText.DOColor(new Color(stageCountText.color.r, stageCountText.color.g, stageCountText.color.b, 0), fadeTime);
        stageCountBackgrouondText.DOColor(new Color(stageCountBackgrouondText.color.r, stageCountBackgrouondText.color.g, stageCountBackgrouondText.color.b, 0), fadeTime);
        yield return new WaitForSeconds(fadeTime);
        Destroy(gameObject);
    }
}
