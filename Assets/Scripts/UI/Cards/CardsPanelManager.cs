using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardsPanelManager : MonoBehaviour
{
    public GameObject cardPrefab;

    public Transform cardContainer;

    public List<CardUI> drawnCards;

    [Button]
    public void DrawCard()
    {
        var card = UIEventManager.OnDrawCard?.Invoke();
        if (card != null)
        {
            CreateCard(card.Value);
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
}
