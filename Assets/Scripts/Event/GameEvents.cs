using System;

public static class GameEvents
{
    // Bir sıra temizlendiğinde tetiklenir. Parametre: temizlenen sıra sayısı.
    public static event Action<int> OnLineCleared;
    public static event Action<Piece, Board> OnPiecePlaced;  // Yeni: Parça düştüğünde

    public static void TriggerLineCleared(int lines)
    {
        OnLineCleared?.Invoke(lines);
    }

    public static void TriggerPiecePlaced(Piece piece, Board board)
    {
        OnPiecePlaced?.Invoke(piece, board);
    }

    // Oyuncunun kaynaklarını (altın vb.) tutacak bir sisteme de ihtiyacın olacak.
    // public static event Action<int> OnGoldGained;

    public static Action<JokerUICard> JokerCardBought;
}
