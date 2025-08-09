using System;
using System.Collections.Generic;

public static class GameEvents
{
    // Bir sıra temizlendiğinde tetiklenir. Parametre: temizlenen sıra sayısı.
    public static event Action<int, List<TetrominoData>> OnLineCleared;
    public static event Action<Piece, Board> OnPiecePlaced;  // Yeni: Parça düştüğünde

    public static void TriggerLineCleared(int lines, List<TetrominoData> clearedLineTiles)
    {
        OnLineCleared?.Invoke(lines, clearedLineTiles);
    }

    public static void TriggerPiecePlaced(Piece piece, Board board)
    {
        OnPiecePlaced?.Invoke(piece, board);
    }

    public static Action<JokerUICard> JokerCardBought;

    public static Action GameStateOver;

    public static Func<TetrominoData> AddRandomCardToDeck;
    public static Func<TetrominoData> GetRandomCardFromDeck;

    public static Action<TetrominoData> PieceCleared;

    public static Action<PowerBase> TarotCardUsed;

    public static Action GameCanStart;

}
