using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardsPanelManager : MonoBehaviour
{
    public GameObject cardPrefab;

    [Button]
    public void CreateCard(TetrominoData tetromino)
    {
        var card = Instantiate(cardPrefab, transform);
        card.GetComponent<CardUI>().sprite.sprite = tetromino.artwork;
        card.GetComponent<CardUI>().tetromino = tetromino;
    }
}
