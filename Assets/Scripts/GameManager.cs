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
    int rerollValueIndex = 0;

    public float stepDelay = GameConstants.DEFAULT_STEP_DELAY;
    public int gold;
    public int score;
    public int level;
    public float scoreMultiplier = GameConstants.DEFAULT_SCORE_MULTIPLIER;
    public float speedMultiplier = GameConstants.DEFAULT_SPEED_MULTIPLIER;
    public int rerollValue;
    public GameStates gameState = GameStates.OnPrepare;

    private void Start() {
        rerollValue = gameData.rerollValues[rerollValueIndex];
    }

    public void RerollButtonClicked()
    {
        rerollValueIndex++;
        if (rerollValueIndex >= gameData.rerollValues.Length)
        {
            rerollValueIndex = 0;
        }
        gold -= rerollValue;
        rerollValue = gameData.rerollValues[rerollValueIndex];
        UIEventManager.UpdateScorePanel?.Invoke();
    }
    public void AddScore(int baseScore)
    {
        score += Mathf.RoundToInt(baseScore * scoreMultiplier);
        // Burada bir UI güncelleme event'i tetikleyebilirsiniz.
        Debug.Log($"Skor: {score} (Çarpan: {scoreMultiplier})");
    }

    public void ChangeDropSpeed(float percentage)
    {
        stepDelay *= (1 - percentage);
    }


    private void OnEnable() {
        UIEventManager.RerollButtonClicked += RerollButtonClicked;
        UIEventManager.PlayButtonClicked += () => gameState = GameStates.OnGame;
        GetDataEvents.GetGameData += () => gameData;
    }

    private void OnDisable() {
        UIEventManager.RerollButtonClicked -= RerollButtonClicked;
        UIEventManager.PlayButtonClicked -= () => gameState = GameStates.OnGame;
        GetDataEvents.GetGameData -= () => gameData;
    }
}
