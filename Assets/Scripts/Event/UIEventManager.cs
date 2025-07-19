

using System;
using System.Collections.Generic;

public static class UIEventManager
{
    public static Func<TetrominoData?> OnDrawCard;

    public static Action DeckInitialized;

    public static Func<List<TetrominoData>> GetDrawnCards;

    public static Action RerollButtonClicked;

    public static Action UpdateScorePanel;

    public static Action PlayButtonClicked;

}

