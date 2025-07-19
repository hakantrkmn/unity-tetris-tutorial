using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;
using System.Linq;

[DefaultExecutionOrder(-1)]
public class Board : SerializedMonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }

    public TetrominoList tetrominoesList;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

    [SerializeField]
    public Dictionary<Vector3Int, TetrominoData> placedTiles = new Dictionary<Vector3Int, TetrominoData>();

    public List<TetrominoData> deck;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();
        placedTiles = new Dictionary<Vector3Int, TetrominoData>();

        for (int i = 0; i < tetrominoesList.tetrominoes.Length; i++) {
            tetrominoesList.tetrominoes[i].Initialize();
            Debug.Log("Tetromino: " + tetrominoesList.tetrominoes[i].cells.Length);
        }
    }

    [Button]
    public void DrawCard()
    {
        deck = UIEventManager.GetDrawnCards?.Invoke();
        if (deck != null)
        {
            deck.First().Initialize();
            SpawnPiece();
        }
    }

    public void SpawnPiece()
    {
        Debug.Log("Spawning piece: " + deck.First().tetromino);
        Debug.Log("Cells: " + deck.First().cells.Length);
        activePiece.Initialize(this, spawnPosition, deck.First());
        if (IsValidPosition(activePiece, spawnPosition)) {
            Set(activePiece);
        } else {
            GameOver();
        }
    }

    public void GameOver()
    {
        tilemap.ClearAllTiles();

        // Do anything else you want on game over here..
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void SetPlacedTile(Piece piece)
    {
        deck.Remove(piece.data);

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            placedTiles[tilePosition] = piece.data;
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // An out of bounds tile is invalid
            if (!bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (tilemap.HasTile(tilePosition)) {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        // Clear from bottom to top
        while (row < bounds.yMax)
        {
            // Only advance to the next row if the current is not cleared
            // because the tiles above will fall down when a row is cleared
            if (IsLineFull(row)) {
                LineClear(row);
            } else {
                row++;
            }
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (!tilemap.HasTile(position)) {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        // Clear all tiles in the row
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
            if (placedTiles.ContainsKey(position)) {
                Debug.Log("Removed tile at " + placedTiles[position].tetromino);
                if (placedTiles[position].specialPower != null)
                {
                    Debug.Log("Special power: " + placedTiles[position].specialPower.name);
                }
                placedTiles.Remove(position);
            }
        }

        // Shift every row above down one
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
        GameEvents.TriggerLineCleared(1);

    }


    private void OnEnable()
    {
        UIEventManager.PlayButtonClicked += DrawCard;
    }

    private void OnDisable()
    {
        UIEventManager.PlayButtonClicked -= DrawCard;
    }

}
