using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static List<Type> upgradeStrategyTypes = Assembly
        .GetExecutingAssembly()
        .GetTypes()
        .Where(t => typeof(IUpgradeStrategy).IsAssignableFrom(t)
                    && t.IsClass
                    && !t.IsAbstract)
        .ToList();

    [SerializeField] private Image image;
    [SerializeField] private Image imageShadow;
    [SerializeField] private Image ButtonTexture;
    [SerializeField] private Text desciptionText;

    private IUpgradeStrategy upgradeStrategy;

    private void Start()
    {
        int randomCount = UnityEngine.Random.Range(0, upgradeStrategyTypes.Count);
        upgradeStrategy = Activator.CreateInstance(upgradeStrategyTypes[randomCount]) as IUpgradeStrategy;

        image.sprite = upgradeStrategy.Sprite;
        imageShadow.sprite = upgradeStrategy.Sprite;
        desciptionText.text = upgradeStrategy.Description;
    }

    public void Apply()
    {
        upgradeStrategy.Apply();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ButtonTexture.color = new Color(1, 1, 1, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ButtonTexture.color = new Color(1, 1, 1, 1f);
    }
}
