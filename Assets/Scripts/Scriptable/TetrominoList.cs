using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Tetromino", menuName = "Tetromino")]
public class TetrominoList : ScriptableObject
{
    public TetrominoData[] tetrominoes;
}

public enum Tetromino
{
    I, J, L, O, S, T, Z
}

[System.Serializable]
public class TetrominoData
{
    public TetrominoData(TetrominoData data)
    {
        tile = data.tile;
        artwork = data.artwork;
        tetromino = data.tetromino;
        cells = data.cells;
        wallKicks = data.wallKicks;
        specialPowers = new List<PowerBase>();
    }
    public Tile tile;

    public Sprite artwork;
    public Tetromino tetromino;


    public Vector2Int[] cells { get; private set; }
    public Vector2Int[,] wallKicks { get; private set; }

    public List<PowerBase> specialPowers;


    public void Initialize()
    {
        cells = Data.Cells[tetromino];
        wallKicks = Data.WallKicks[tetromino];
    }

}
