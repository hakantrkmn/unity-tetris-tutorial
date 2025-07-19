using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class DeckPanelManager : MonoBehaviour
{
    public TetrominoList tetrominoesList;
    
    public List<TetrominoData> deck;


    private void Start()
    {
        CreateDeck();
    }

    [Button]
    public void CreateDeck()
    {
        deck = new List<TetrominoData>();
        foreach (var tetromino in tetrominoesList.tetrominoes)
        {
            for (int i = 0; i < 10; i++)
            {
                deck.Add(tetromino);
            }
        }
        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
        deck = deck.OrderBy(x => Random.value).ToList();
        UIEventManager.DeckInitialized?.Invoke();
    }

    public TetrominoData? DrawCard()
    {
        if (deck.Count == 0)
        {
            Debug.Log("No cards left in deck");
            return null;
        }
        var card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    private void OnEnable()
    {
        UIEventManager.OnDrawCard += DrawCard;
    }

    private void OnDisable()
    {
        UIEventManager.OnDrawCard -= DrawCard;
    }
}
