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

    public List<JokerSlot> powerSlots;
    public JokerUICard jokerUICardPrefab;

    public TextMeshProUGUI rerollValueText;
    private void Start() {
        powerSlots = new List<JokerSlot>();
        var gameData = GetDataEvents.GetGameData?.Invoke();
        availablePowers = new List<PowerBase>(gameData.powers);
        powerSlots.AddRange(transform.GetComponentsInChildren<JokerSlot>());
        for (int i = 0; i < powerSlots.Count; i++) {
            CreateRandomPowerCard();
        }
    }

    [Button]
    public void CreateRandomPowerCard() {
        // find first empty slot
        var emptySlot = powerSlots.FirstOrDefault(slot => slot.isSlotEmpty());
        var randomPower = availablePowers[Random.Range(0, availablePowers.Count)];
        var jokerUICard = Instantiate(jokerUICardPrefab, emptySlot.transform);
        jokerUICard.power = randomPower;
        jokerUICard.artwork.sprite = randomPower.artwork;
        jokerUICard.jokerPanelManager = this;
        availablePowers.Remove(randomPower);
        inUsePowers.Add(randomPower);
    }

    public void UpdateRerollValueText()
    {
        rerollValueText.text = "Reroll: " + PlayerData.Instance.rerollValue.ToString();
    }
    private void OnEnable() {
        UIEventManager.UpdateScorePanel += UpdateRerollValueText;
    }

    private void OnDisable() {
        UIEventManager.UpdateScorePanel -= UpdateRerollValueText;
    }

}
