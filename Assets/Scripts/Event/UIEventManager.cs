

using System;
using System.Collections.Generic;

public static class UIEventManager
{
    public static Func<TetrominoData?> OnDrawCard;

    public static Action DeckInitialized;

    public static Func<List<TetrominoData>> GetDrawnCards;

    public static Action RerollButtonClicked;

    public static Action<ButtonType> UpdateButtonText;
    public static Action Reroll;

    public static Action UpdateScorePanel;

    public static Action PlayButtonClicked;

    public static Action DiscardButtonClicked;

    public static Action<PowerBase> JokerCardBought;

}

