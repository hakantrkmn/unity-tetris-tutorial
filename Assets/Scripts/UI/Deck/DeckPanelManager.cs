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
    public List<CardUI> selectedCards;

    public Transform deckContainer;


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

    public TetrominoData DrawCard(int? index)
    {
        if (deck.Count == 0)
        {
            Debug.Log("No cards left in deck");
            return null;
        }
        var card = deck[Random.Range(0, deck.Count)];
        while (drawnCards.Contains(card) || selectedCards.Any(x => x.tetromino == card))
        {
            card = deck[Random.Range(0, deck.Count)];
        }
        if (index != null)
        {
            drawnCards[index.Value] = card;
        }
        else
        {
            drawnCards.Add(card);
        }
        return card;
    }

    public TetrominoData AddRandomCardToDeck()
    {
        drawnCards.Add(deck[Random.Range(0, deck.Count)]);
        return drawnCards.Last();
    }

    public TetrominoData GetRandomCardFromDeck()
    {
        var card = new TetrominoData(deck[Random.Range(0, deck.Count)]);
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
        if (card.power.cardType == CardType.Tarot)
        {
            UIEventManager.TarotCardBought?.Invoke(card.power);
        }
        GameModifierManager.Instance.activePowers.Add(card.power);
        GameModifierManager.Instance.StartPowers(drawnCards);
    }


    public void DiscardCards()
    {

    }

    [Button]
    public void PlayButtonClicked()
    {

        GameEvents.GameCanStart?.Invoke();
    }

    private void OnEnable()
    {
        UIEventManager.PlayButtonClicked += PlayButtonClicked;
        GameEvents.JokerCardBought += OnJokerCardBought;
        UIEventManager.OnDrawCard += DrawCard;
        UIEventManager.DiscardButtonClicked += DiscardCards;
        UIEventManager.GetDrawnCards += () => drawnCards;
        GameEvents.AddRandomCardToDeck += AddRandomCardToDeck;
        GameEvents.GetRandomCardFromDeck += GetRandomCardFromDeck;
    }

    private void OnDisable()
    {
        UIEventManager.PlayButtonClicked -= PlayButtonClicked;
        GameEvents.JokerCardBought -= OnJokerCardBought;
        UIEventManager.OnDrawCard -= DrawCard;
        UIEventManager.DiscardButtonClicked -= DiscardCards;
        UIEventManager.GetDrawnCards -= () => drawnCards;
        GameEvents.AddRandomCardToDeck -= AddRandomCardToDeck;
        GameEvents.GetRandomCardFromDeck -= GetRandomCardFromDeck;
    }
}
