using System.Collections.Generic;
using UnityEngine;

public class GameModifierManager : MonoBehaviour
{
    public Board gameBoard;
    public List<RoundModifierPower> activeModifiers;

    void Start()
    {
        gameBoard = FindObjectOfType<Board>();
        foreach (var modifier in activeModifiers)
        {
            modifier.Enable(gameBoard);
        }
    }

    void OnDestroy()
    {
        foreach (var modifier in activeModifiers)
        {
            modifier.Disable(gameBoard);
        }
    }
}