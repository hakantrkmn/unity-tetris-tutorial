using UnityEngine;

[CreateAssetMenu(fileName = "New Event Power", menuName = "Tetris/Powers/Event Power")]
public class EventPower : PowerBase
{
    // Hangi event'e abone olacağını belirt (enum ile kolay seçim)
    public enum TriggerEvent { LineCleared, PiecePlaced }  // Yeni event'ler ekleyebilirsin

    public TriggerEvent trigger;

    public override void Enable(Board board)
    {
        if (trigger == TriggerEvent.LineCleared)
            GameEvents.OnLineCleared += OnTriggered;
        // Diğer event'ler için ekle (örn. GameEvents.OnPiecePlaced += ...)
    }

    public override void Disable(Board board)
    {
        if (trigger == TriggerEvent.LineCleared)
            GameEvents.OnLineCleared -= OnTriggered;
    }

    private void OnTriggered(int value)  // LineCleared için örnek
    {
        Debug.Log($"{powerName} event tetiklendi! Değer: {value}");
        // Özel mantık (örn. altın ver)
    }
}
