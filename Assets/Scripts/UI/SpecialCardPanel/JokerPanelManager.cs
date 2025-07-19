using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class JokerPanelManager : SerializedMonoBehaviour
{
    public UpgradeData upgradeData;
    public Dictionary<Transform, JokerUICard> powerSlots;
    public JokerUICard jokerUICardPrefab;
    private void Start() {
        powerSlots = new Dictionary<Transform, JokerUICard>();
        upgradeData = GetDataEvents.GetUpgradeData?.Invoke();
        for (int i = 0; i < transform.GetChild(0).childCount; i++) {
            var powerSlot = transform.GetChild(0).GetChild(i);
            powerSlots.Add(powerSlot, null);
        }
    }

    [Button]
    public void CreateRandomPowerCard() {
        // find first empty slot
        var emptySlot = powerSlots.FirstOrDefault(slot => slot.Value == null).Key;
        var randomPower = upgradeData.piecePowers[Random.Range(0, upgradeData.piecePowers.Count)];
        var jokerUICard = Instantiate(jokerUICardPrefab, emptySlot);
        jokerUICard.power = randomPower;
        jokerUICard.roundPower = null;
        jokerUICard.artwork.sprite = randomPower.artwork;
        powerSlots[emptySlot] = jokerUICard;
    }

}
