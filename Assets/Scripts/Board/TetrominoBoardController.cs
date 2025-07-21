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

    // Matrisi artık yeni pencerede çizdiğimiz için
    // #if UNITY_EDITOR bloğunun tamamını silebilirsiniz.
}
