using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameData gameData;
    int rerollValueIndex = 0;

    private void Start() {
        PlayerData.Instance.rerollValue = gameData.rerollValues[rerollValueIndex];
    }

    public void RerollButtonClicked()
    {
        rerollValueIndex++;
        if (rerollValueIndex >= gameData.rerollValues.Length)
        {
            rerollValueIndex = 0;
        }
        int rerollValue = PlayerData.Instance.rerollValue;
        PlayerData.Instance.gold -= rerollValue;
        PlayerData.Instance.rerollValue = gameData.rerollValues[rerollValueIndex];
        UIEventManager.UpdateScorePanel?.Invoke();
    }



    private void OnEnable() {
        UIEventManager.RerollButtonClicked += RerollButtonClicked;
        GetDataEvents.GetGameData += () => gameData;
    }

    private void OnDisable() {
        UIEventManager.RerollButtonClicked -= RerollButtonClicked;
        GetDataEvents.GetGameData -= () => gameData;
    }
}
