using UnityEngine;

[CreateAssetMenu(fileName = "Explosion Power", menuName = "Tetris/Powers/Explosion")]
public class ExplosionPower : PowerBase
{
    [Tooltip("Patlama yarıçapı")]
    public int radius = 1;

    public Tetromino tetromino;

    public override void Activate(Piece piece, Board board)
    {
        Vector3Int piecePosition = piece.position;

        // Parçanın merkezini patlama merkezi olarak alalım
        // Daha gelişmiş bir mantıkla parçanın her hücresinin etrafı da taranabilir.
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3Int targetPos = new Vector3Int(piecePosition.x + x, piecePosition.y + y, 0);

                // Patlamanın tahta sınırları içinde olduğundan emin ol
                if (board.Bounds.Contains((Vector2Int)targetPos))
                {
                    board.tilemap.SetTile(targetPos, null);
                }
            }
        }

        Debug.Log($"{powerName} tetiklendi!");
    }
}
