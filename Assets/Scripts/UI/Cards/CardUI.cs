using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler
, IDropHandler, IEndDragHandler
{
    public Image sprite;
    public TetrominoData tetromino;

    public bool isSelected = false;
    public bool isDragging = false;

    public CardSlot slot;

    public DeckPanelManager deckPanelManager;
    public CardsPanelManager cardsPanelManager; // Reference to cards panel manager

    public CanvasGroup canvasGroup;

    public LayoutElement layoutElement;

    // Drag system variables
    private Canvas canvas;

    private void Awake()
    {
        slot = GetComponentInParent<CardSlot>();
        deckPanelManager = FindObjectOfType<DeckPanelManager>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void SetCard(TetrominoData tetromino, CardsPanelManager cardsPanelManager)
    {
        this.tetromino = tetromino;
        sprite.sprite = tetromino.artwork;
        this.cardsPanelManager = cardsPanelManager; // Set reference to this manager
        transform.DOLocalJump(Vector3.zero, 100, 1, 0.5f);
        canvasGroup.DOFade(1, 0.5f);
    }

    public void DestroyCard(bool hasAnimation = true)
    {
        slot.isSlotEmpty = true;
        if (hasAnimation)
        {
            transform.DOLocalJump(deckPanelManager.deckContainer.position, 100, 1, 0.5f).onComplete = () =>
        {
            canvasGroup.alpha = 0;
        };
        }
        else
        {
            canvasGroup.alpha = 0;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDragging)
        {
            return;
        }
        isSelected = !isSelected;
        var offset = Screen.height / 10;
        if (isSelected)
        {
            deckPanelManager.selectedCards.Add(this);
            isSelected = true;
            transform.DOMoveY(transform.position.y + offset, 0.2f).SetEase(Ease.OutBack);
        }
        else
        {
            deckPanelManager.selectedCards.Remove(this);
            isSelected = false;
            transform.DOMoveY(transform.position.y - offset, 0.2f).SetEase(Ease.OutBack);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIEventManager.ShowTooltip?.Invoke(tetromino.specialPowers);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIEventManager.HideTooltip?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // Update card position to follow mouse
        transform.position = Input.mousePosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        // Store original state

        // Setup dragging visuals
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f; // Make card semi-transparent while dragging

        // Move to top level for proper rendering
        transform.SetParent(canvas.transform);

        // Ensure card renders on top
        transform.SetAsLastSibling();

        // Notify manager that drag started
        if (cardsPanelManager != null)
        {
            cardsPanelManager.OnCardDragStart(this);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        // Restore visual state
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // Get final target slot
        int finalTargetSlot = GetSlotIndexUnderMouse();

        // Notify manager to finalize the move
        if (cardsPanelManager != null)
        {
            cardsPanelManager.OnCardDragEnd(this, finalTargetSlot);
        }
    }

    private int GetSlotIndexUnderMouse()
    {
        if (cardsPanelManager == null) return -1;

        // Use the manager's method to get slot index at current mouse position
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // Ensure z is 0 for 2D UI

        return cardsPanelManager.GetSlotIndexAtPosition(Input.mousePosition);
    }

    public void OnDrop(PointerEventData eventData)
    {
        // This method is now handled by the enhanced drag system
        // Keeping it for compatibility but functionality moved to OnEndDrag
    }
}
