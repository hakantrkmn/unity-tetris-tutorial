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
    public GameData gameData;
    public TetrominoList tetrominoesList;
    public Vector2Int boardSize = new Vector2Int(GameConstants.BOARD_WIDTH, GameConstants.BOARD_HEIGHT);
    public TetrominoBoardController tetronimoBoardController;
    public List<TetrominoData> deck;

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

        for (int i = 0; i < tetrominoesList.tetrominoes.Length; i++) {
            tetrominoesList.tetrominoes[i].Initialize();
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
            //temporary. remove this after testing
            deck = UIEventManager.GetDrawnCards?.Invoke();
        }
        activePiece.Initialize(this, gameData.spawnPosition, deck.First());
        if (IsValidPosition(activePiece, gameData.spawnPosition)) {
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
#region Set and Clear
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
            tetronimoBoardController.SetTetronimoPosition(tilePosition, piece.data);
        }
    }
    public void ClearPlacedTile(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tetronimoBoardController.ClearTetronimoPosition(tilePosition);
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
#endregion
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
        if (fullRows.Count == 0)
        {
            SpawnPiece();
            return;
        }

        Debug.Log("ClearLines started");
        GameManager.Instance.gameSession.gameState = GameStates.OnAnimation;
        ClearRowsAndTriggerPowers(fullRows).OnComplete(() => {
            ShiftRemainingRows(fullRows);
            GameEvents.TriggerLineCleared(fullRows.Count);
            SpawnPiece();
            Debug.Log("ClearLines completed");
            GameManager.Instance.gameSession.gameState = GameStates.OnGame;
        });
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

    public List<Vector3Int> GetPowerClearTiles(List<int> fullRows)
    {
        RectInt bounds = Bounds;
        List<Vector3Int> tiles = new List<Vector3Int>();
        foreach (int row in fullRows)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);

                // Güçleri tetikle
                tiles.AddRange(TriggerTilePowers(position));

            }
        }
        return tiles;
    }

    private Sequence ClearRowsAndTriggerPowers(List<int> fullRows)
    {
        RectInt bounds = Bounds;

        var sequence = DOTween.Sequence();
        var powerClearTiles = GetPowerClearTiles(fullRows);
        foreach (var tile in powerClearTiles)
        {
            ClearTile(tile);
        }

        sequence.AppendInterval(0.5f);

        foreach (int row in fullRows)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);
                if (powerClearTiles.Contains(position))
                {
                    continue;
                }
                // Artık bu satır yerine yeni metodumuzu çağırıyoruz.
                // tilemap.GetTile(position).DOFade(0, 0.5f); // ESKİ YANLIŞ KOD
                sequence.Append(FadeAndClearTile(position)); // YENİ DOĞRU KOD
                // Bu satırları da siliyoruz, çünkü bu işi artık OnComplete callback'i yapıyor.
                // tilemap.SetTile(position, null);
                // placedTiles.Remove(position);
            }
        }
        return sequence;
    }

    private List<Vector3Int> TriggerTilePowers(Vector3Int position)
    {
        List<Vector3Int> tiles = new List<Vector3Int>();
        if (tetronimoBoardController.GetTetronimoPosition(position) != null)
        {
            var data = tetronimoBoardController.GetTetronimoPosition(position);
            if (data.specialPowers != null && data.specialPowers.Count > 0)
            {
                var explosionPowers = data.specialPowers.OfType<ExplosionPower>().ToList();
                foreach (var power in explosionPowers)
                {
                    tiles.AddRange(power.Activate(this, position));
                }
            }
        }
        return tiles;
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
            if (!tilemap.HasTile(position)) {
                return false;
            }
        }

        return true;
    }


    private void OnEnable()
    {
        UIEventManager.PlayButtonClicked += DrawCard;
    }

    private void OnDisable()
    {
        UIEventManager.PlayButtonClicked -= DrawCard;
    }

    private void ClearTile(Vector3Int position)
    {
        tilemap.SetTile(position, null);
        tetronimoBoardController.ClearTetronimoPosition(position);
    }

    private Tween FadeAndClearTile(Vector3Int position)
    {
        // Önemli: Önce o pozisyonda bir tile olduğundan emin olalım.
        if (!tilemap.HasTile(position)) return null;

        // Hedef renk: Mevcut rengin aynısı ama alfası 0 (tamamen şeffaf).
        Color targetColor = new Color(1, 1, 1, 0);

        // DOTween.To() sihri burada başlıyor!
        return DOTween.To(
            // 1. Getter: Hangi değeri animasyonla değiştireceğiz? -> O pozisyondaki tile'ın rengini.
            () => tilemap.GetColor(position),

            // 2. Setter: Her animasyon adımında bu değerle ne yapacağız? -> O pozisyondaki tile'ın rengini yeni değerle güncelleyeceğiz.
            (color) => tilemap.SetColor(position, color),

            // 3. End Value: Animasyon bittiğinde değer ne olmalı? -> Şeffaf renk.
            targetColor,

            // 4. Duration: Animasyon ne kadar sürecek?
            0.05f
        ).OnComplete(() => {
            // 5. OnComplete: Animasyon BİTTİĞİNDE ne olacak? -> Tile'ı ve verisini tamamen yok et.
            tilemap.SetTile(position, null);
            tetronimoBoardController.ClearTetronimoPosition(position);
            // Not: İsteğe bağlı ama iyi bir pratik: Gelecekte o slota konacak tile'ların
            // şeffaf olmaması için rengi varsayılana sıfırla.
            tilemap.SetColor(position, Color.white);
        });
    }
}
