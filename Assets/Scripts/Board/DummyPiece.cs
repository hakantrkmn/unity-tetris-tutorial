using DG.Tweening;
using UnityEngine;

public class DummyPiece : Piece
{

    public override void Update()
    {
        //dummy piece does not move
    }

    public override void Lock()
    {
        board.SetPieceOnBoard(this);

        GameEvents.TriggerPiecePlaced(this, board);
        board.ClearLines(false);

    }


}
