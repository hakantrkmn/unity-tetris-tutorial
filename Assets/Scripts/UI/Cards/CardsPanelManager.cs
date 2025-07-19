using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardsPanelManager : MonoBehaviour
{
    public GameObject cardPrefab;

    public Transform cardContainer;

    public List<CardUI> drawnCards;

    public int startCardCount = 10;

    [Button]
    public void DrawCard()
    {
        for (int i = 0; i < startCardCount; i++)
        {
            var card = UIEventManager.OnDrawCard?.Invoke();
            if (card != null)
            {
                CreateCard(card.Value);
            }
        }

    }

    [Button]
    public void CreateCard(TetrominoData tetromino)
    {
        var card = Instantiate(cardPrefab, cardContainer);
        card.GetComponent<CardUI>().sprite.sprite = tetromino.artwork;
        card.GetComponent<CardUI>().tetromino = tetromino;
        drawnCards.Add(card.GetComponent<CardUI>());
    }

    private void OnEnable()
    {
        UIEventManager.DeckInitialized += DrawCard;
        UIEventManager.GetDrawnCards += () => drawnCards.Select(x => x.tetromino).ToList();
    }

    private void OnDisable()
    {
        UIEventManager.DeckInitialized -= DrawCard;
        UIEventManager.GetDrawnCards -= () => drawnCards.Select(x => x.tetromino).ToList();
    }
}
