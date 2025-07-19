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
        int playerMoney = PlayerData.Instance.gold;
        int rerollValue = PlayerData.Instance.rerollValue;
        if (playerMoney >= rerollValue)
        {
            UIEventManager.RerollButtonClicked?.Invoke();
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
