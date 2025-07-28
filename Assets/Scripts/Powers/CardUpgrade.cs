

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Upgrade", menuName = "Tetris/Powers/Card Upgrade")]
public class CardUpgrade : PiecePower
{

    public override void ApplyToDeck(List<TetrominoData> deck)
    {
        var cards = deck.Where(x => x.tetromino == tetromino).ToList();
        cards.ForEach(x => x.level += 1);

    }
}
