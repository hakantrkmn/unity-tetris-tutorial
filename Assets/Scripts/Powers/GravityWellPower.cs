using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Gravity Well Power", menuName = "Tetris/Powers/Gravity Well")]
public class GravityWellPower : PowerBase // Veya PowerBase'den türetin
{
    [Tooltip("Her kaç blokta bir etkinleşeceği")]
    public int blocksPerStack = 20;

    [Tooltip("Her yığın için hız artışı (örn. 0.02 = %2)")]
    public float speedIncreasePerStack = 0.02f;

    [Tooltip("Her yığın için puan artışı (örn. 0.05 = %5)")]
    public float scoreIncreasePerStack = 0.05f;

    public override void Enable(Board board)
    {
        Debug.Log($"{powerName} etkinleştirildi!");
        // Parça yerleştiğinde UpdatePower metodunu çağır
        GameEvents.OnPiecePlaced += UpdatePower;
    }

    public override void Disable(Board board)
    {
        GameEvents.OnPiecePlaced -= UpdatePower;
    }

    // Bu metot, her parça yerleştiğinde çalışacak
    private void UpdatePower(Piece piece, Board board)
    {
        if (GameManager.Instance == null) return;

        // 1. Tahtadaki blok sayısına göre "yığın" sayısını hesapla
        int stacks = board.placedTiles.Count / blocksPerStack;
        Debug.Log("piece count: " + board.placedTiles.Count);
        // 2. Yeni hız ve puan çarpanlarını hesapla
        // Hız için: 1'den çıkarıyoruz çünkü stepDelay azaldıkça hız artar.
        float newSpeedMultiplier = 1f - (stacks * speedIncreasePerStack);
        float newScoreMultiplier = 1f + (stacks * scoreIncreasePerStack);

        // 3. GameManager'daki genel çarpanları güncelle
        GameManager.Instance.speedMultiplier = Mathf.Max(0.1f, newSpeedMultiplier); // Hızın çok artmasını önle
        GameManager.Instance.scoreMultiplier = newScoreMultiplier;

        Debug.Log($"Yerçekimi Kuyusu: {stacks} yığın aktif. Hız Çarpanı: {newSpeedMultiplier}, Puan Çarpanı: {newScoreMultiplier}");
    }
}
