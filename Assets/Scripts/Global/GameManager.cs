using System.Collections.Generic;
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
    }
    public GameData gameData;
    public GameSession gameSession;
    public Board board;
    int rerollValueIndex = 0;


    private void OnValidate()
    {
        board = FindObjectOfType<Board>();
    }
    private void Start()
    {
        gameSession.rerollValue = gameData.rerollPrices[rerollValueIndex];
        board = FindObjectOfType<Board>();
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

    public void ChangeDropSpeed(float multiplier)
    {
        if (multiplier > 0)
        {
            gameSession.stepDelay *= multiplier;
        }
        else
        {
            gameSession.stepDelay /= Mathf.Abs(multiplier);
        }
    }

    private void OnPieceCleared(TetrominoData data)
    {
        Debug.Log("PieceCleared: " + data.tetromino + " " + data.level + " " + data.points[data.level]);
        gameSession.score += data.points[data.level];
        UIEventManager.UpdateScorePanel?.Invoke();
    }

    private void OnLineCleared(int lines, List<TetrominoData> clearedLineTiles)
    {
        Debug.Log("LineCleared: " + lines);

        // Renklerine göre tile'ları grupla
        Dictionary<TetrominoColor, List<TetrominoData>> tilesByColor = new Dictionary<TetrominoColor, List<TetrominoData>>();

        foreach (var tile in clearedLineTiles)
        {
            Debug.Log("ClearedTile: " + tile.color);

            // Aynı renkteki tile'ları aynı listeye ekle
            if (!tilesByColor.ContainsKey(tile.color))
            {
                tilesByColor[tile.color] = new List<TetrominoData>();
            }
            tilesByColor[tile.color].Add(tile);
        }

        // Her renk grubunu ayrı ayrı logla
        foreach (var colorGroup in tilesByColor)
        {
            Debug.Log($"Renk {colorGroup.Key.colorName}: {colorGroup.Value.Count} adet tile");
            foreach (var tile in colorGroup.Value)
            {
                Debug.Log($"  - {tile.tetromino} (Level: {tile.level})");
            }
        }

        gameSession.score += lines * 100;
        UIEventManager.UpdateScorePanel?.Invoke();
        if (lines == 2)
        {
            GameEvents.AddRandomCardToDeck?.Invoke();
        }
    }


    private void OnEnable()
    {
        UIEventManager.RerollButtonClicked += RerollButtonClicked;
        GameEvents.GameCanStart += () => gameSession.gameState = GameStates.OnGame;
        GetDataEvents.GetGameData += () => gameData;
        UIEventManager.Reroll += Reroll;
        GameEvents.PieceCleared += OnPieceCleared;
        GameEvents.OnLineCleared += OnLineCleared;
    }

    private void OnDisable()
    {
        UIEventManager.RerollButtonClicked -= RerollButtonClicked;
        GameEvents.GameCanStart -= () => gameSession.gameState = GameStates.OnGame;
        GetDataEvents.GetGameData -= () => gameData;
        UIEventManager.Reroll -= Reroll;
        GameEvents.PieceCleared -= OnPieceCleared;
        GameEvents.OnLineCleared -= OnLineCleared;
    }
}

