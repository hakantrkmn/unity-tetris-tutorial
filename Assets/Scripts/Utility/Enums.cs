using System.Collections.Generic;
using UnityEngine;

public enum GameStates
{
    OnPrepare,
    OnAnimation,
    OnGame,

}

public enum SlotTypes
{
    JokerBuy,
    JokerInUse,
    TarotBuy,
    TarotInUse,

    Deck,
}

public enum ButtonType
{
    Reroll,
    Play,
    Buy,

    Discard,

    SellJokerCard,
}

[System.Serializable]
public struct PlacedTile
{
    public List<Vector3Int> position;
    public TetrominoData data;
}
