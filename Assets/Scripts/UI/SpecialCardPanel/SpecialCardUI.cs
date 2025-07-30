using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpecialCardUI : CardBase, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public PowerBase power;
    public Image artwork;
    public TextMeshProUGUI description;
    public SpecialCardBuyPanelManager specialCardBuyPanelManager;
    public CanvasGroup canvasGroup;
    public Button sellButton;

    public override void SetCard(PowerBase power)
    {
        this.power = power;
        description.text = power.description;
        canvasGroup.DOFade(1, 0.5f).onComplete = () =>
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        };
    }

    public void SellCard()
    {
        if (slot.slotType == SlotTypes.JokerInUse)
        {
            slot.isSlotEmpty = true;
            UIEventManager.SellJokerCard?.Invoke(power);
            canvasGroup.DOFade(0, 0.5f).onComplete = () =>
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            };
            power = null;
            sellButton.gameObject.SetActive(false);
        }
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slot.slotType == SlotTypes.JokerInUse)
        {
            sellButton.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (slot.slotType == SlotTypes.JokerInUse)
        {
            sellButton.gameObject.SetActive(false);
        }
    }


}
