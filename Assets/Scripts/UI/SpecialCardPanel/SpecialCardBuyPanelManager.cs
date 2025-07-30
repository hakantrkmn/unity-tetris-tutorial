using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialCardBuyPanelManager : MonoBehaviour
{
    public List<Slot> jokerSlots;
    public List<Slot> tarotSlots;

    public SpecialCardUI specialCardPrefab;

    private void OnValidate()
    {
        FillSlots();
    }


    private void Start()
    {
        FillSlots();
    }

    public void FillSlots()
    {
        jokerSlots = GetComponentsInChildren<Slot>().Where(slot => slot.slotType == SlotTypes.JokerInUse).ToList();
        tarotSlots = GetComponentsInChildren<Slot>().Where(slot => slot.slotType == SlotTypes.TarotInUse).ToList();

        foreach (var slot in jokerSlots)
        {
            slot.card = slot.GetComponentInChildren<CardBase>();
            slot.card.slot = slot;
        }

        foreach (var slot in tarotSlots)
        {
            slot.card = slot.GetComponentInChildren<CardBase>();
            slot.card.slot = slot;
        }
    }

    private void OnEnable()
    {
        UIEventManager.JokerCardBought += OnJokerCardBought;
        UIEventManager.TarotCardBought += OnTarotCardBought;
    }

    private void OnDisable()
    {
        UIEventManager.JokerCardBought -= OnJokerCardBought;
        UIEventManager.TarotCardBought -= OnTarotCardBought;
    }

    private void OnJokerCardBought(PowerBase power)
    {
        var emptySlot = jokerSlots.FirstOrDefault(x => x.isSlotEmpty);
        if (emptySlot == null)
        {
            Debug.Log("No empty slot found");
            return;
        }
        emptySlot.card.SetCard(power);
        (emptySlot.card as SpecialCardUI).specialCardBuyPanelManager = this;
        emptySlot.isSlotEmpty = false;
    }

    private void OnTarotCardBought(PowerBase power)
    {
        var emptySlot = tarotSlots.FirstOrDefault(x => x.isSlotEmpty);
        if (emptySlot == null)
        {
            Debug.Log("No empty slot found");
            return;
        }
        emptySlot.card.SetCard(power);
        (emptySlot.card as SpecialCardUI).specialCardBuyPanelManager = this;
        emptySlot.isSlotEmpty = false;
    }
}
