using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpecialCardUI : MonoBehaviour, IPointerClickHandler
{
    public PowerBase power;
    public Image artwork;
    public TextMeshProUGUI description;
    public SpecialCardBuyPanelManager specialCardBuyPanelManager;
    public SpecialCardSlot slot;
    public CanvasGroup canvasGroup;


    public void SetPower(PowerBase power)
    {
        this.power = power;
        description.text = power.description;
        canvasGroup.DOFade(1, 0.5f).onComplete = () =>
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        };
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slot.slotType == SlotTypes.TarotInUse)
        {
            power.Activate();
            canvasGroup.DOFade(0, 0.5f).onComplete = () =>
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            };
            slot.isSlotEmpty = true;
        }
    }
}
