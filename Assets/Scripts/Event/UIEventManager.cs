

using System;
using System.Collections.Generic;

public static class UIEventManager
{
    public static Func<TetrominoData?> OnDrawCard;

    public static Action DeckInitialized;

    public static Func<List<TetrominoData>> GetDrawnCards;

}