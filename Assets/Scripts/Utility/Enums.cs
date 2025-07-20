using System.Collections.Generic;
using UnityEngine;

public enum GameStates
{
    OnPrepare,
    OnAnimation,
    OnGame,

}

[System.Serializable]
public struct PlacedTile
{
    public List<Vector3Int> position;
    public TetrominoData data;
}
