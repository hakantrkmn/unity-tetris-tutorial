using System.Collections.Generic;
using UnityEngine;

public class GameModifierManager : MonoBehaviour
{
    public Board gameBoard;
    public List<PowerBase> activePowers;  // Listeyi PowerBase'e değiştir

    void Start()
    {
        gameBoard = FindObjectOfType<Board>();
        foreach (var power in activePowers)
        {
            power.Enable(gameBoard);  // Event bazlı için
            power.ApplyGlobal(gameBoard);  // Global için
        }
        // Parça bazlı için: Piece'in Lock metoduna entegre et (aşağıda)
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
