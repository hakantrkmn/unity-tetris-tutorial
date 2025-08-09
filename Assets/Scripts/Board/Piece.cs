using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data;
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    public float moveDelay = GameConstants.DEFAULT_MOVE_DELAY;
    public float lockDelay = GameConstants.DEFAULT_LOCK_DELAY;

    private float stepTime;
    private float moveTime;
    private float lockTime;


    public bool isReplaced = false;

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.data = data;
        this.board = board;
        this.position = position;
        this.data.tile.color = data.color.color;
        rotationIndex = 0;
        stepTime = Time.time + GameManager.Instance.gameSession.stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;
        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    public virtual void Update()
    {
        if (GameManager.Instance.gameSession.gameState != GameStates.OnGame || board == null)
        {
            return;
        }

        board.ClearPieceOnBoard(this);


        // Handle rotation
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(1);
        }

        // Handle hard drop
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
            return; // Parça kilitlendi, bu frame'de başka işlem yapma.
        }

        // Allow the player to hold movement keys
        if (Time.time > moveTime)
        {
            HandleMoveInputs();
        }

        // Advance the piece to the next row every x seconds
        if (Time.time > stepTime)
        {
            Step();
        }

        // Parçanın daha aşağı inip inemediğini kontrol et.
        if (!board.IsValidPosition(this, position + Vector3Int.down))
        {
            // Zemine değiyorsa, kilitlenme sayacını başlat/arttır.
            lockTime += Time.deltaTime;

            if (lockTime >= lockDelay)
            {
                Lock();
                return; // Parça kilitlendi, bu frame'de başka işlem yapma.
            }
        }
        else
        {
            // Zemine değmiyorsa (düşüyorsa), sayaç sıfırlanır.
            lockTime = 0f;
        }

        board.SetPieceOnBoard(this);
    }

    private void HandleMoveInputs()
    {
        // Soft drop movement
        if (Input.GetKey(KeyCode.S))
        {
            if (Move(Vector2Int.down))
            {
                // Hızlı indirme yapıldığında da zamanlayıcıyı çarpanı dikkate alarak sıfırlıyoruz.
                ResetStepTime();
            }
        }

        // Left/right movement
        if (Input.GetKey(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Move(Vector2Int.right);
        }
    }
    private void ResetStepTime()
    {
        // GameManager'dan güncel hız çarpanını alıyoruz. Eğer GameManager yoksa, varsayılan olarak 1 kullanıyoruz.
        float currentMultiplier = (GameManager.Instance != null) ? GameManager.Instance.gameSession.speedMultiplier : GameConstants.DEFAULT_SPEED_MULTIPLIER;

        // Bir sonraki düşme zamanını, temel gecikmeyi çarpanla ayarlayarak hesaplıyoruz.
        // speedMultiplier < 1 olacağı için (örn. 0.8), gecikme azalır ve parça hızlanır.
        stepTime = Time.time + (GameManager.Instance.gameSession.stepDelay * currentMultiplier);
    }
    private void Step()
    {
        // stepTime'ı artık yeni metodumuzla, çarpanı dikkate alarak sıfırlıyoruz.
        ResetStepTime();

        // Step down to the next row
        Move(Vector2Int.down);

        // Kilitlenme kontrolü buradan kaldırıldı ve Update'e taşındı.
    }

    public void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }
        Lock();
    }

    public virtual void Lock()
    {
        board.SetPieceOnBoard(this);
        if (isReplaced)
        {
            GameManager.Instance.ChangeDropSpeed(-0.5f);
            isReplaced = false;
        }
        GameEvents.TriggerPiecePlaced(this, board);
        board.ClearLines();

    }

    [Button]
    public void ReplaceWithRandomCard()
    {
        board.ClearPieceOnBoard(this);
        isReplaced = true;
        GameManager.Instance.ChangeDropSpeed(0.5f);
        // DeckPanelManager'dan random kart al
        TetrominoData newCard = GameEvents.GetRandomCardFromDeck?.Invoke();

        if (newCard != null)
        {
            // Yeni kartı mevcut parçaya ata
            this.data = newCard;

            // Hücreleri yeni karta göre güncelle
            if (cells == null)
            {
                cells = new Vector3Int[data.cells.Length];
            }

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = (Vector3Int)data.cells[i];
            }

            // Rotasyonu sıfırla
            rotationIndex = 0;

            // Pozisyonu geçerli bir konuma ayarla (gerekirse)
            if (!board.IsValidPosition(this, position))
            {
                // Eğer mevcut pozisyon geçersizse, yukarı taşı
                Vector3Int newPosition = position;
                while (!board.IsValidPosition(this, newPosition))
                {
                    newPosition.y++;
                }
                position = newPosition;
            }

            Debug.Log($"Parça {data.tetromino} ile değiştirildi");
        }
        else
        {
            Debug.LogWarning("Random kart alınamadı!");
        }
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        // Only save the movement if the new position is valid
        if (valid)
        {
            position = newPosition;
            moveTime = Time.time + moveDelay;
            lockTime = 0f; // reset
        }

        return valid;
    }

    private void Rotate(int direction)
    {
        // Store the current rotation in case the rotation fails
        // and we need to revert
        int originalRotation = rotationIndex;

        // Rotate all of the cells using a rotation matrix
        rotationIndex = Wrap(rotationIndex + direction, 0, GameConstants.ROTATION_STATES);
        ApplyRotationMatrix(direction);

        // Revert the rotation if the wall kick tests fail
        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix;

        // Rotate all of the cells using the rotation matrix
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    // "I" and "O" are rotated from an offset center point
                    cell.x -= GameConstants.ROTATION_OFFSET;
                    cell.y -= GameConstants.ROTATION_OFFSET;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

}
