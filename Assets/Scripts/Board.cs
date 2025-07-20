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
    public Vector2Int boardSize = new Vector2Int(GameConstants.BOARD_WIDTH, GameConstants.BOARD_HEIGHT);
    public Vector3Int spawnPosition = new Vector3Int(GameConstants.SPAWN_POSITION_X, GameConstants.SPAWN_POSITION_Y, GameConstants.SPAWN_POSITION_Z);

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
        if (deck != null && deck.Count > 0)
        {
            deck.First().Initialize();
            SpawnPiece();
        }
    }

    public void SpawnPiece()
    {
        if (deck.Count == 0)
        {
            deck = UIEventManager.GetDrawnCards?.Invoke();
        }
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
        var fullRows = FindFullRows();
        if (fullRows.Count == 0) return;

        ClearRowsAndTriggerPowers(fullRows);
        ShiftRemainingRows(fullRows);
        GameEvents.TriggerLineCleared(fullRows.Count);
    }

    private List<int> FindFullRows()
    {
        RectInt bounds = Bounds;
        List<int> fullRows = new List<int>();

        for (int row = bounds.yMin; row < bounds.yMax; row++)
        {
            if (IsLineFull(row))
            {
                fullRows.Add(row);
            }
        }

        return fullRows;
    }

    private void ClearRowsAndTriggerPowers(List<int> fullRows)
    {
        RectInt bounds = Bounds;

        foreach (int row in fullRows)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);

                // Trigger powers before clearing
                TriggerTilePowers(position);

                // Clear tile from both visual and data
                tilemap.SetTile(position, null);
                placedTiles.Remove(position);
            }
        }
    }

    private void TriggerTilePowers(Vector3Int position)
    {
        if (placedTiles.TryGetValue(position, out TetrominoData data))
        {
            if (data.specialPowers != null && data.specialPowers.Count > 0)
            {
                var explosionPowers = data.specialPowers.OfType<ExplosionPower>().ToList();
                foreach (var power in explosionPowers)
                {
                    power.Activate(this, position);
                }
            }
        }
    }

    private void ShiftRemainingRows(List<int> clearedRows)
    {
        RectInt bounds = Bounds;
        int writeRow = bounds.yMin;

        for (int readRow = bounds.yMin; readRow < bounds.yMax; readRow++)
        {
            if (clearedRows.Contains(readRow))
            {
                continue;
            }

            ShiftRowData(readRow, writeRow, bounds);
            writeRow++;
        }
    }

    private void ShiftRowData(int fromRow, int toRow, RectInt bounds)
    {
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int fromPos = new Vector3Int(col, fromRow, 0);
            Vector3Int toPos = new Vector3Int(col, toRow, 0);

            // Move visual tile
            tilemap.SetTile(toPos, tilemap.GetTile(fromPos));

            // Move data
            if (placedTiles.TryGetValue(fromPos, out TetrominoData data))
            {
                placedTiles[toPos] = data;
                placedTiles.Remove(fromPos);
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

    /// <summary>
    /// Clears a single line - deprecated, use ClearLines() instead
    /// </summary>
    [System.Obsolete("Use ClearLines() instead for better performance")]
    public void LineClear(int row)
    {
        ClearLines();
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
