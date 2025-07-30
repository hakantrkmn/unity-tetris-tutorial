using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;
using System.Linq;
using DG.Tweening;
using System.Collections;
[DefaultExecutionOrder(-1)]
public class Board : SerializedMonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public SpriteRenderer gridSpriteRenderer;
    public DummyPiece dummyPiece;
    public GameData gameData;
    public TetrominoList tetrominoesList;
    public Vector2Int boardSize = new Vector2Int(GameConstants.BOARD_WIDTH, GameConstants.BOARD_HEIGHT);
    public TetrominoBoardController tetronimoBoardController;
    public List<TetrominoData> deck;
    public List<Vector3Int> powerClearTiles = new List<Vector3Int>();

    public Vector3Int currentPowerTilePosition;
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    void OnValidate()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();
    }
    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoesList.tetrominoes.Length; i++)
        {
            tetrominoesList.tetrominoes[i].Initialize();
        }
    }

    [Button]
    public void GetDeckAndSpawnpiece()
    {
        deck = UIEventManager.GetDrawnCards?.Invoke();
        if (deck != null && deck.Count > 0)
        {
            deck.First().Initialize();
            SpawnPiece();
        }
    }

    public bool SpawnPiece()
    {
        if (deck.Count == 0)
        {
            //temporary. remove this after testing
            GameEvents.GameStateOver?.Invoke();
            GameManager.Instance.gameSession.gameState = GameStates.OnPrepare;
            return false;
        }
        GameManager.Instance.gameSession.gameState = GameStates.OnGame;
        activePiece.Initialize(this, gameData.spawnPosition, deck.First());
        if (IsValidPosition(activePiece, gameData.spawnPosition))
        {
            SetPieceOnBoard(activePiece);
        }
        else
        {
            GameOver();
            return false;
        }
        return true;
    }

    public void GameOver()
    {
        tilemap.ClearAllTiles();
    }
    #region Set and Clear Piece on Board
    public void SetPieceOnBoard(Piece piece)
    {
        deck.Remove(piece.data);
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
            tetronimoBoardController.SetTetronimoPosition(tilePosition, piece.data);

        }
    }
    public void ClearPieceOnBoard(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
            tetronimoBoardController.ClearTetronimoPosition(tilePosition);
        }
    }
    #endregion
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // An out of bounds tile is invalid
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (tetronimoBoardController.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public void CheckPowerTilesAndClear(List<int> fullRows, bool spawnPiece)
    {
        GameManager.Instance.gameSession.gameState = GameStates.OnAnimation;
        ClearRowsAndTriggerPowers(fullRows).OnComplete(() =>
        {
            powerClearTiles.Clear();
            if (fullRows.Count == 0)
            {
                if (spawnPiece)
                {
                    SpawnPiece();
                }
                return;
            }
            ShiftRemainingRows(fullRows);
            GameEvents.TriggerLineCleared(fullRows.Count);
            if (SpawnPiece())
            {
                Debug.Log("ClearLines completed");
                GameManager.Instance.gameSession.gameState = GameStates.OnGame;
            }

        });
    }

    public void ClearLines(bool spawnPiece = true)
    {
        var fullRows = FindFullRows();
        if (fullRows.Count == 0)
        {
            CheckPowerTilesAndClear(fullRows, spawnPiece);
            return;
        }

        CheckPowerTilesAndClear(fullRows, spawnPiece);
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
    public Sequence ClearPowerTiles()
    {
        var powerSequence = DOTween.Sequence();
        foreach (var tile in powerClearTiles)
        {
            if (tetronimoBoardController.HasTile(tile))
            {
                powerSequence.Join(FadeAndClearTile(tile));
            }
        }
        powerClearTiles.Clear();
        return powerSequence;
    }
    public Sequence GetPowerClearTilesAndClear(List<int> fullRows)
    {
        var powerSequence = DOTween.Sequence();
        RectInt bounds = Bounds;
        foreach (int row in fullRows)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);

                // Güçleri tetikle
                powerSequence.Append(TriggerTilePowers(position));
            }
        }
        return powerSequence;
    }

    private Sequence ClearRowsAndTriggerPowers(List<int> fullRows)
    {
        var mainSequence = DOTween.Sequence();
        if (powerClearTiles.Count > 0)
        {
            mainSequence.Append(ClearPowerTiles());
        }
        RectInt bounds = Bounds;
        mainSequence.Append(GetPowerClearTilesAndClear(fullRows));
        mainSequence.AppendInterval(fullRows.Count > 0 ? 0.05f : 0f);

        foreach (int row in fullRows)
        {
            var lineSequence = DOTween.Sequence();
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);
                if (powerClearTiles.Contains(position))
                {
                    continue;
                }
                if (tetronimoBoardController.HasTile(position))
                {
                    lineSequence.Append(FadeAndClearTile(position));
                }
            }
            mainSequence.Join(lineSequence);
        }
        return mainSequence;
    }

    private Sequence TriggerTilePowers(Vector3Int position)
    {
        var powerSequence = DOTween.Sequence();
        var data = tetronimoBoardController.GetTetronimoPosition(position);
        if (data != null)
        {
            currentPowerTilePosition = position;
            if (data.specialPowers != null && data.specialPowers.Count > 0)
            {
                foreach (var power in data.specialPowers)
                {
                    power.Activate();
                    foreach (var tile in powerClearTiles)
                    {
                        if (tetronimoBoardController.HasTile(tile))
                        {
                            powerSequence.Join(FadeAndClearTile(tile));
                        }
                    }
                    powerClearTiles.Clear();
                }
            }
        }
        return powerSequence;
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

            if (readRow != writeRow)
            {
                ShiftRowData(readRow, writeRow, bounds);
            }
            writeRow++;
        }

        // Clear the top rows that are now empty
        for (int row = writeRow; row < bounds.yMax; row++)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, null);
                if (tetronimoBoardController.GetTetronimoPosition(position) != null)
                {
                    tetronimoBoardController.ClearTetronimoPosition(position);
                }
            }
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
            var data = tetronimoBoardController.GetTetronimoPosition(fromPos);
            tetronimoBoardController.SetTetronimoPosition(toPos, data);
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    [Button]
    public void SpawnRandomCardAndHardDrop()
    {
        var card = GameEvents.GetRandomCardFromDeck?.Invoke();
        dummyPiece.Initialize(this, gameData.spawnPosition, card);
        if (IsValidPosition(dummyPiece, gameData.spawnPosition))
        {
            dummyPiece.HardDrop();
        }

    }



    private void OnEnable()
    {
        GameEvents.GameCanStart += GetDeckAndSpawnpiece;
    }

    private void OnDisable()
    {
        GameEvents.GameCanStart -= GetDeckAndSpawnpiece;
    }

    public void ClearTile(Vector3Int position)
    {
        tilemap.SetTile(position, null);
        tetronimoBoardController.ClearTetronimoPosition(position);
    }

    public Tween FadeAndClearTile(Vector3Int position)
    {
        if (!tetronimoBoardController.HasTile(position)) return null;

        var sequence = DOTween.Sequence();
        sequence.AppendInterval(gameData.TILE_ANIMATION_SPEED);
        sequence.AppendCallback(() =>
        {
            if (!tetronimoBoardController.HasTile(position)) return;
            var data = tetronimoBoardController.GetTetronimoPosition(position);
            GameEvents.PieceCleared?.Invoke(data);
            tilemap.SetTile(position, null);
            tetronimoBoardController.ClearTetronimoPosition(position);
        });
        return sequence;


    }

    [Button]
    public void ChangeBoardSize(int width, int height)
    {
        // Eski board sınırlarını al
        var oldBounds = Bounds;

        // Board size'ı güncelle
        gridSpriteRenderer.size = new Vector2(width, height);
        boardSize = new Vector2Int(width, height);

        // Tetromino board controller'ı güncelle
        tetronimoBoardController.ChangeBoardSize(width, height);

        // Yeni board sınırlarını al
        var newBounds = Bounds;

        // Eski board sınırları dışında kalan tile'ları temizle
        ClearTilesOutsideBounds(oldBounds, newBounds);
    }

    private void ClearTilesOutsideBounds(RectInt oldBounds, RectInt newBounds)
    {
        // Eski board'ın tüm pozisyonlarını kontrol et
        for (int y = oldBounds.yMin; y < oldBounds.yMax; y++)
        {
            for (int x = oldBounds.xMin; x < oldBounds.xMax; x++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                // Eğer pozisyon yeni board sınırları dışındaysa temizle
                if (!newBounds.Contains((Vector2Int)position))
                {
                    // Tile'ı temizle
                    tilemap.SetTile(position, null);

                    // Tetromino data'yı temizle (eğer varsa)
                    if (tetronimoBoardController.HasTile(position))
                    {
                        tetronimoBoardController.ClearTetronimoPosition(position);
                    }
                }
            }
        }
    }
}

