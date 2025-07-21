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
}

public enum ButtonType
{
    Reroll,
    Play,
    Buy,

    Discard,
}

[System.Serializable]
public struct PlacedTile
{
    public List<Vector3Int> position;
    public TetrominoData data;
}
