using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialCardBuyPanelManager : MonoBehaviour
{
    public List<Slot> slots;

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
        slots = GetComponentsInChildren<Slot>().ToList();
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
        var emptySlot = slots.FirstOrDefault(x => x.isSlotEmpty);
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
