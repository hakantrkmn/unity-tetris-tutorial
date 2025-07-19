using UnityEngine;

[CreateAssetMenu(fileName = "New Global Power", menuName = "Tetris/Powers/Global Power")]
public class GlobalPower : PowerBase
{
    [Tooltip("Düşme hızı çarpanı (örn. 2 için iki kat hızlı)")]
    public float speedMultiplier = 1f;

    public override void ApplyGlobal(Board board)
    {
        // Örnek: Düşme hızını değiştir (Piece'deki stepDelay'i etkile)
        Piece piece = board.activePiece;  // Veya global bir referans kullan
        piece.stepDelay /= speedMultiplier;
        Debug.Log($"{powerName} uygulandı: Hız {speedMultiplier} kat arttı!");
    }

    public override void RevertGlobal(Board board)
    {
        Piece piece = board.activePiece;
        piece.stepDelay *= speedMultiplier;  // Geri al
    }
}
