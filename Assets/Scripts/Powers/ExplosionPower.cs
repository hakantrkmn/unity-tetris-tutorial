using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Explosion Power", menuName = "Tetris/Powers/Explosion")]
public class ExplosionPower : PiecePower
{
    [Tooltip("Patlama yarıçapı")]
    public int radius = 2;


    public override void Activate()
    {
        var board = GameManager.Instance.board;
        var activationPosition = board.currentPowerTilePosition;
        // Artık 'piece.position' kullanmıyoruz!
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3Int targetPos = new Vector3Int(activationPosition.x + x, activationPosition.y + y, 0);
                if (board.tetronimoBoardController.HasTile(targetPos))
                {
                    board.powerClearTiles.Add(targetPos);
                }
            }
        }

        Debug.Log($"{powerName} tetiklendi!");

    }

    public override void ApplyToDeck(List<TetrominoData> deck)
    {
        var cards = deck.Where(x => x.tetromino == tetromino).ToList();
        cards.ForEach(x => x.specialPowers.Add(this));
        Debug.Log($"{powerName} kartlara uygulandı!");
    }


}
