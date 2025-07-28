using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardsPanelManager : MonoBehaviour
{
    public GameObject cardPrefab;

    public Transform cardContainer;


    public int startCardCount = GameConstants.DEFAULT_START_CARD_COUNT;

    public List<CardSlot> slots;

    public Slot deckSlot;

    public DeckPanelManager deckPanelManager;

    // Dynamic card moving system
    private CardUI draggedCard;
    private int originalSlotIndex = -1;
    private Canvas canvas;

    private void OnValidate()
    {
        slots = GetComponentsInChildren<CardSlot>().ToList();
        deckPanelManager = FindObjectOfType<DeckPanelManager>();
    }

    private void Start()
    {
        // Get canvas reference
        canvas = GetComponentInParent<Canvas>();
    }

    // Dynamic card moving methods
    public void OnCardDragStart(CardUI card)
    {
        draggedCard = card;

        // Find the actual slot index by checking which slot contains this card
        originalSlotIndex = -1;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount > 0 &&
                slots[i].transform.GetChild(0).GetComponent<CardUI>() == card)
            {
                originalSlotIndex = i;
                break;
            }
        }

        // Disable selection during drag
        if (card.isSelected)
        {
            card.isSelected = false;
            deckPanelManager.selectedCards.Remove(card);
        }
    }



    public void OnCardDragEnd(CardUI card, int targetSlotIndex)
    {
        if (draggedCard != card) return;

        if (targetSlotIndex >= 0 && targetSlotIndex < slots.Count)
        {
            CommitCardMove(card, targetSlotIndex);
        }
        else
        {
            // Invalid drop, return to original position
            ReturnCardToOriginalPosition(card);
        }

        // Reset drag state
        draggedCard = null;
        originalSlotIndex = -1;
    }



    private void CommitCardMove(CardUI card, int newSlotIndex)
    {
        // Simple approach: Just reorganize everything from scratch

        // First, get all cards including the dragged one
        List<CardUI> allCards = new List<CardUI>();

        // Collect all existing cards except the dragged one
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].card != null)
            {
                var existingCard = slots[i].card;
                if (existingCard != null && existingCard != card)
                {
                    allCards.Add(existingCard);
                }
            }
        }

        // Insert the dragged card at the target position
        if (newSlotIndex >= allCards.Count)
        {
            allCards.Add(card);
        }
        else
        {
            allCards.Insert(newSlotIndex, card);
        }

        // Now place all cards in their new positions
        FinalizeCardPositions(allCards);
    }

    private void FinalizeCardPositions(List<CardUI> cards)
    {
        // First, clear all slots
        foreach (var slot in slots)
        {
            slot.isSlotEmpty = true;
        }

        // Place cards in order
        for (int i = 0; i < cards.Count && i < slots.Count; i++)
        {
            var card = cards[i];
            var targetSlot = slots[i];

            // Update slot assignment
            card.slot = targetSlot;
            targetSlot.isSlotEmpty = false;

            // Animate card to position
            card.transform.DOMoveX(targetSlot.transform.position.x, 0.2f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    if (card != null && targetSlot != null)
                    {
                        targetSlot.card = card;
                        card.slot = targetSlot;
                        card.transform.SetParent(targetSlot.transform);
                        card.transform.localPosition = Vector3.zero;
                        card.isSelected = false;
                    }
                });
        }

        // Update deck order to match slot order
        UpdateDeckOrder(cards);
    }

    private void UpdateDeckOrder(List<CardUI> orderedCards)
    {
        if (deckPanelManager == null) return;

        // Clear current deck and rebuild it according to new card order
        deckPanelManager.drawnCards.Clear();

        // Add cards to deck in the new slot order
        foreach (var card in orderedCards)
        {
            if (card != null && card.tetromino != null)
            {
                deckPanelManager.drawnCards.Add(card.tetromino);
            }
        }

        UnityEngine.Debug.Log("Deck updated to new order. Count: " + deckPanelManager.drawnCards.Count);
    }

    private void ReturnCardToOriginalPosition(CardUI card)
    {
        // Ensure original slot index is valid
        if (originalSlotIndex < 0 || originalSlotIndex >= slots.Count)
        {
            Debug.LogError("Invalid original slot index: " + originalSlotIndex);
            return;
        }

        // Simply return dragged card to original position and reorganize everything
        card.transform.DOMoveX(slots[originalSlotIndex].transform.position.x, 0.2f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                if (card != null && slots[originalSlotIndex] != null)
                {
                    card.transform.SetParent(slots[originalSlotIndex].transform);
                    card.transform.localPosition = Vector3.zero;
                    card.slot = slots[originalSlotIndex];
                    card.isSelected = false;
                }
            });
    }





    public int GetSlotIndexAtPosition(Vector3 screenPosition)
    {
        float minDistance = float.MaxValue;
        int closestIndex = -1;

        for (int i = 0; i < slots.Count; i++)
        {
            // Convert slot world position to screen position for accurate comparison
            Vector3 slotScreenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, slots[i].transform.position);
            float distance = Vector3.Distance(screenPosition, slotScreenPos);

            if (distance < minDistance && distance < 150f) // Increased threshold for UI elements
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    [Button]
    public void DrawCard()
    {
        for (int i = 0; i < startCardCount; i++)
        {
            var card = UIEventManager.OnDrawCard?.Invoke(null);
            if (card != null)
            {
                CreateCard(card, i);
            }
        }

    }

    [Button]
    public void CreateCard(TetrominoData tetromino, int index)
    {
        var slot = slots[index];
        slot.index = index;
        if (slot != null)
        {
            slot.isSlotEmpty = false;
        }
        else
        {
            Debug.LogError("No empty slot found");
        }

        slot.card.SetCard(tetromino, this);
    }

    public void DiscardCards()
    {
        var sequence = DOTween.Sequence();
        int amount = deckPanelManager.selectedCards.Count;
        if (amount == 0)
        {
            return;
        }
        for (int i = 0; i < amount; i++)
        {
            var card = deckPanelManager.selectedCards[i];
            card.slot.isSlotEmpty = true;
            sequence.Join(card.slot.transform.GetChild(0).DOJump(deckPanelManager.deckContainer.position, 100, 1, 0.5f)
            .OnComplete(() =>
            {
                card.DestroyCard();
                card.isSelected = false;

                var nextCard = UIEventManager.OnDrawCard?.Invoke(card.slot.index);
                if (nextCard != null)
                {
                    CreateCard(nextCard, card.slot.index);
                }
            }));

        }
        sequence.OnComplete(() =>
        {

            deckPanelManager.selectedCards.Clear();
        });
    }

    public void ClearSlots()
    {
        foreach (var slot in slots)
        {
            slot.isSlotEmpty = true;
            slot.card.DestroyCard(false);
        }
    }

    private void OnEnable()
    {
        UIEventManager.DiscardButtonClicked += DiscardCards;
        UIEventManager.DeckInitialized += DrawCard;
        GameEvents.GameStateOver += DrawCard;
        GameEvents.GameCanStart += ClearSlots;
    }

    private void OnDisable()
    {
        UIEventManager.DiscardButtonClicked -= DiscardCards;
        UIEventManager.DeckInitialized -= DrawCard;
        GameEvents.GameStateOver -= DrawCard;
        GameEvents.GameCanStart -= ClearSlots;
    }
}
