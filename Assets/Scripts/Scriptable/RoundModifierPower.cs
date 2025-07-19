using UnityEngine;

public abstract class RoundModifierPower : ScriptableObject
{
    public string powerName;
    [TextArea] public string description;

    // Tur başladığında çağrılır, event'lere abone olur.
    public abstract void Enable(Board board);
    // Tur bittiğinde çağrılır, event'lerden aboneliği kaldırır.
    public abstract void Disable(Board board);
}