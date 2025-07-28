using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
using Sirenix.Utilities;
#endif

public class TetrominoBoardController : MonoBehaviour
{
    public Board board;

    public TetrominoData[,] tetronimoBoard;

    public int blockCount;

    private void Awake()
    {
        // Inspector'da board atanmamışsa hata almamak için kontrol edelim.
        if (board != null)
        {
            tetronimoBoard = new TetrominoData[board.boardSize.x, board.boardSize.y];
        }
    }

    public void SetTetronimoPosition(Vector3Int position, TetrominoData data)
    {
        if (tetronimoBoard == null) return;
        tetronimoBoard[position.x + board.boardSize.x / 2, position.y + board.boardSize.y / 2] = data;
        blockCount++;
    }

    public bool HasTile(Vector3Int position)
    {
        if (tetronimoBoard == null) return false;
        //check if position is out of bounds
        if (position.x + board.boardSize.x / 2 < 0 || position.x + board.boardSize.x / 2 >= board.boardSize.x || position.y + board.boardSize.y / 2 < 0 || position.y + board.boardSize.y / 2 >= board.boardSize.y)
        {
            return false;
        }
        return tetronimoBoard[position.x + board.boardSize.x / 2, position.y + board.boardSize.y / 2] != null;
    }

    [Button]
    public void ClearRandomTileFromBoard()
    {
        List<Vector3Int> filledPositions = new List<Vector3Int>();
        for (int y = board.boardSize.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < board.boardSize.x; x++)
            {
                if (tetronimoBoard[x, y] != null)
                {
                    filledPositions.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        if (filledPositions.Count == 0)
            return; // veya default(TetrominoData)

        // Rastgele bir pozisyon seç
        int randomIndex = Random.Range(0, filledPositions.Count);
        Vector3Int pos = filledPositions[randomIndex];
        board.ClearTile(pos - new Vector3Int(board.boardSize.x / 2, board.boardSize.y / 2, 0));
        ClearTetronimoPosition(pos - new Vector3Int(board.boardSize.x / 2, board.boardSize.y / 2, 0));
        Debug.Log("ClearTetronimoPosition: " + pos);

    }

    public void ClearTetronimoPosition(Vector3Int position)
    {
        if (tetronimoBoard == null) return;
        tetronimoBoard[position.x + board.boardSize.x / 2, position.y + board.boardSize.y / 2] = null;
        blockCount--;
    }

    public TetrominoData GetTetronimoPosition(Vector3Int position)
    {
        if (tetronimoBoard == null) return null;

        return tetronimoBoard[position.x + board.boardSize.x / 2, position.y + board.boardSize.y / 2];
    }

    public bool IsTetronimoPositionEmpty(Vector3Int position)
    {
        if (tetronimoBoard == null) return true;
        //check if position is out of bounds
        if (position.x + board.boardSize.x / 2 < 0 || position.x + board.boardSize.x / 2 >= board.boardSize.x || position.y + board.boardSize.y / 2 < 0 || position.y + board.boardSize.y / 2 >= board.boardSize.y)
        {
            return true;
        }
        return tetronimoBoard[position.x + board.boardSize.x / 2, position.y + board.boardSize.y / 2] == null;
    }

    public void ChangeBoardSize(int width, int height)
    {
        var oldBoard = tetronimoBoard;
        var oldWidth = oldBoard?.GetLength(0) ?? 0;
        var oldHeight = oldBoard?.GetLength(1) ?? 0;

        tetronimoBoard = new TetrominoData[width, height];

        // Eğer eski board yoksa, sadece yeni board oluştur
        if (oldBoard == null) return;

        // Eski board'dan yeni board'a veri kopyala
        // Merkez noktalarını hesapla
        int oldCenterX = oldWidth / 2;
        int oldCenterY = oldHeight / 2;
        int newCenterX = width / 2;
        int newCenterY = height / 2;

        // Kopyalanacak alanın sınırlarını belirle
        int copyWidth = Mathf.Min(oldWidth, width);
        int copyHeight = Mathf.Min(oldHeight, height);

        // Eski board'dan yeni board'a veri kopyala
        for (int y = 0; y < copyHeight; y++)
        {
            for (int x = 0; x < copyWidth; x++)
            {
                // Eski board'daki pozisyon
                int oldX = x;
                int oldY = y;

                // Yeni board'daki pozisyon (merkez farkını hesapla)
                int newX = x + (newCenterX - oldCenterX);
                int newY = y + (newCenterY - oldCenterY);

                // Sınırları kontrol et
                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                {
                    tetronimoBoard[newX, newY] = oldBoard[oldX, oldY];
                }
            }
        }

        // Block count'u güncelle
        blockCount = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (tetronimoBoard[x, y] != null)
                {
                    blockCount++;
                }
            }
        }
    }

}
