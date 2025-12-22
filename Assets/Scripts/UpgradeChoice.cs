using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UpgradeChoice : MonoBehaviour, /*IPointerEnterHandler, IPointerExitHandler,*/ IPointerDownHandler
{
    [Header("Children")]
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;
    public Image icon;

    [Header("Data")]
    [HideInInspector] public UpgradeManager.Upgrade upgrade;

    [Header("References")]
    [HideInInspector] public UpgradeManager upgradeManager;


    public void Start()
    {
        title.text = upgrade.name;
        desc.text = upgrade.description;
        icon.sprite = upgrade.icon;
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        upgradeManager.AddUpgrade(upgrade);
    }

    /*public void OnPointerEnter(PointerEventData pointerEventData)
    {
        transform.localScale *= 1.15f;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        transform.localScale /= 1.15f;
    }*/
}
