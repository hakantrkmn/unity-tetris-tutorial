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
    }

    private void OnEnable()
    {
        UIEventManager.JokerCardBought += OnJokerCardBought;
    }

    private void OnDisable()
    {
        UIEventManager.JokerCardBought -= OnJokerCardBought;
    }

    private void OnJokerCardBought(PowerBase power)
    {
        var emptySlot = jokerSlots.FirstOrDefault(x => x.isSlotEmpty);
        if (emptySlot == null)
        {
            Debug.Log("No empty slot found");
            return;
        }
        var card = Instantiate(specialCardPrefab, emptySlot.transform);
        card.power = power;
        card.artwork.sprite = power.artwork;
        card.specialCardBuyPanelManager = this;
        emptySlot.isSlotEmpty = false;
    }
}
