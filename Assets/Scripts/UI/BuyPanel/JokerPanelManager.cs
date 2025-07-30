using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JokerPanelManager : SerializedMonoBehaviour
{
    public List<PowerBase> availablePowers;
    public List<PowerBase> inUsePowers;

    public List<Slot> powerSlots;
    public JokerUICard jokerUICardPrefab;


    void OnValidate()
    {
        powerSlots = new List<Slot>();
        powerSlots.AddRange(transform.GetComponentsInChildren<Slot>().Where(slot => slot.slotType == SlotTypes.JokerBuy));
    }

    private void Start()
    {
        powerSlots = new List<Slot>();
        var gameData = GetDataEvents.GetGameData?.Invoke();
        availablePowers = new List<PowerBase>(gameData.powers);
        powerSlots.AddRange(transform.GetComponentsInChildren<Slot>().Where(slot => slot.slotType == SlotTypes.JokerBuy));
        CreateRandomPowerCard(powerSlots.Count);
    }

    [Button]
    public void CreateRandomPowerCard(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // find first empty slot
            var emptySlot = powerSlots.FirstOrDefault(slot => slot.isSlotEmpty);
            var randomPower = availablePowers[Random.Range(0, availablePowers.Count)];
            var jokerUICard = emptySlot.card as JokerUICard;
            jokerUICard.SetCard(randomPower);
            jokerUICard.jokerPanelManager = this;
            availablePowers.Remove(randomPower);
            inUsePowers.Add(randomPower);
            emptySlot.isSlotEmpty = false;
        }
    }

    public void ClearPowerSlots()
    {
        foreach (var slot in powerSlots)
        {
            slot.isSlotEmpty = true;
            (slot.card as JokerUICard).DestroyCard(false);
        }

    }

    public void UpdateAvailablePowers(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            //get items on inusepowers list amount
            var powers = inUsePowers[i];
            availablePowers.Add(powers);
            inUsePowers.Remove(powers);
        }
    }



    public void Reroll()
    {
        ClearPowerSlots();
        CreateRandomPowerCard(powerSlots.Count);
        UpdateAvailablePowers(powerSlots.Count);

    }

    private void OnEnable()
    {
        UIEventManager.Reroll += Reroll;

    }

    private void OnDisable()
    {
        UIEventManager.Reroll -= Reroll;
    }

}
