using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }
    #endregion
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public GameData gameData;
    public GameSession gameSession;
    int rerollValueIndex = 0;

    private void Start() {
        gameSession.rerollValue = gameData.rerollPrices[rerollValueIndex];
    }

    public void Reroll()
    {
        rerollValueIndex++;
        if (rerollValueIndex >= gameData.rerollPrices.Length)
        {
            rerollValueIndex = 0;
        }
        gameSession.gold -= gameSession.rerollValue;
        gameSession.rerollValue = gameData.rerollPrices[rerollValueIndex];
        UIEventManager.UpdateScorePanel?.Invoke();
        UIEventManager.UpdateButtonText?.Invoke(ButtonType.Reroll);
    }

    public void RerollButtonClicked()
    {
        int playerMoney = gameSession.gold;
        int rerollValue = gameSession.rerollValue;
        if (playerMoney >= rerollValue)
        {
            UIEventManager.Reroll?.Invoke();
        }
        else
        {
            Debug.Log("Not enough money");
        }

    }
    public void AddScore(int baseScore)
    {
        gameSession.score += Mathf.RoundToInt(baseScore * gameSession.scoreMultiplier);
        // Burada bir UI güncelleme event'i tetikleyebilirsiniz.
        Debug.Log($"Skor: {gameSession.score} (Çarpan: {gameSession.scoreMultiplier})");
    }

    public void ChangeDropSpeed(float percentage)
    {
        gameSession.stepDelay *= (1 - percentage);
    }


    private void OnEnable() {
        UIEventManager.RerollButtonClicked += RerollButtonClicked;
        UIEventManager.PlayButtonClicked += () => gameSession.gameState = GameStates.OnGame;
        GetDataEvents.GetGameData += () => gameData;
        UIEventManager.Reroll += Reroll;
    }

    private void OnDisable() {
        UIEventManager.RerollButtonClicked -= RerollButtonClicked;
        UIEventManager.PlayButtonClicked -= () => gameSession.gameState = GameStates.OnGame;
        GetDataEvents.GetGameData -= () => gameData;
        UIEventManager.Reroll -= Reroll;
    }
}

