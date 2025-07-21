using System.Collections.Generic;
using UnityEngine;

public class GameModifierManager : MonoBehaviour
{
    private static GameModifierManager _instance;
    public static GameModifierManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameModifierManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameModifierManager");
                    _instance = go.AddComponent<GameModifierManager>();
                }
            }
            return _instance;
        }
    }

    public Board gameBoard;
    public List<PowerBase> activePowers;  // Listeyi PowerBase'e değiştir
    public List<PowerBase> appliedPowers;

    public GameObject ghost;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    void OnValidate()
    {
        gameBoard = FindObjectOfType<Board>();
    }
    void Start()
    {
        gameBoard = FindObjectOfType<Board>();
    }

    public void StartPowers(List<TetrominoData> deck)
    {
        foreach (var power in activePowers)
        {
            if (appliedPowers.Contains(power))
            {
                continue;
            }
            power.Enable(gameBoard);  // Event bazlı için
            power.ApplyGlobal(gameBoard);  // Global için
            power.ApplyToDeck(deck);
            appliedPowers.Add(power);
        }
    }

    void OnDestroy()
    {
        foreach (var power in activePowers)
        {
            power.Disable(gameBoard);
            power.RevertGlobal(gameBoard);
        }
    }

    public void EnableGhost()
    {
        ghost.SetActive(true);
    }

    public void DisableGhost()
    {
        ghost.SetActive(false);
    }
}
