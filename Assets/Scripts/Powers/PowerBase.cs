using UnityEngine;

public abstract class PowerBase : ScriptableObject
{
    [Tooltip("Gücün adı")]
    public string powerName;

    [Tooltip("Gücün açıklaması")]
    [TextArea]
    public string description;

    public Sprite artwork;

    // Gücün etkinleştirilmesi için ortak metot (alt sınıflar override eder)
    public virtual void Activate(Piece piece, Board board) { }  // Parça bazlı için

    // Event bazlı için (Enable/Disable)
    public virtual void Enable(Board board) { }
    public virtual void Disable(Board board) { }

    // Global değişiklikler için (örn. hız değiştirme)
    public virtual void ApplyGlobal(Board board) { }
    public virtual void RevertGlobal(Board board) { }
}
