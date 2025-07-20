using UnityEngine;

[CreateAssetMenu(fileName = "Explosion Power", menuName = "Tetris/Powers/Explosion")]
public class ExplosionPower : PiecePower
{
    [Tooltip("Patlama yarıçapı")]
    public int radius = 1;


    public override void Activate(Board board, Vector3Int activationPosition)
    {
        // Artık 'piece.position' kullanmıyoruz!
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3Int targetPos = new Vector3Int(activationPosition.x + x, activationPosition.y + y, 0);
                if (board.Bounds.Contains((Vector2Int)targetPos))
                {
                    board.tilemap.SetTile(targetPos, null);
                    board.placedTiles.Remove(targetPos);
                }
            }
        }

        Debug.Log($"{powerName} tetiklendi!");
    }
}
