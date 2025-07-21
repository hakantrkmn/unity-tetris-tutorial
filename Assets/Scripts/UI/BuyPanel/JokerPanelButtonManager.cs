using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JokerPanelButtonManager : MonoBehaviour
{
    public Button rerollButton;
    public Button playButton;

    public void RerollButtonClicked()
    {
        int playerMoney = GameManager.Instance.gameSession.gold;
        int rerollValue = GameManager.Instance.gameSession.rerollValue;
        if (playerMoney >= rerollValue)
        {
            UIEventManager.Reroll?.Invoke();
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }

    public void PlayButtonClicked()
    {
        UIEventManager.PlayButtonClicked?.Invoke();
    }
}
