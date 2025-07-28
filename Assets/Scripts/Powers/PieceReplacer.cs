using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PieceReplacer", menuName = "Powers/PieceReplacer")]
public class PieceReplacer : PowerBase
{
    public override void Activate()
    {
        var board = GameManager.Instance.board;
        var piece = board.activePiece;
        piece.ReplaceWithRandomCard();
    }

}
