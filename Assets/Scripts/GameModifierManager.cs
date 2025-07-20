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

    void Start()
    {
        gameBoard = FindObjectOfType<Board>();
        StartPowers();
    }

    public void StartPowers()
    {
        foreach (var power in activePowers)
        {
            power.Enable(gameBoard);  // Event bazlı için
            power.ApplyGlobal(gameBoard);  // Global için
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
}
