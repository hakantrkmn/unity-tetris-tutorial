using System.Collections.Generic;
using UnityEngine;

public abstract class PowerBase : ScriptableObject
{
    [Tooltip("Gücün adı")]
    public string powerName;

    [Tooltip("Gücün açıklaması")]
    [TextArea]
    public string description;

    public Sprite artwork;
    public CardType cardType;

    // Gücün etkinleştirilmesi için ortak metot (alt sınıflar override eder)
    public virtual List<Vector3Int> Activate(Board board, Vector3Int activationPosition) { return new List<Vector3Int>(); }
    public virtual void Activate(Piece piece, Board board) { }
    // Event bazlı için (Enable/Disable)
    public virtual void Enable(Board board) { }

    public virtual void ApplyToDeck(List<TetrominoData> deck) { }

    public virtual void Disable(Board board) { }

    // Global değişiklikler için (örn. hız değiştirme)
    public virtual void ApplyGlobal(Board board) { }
    public virtual void RevertGlobal(Board board) { }
}

public enum CardType
{
    Joker,
    Tarot,
}
