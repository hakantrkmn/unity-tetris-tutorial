using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class DeckPanelManager : MonoBehaviour
{
    public TetrominoList tetrominoesList;
    public List<TetrominoData> allDeck;

    public List<TetrominoData> deck;

    public List<TetrominoData> drawnCards;


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
            for (int i = 0; i < GameConstants.DEFAULT_DECK_SIZE; i++)
            {
                var card = new TetrominoData(tetromino);
                deck.Add(card);
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
        drawnCards.Add(card);
        return card;
    }

    public void OnJokerCardBought(JokerUICard card)
    {
        Debug.Log("OnJokerCardBought");
        Debug.Log(card.power);
        if (card.power.cardType == CardType.Joker)
        {
            UIEventManager.JokerCardBought?.Invoke(card.power);
        }
        GameModifierManager.Instance.activePowers.Add(card.power);
        var allCards = new List<TetrominoData>(deck);
        allCards.AddRange(drawnCards);
        GameModifierManager.Instance.StartPowers(allCards);
    }


    private void OnEnable()
    {
        GameEvents.JokerCardBought += OnJokerCardBought;
        UIEventManager.OnDrawCard += DrawCard;
    }

    private void OnDisable()
    {
        GameEvents.JokerCardBought -= OnJokerCardBought;
        UIEventManager.OnDrawCard -= DrawCard;
    }
}
