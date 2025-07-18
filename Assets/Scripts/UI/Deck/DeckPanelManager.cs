using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DeckPanelManager : MonoBehaviour
{
    public TetrominoList tetrominoesList;
    
    public List<TetrominoData> deck;

    [Button]
    public void CreateDeck()
    {
        foreach (var tetromino in tetrominoesList.tetrominoes)
        {
            for (int i = 0; i < 10; i++)
            {
                deck.Add(tetromino);
            }
        }
    }
}
