using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Lazer Drill Power", menuName = "Tetris/Powers/Lazer Drill")]
public class LazerDrill : PiecePower
{
    public override void Enable(Board board)
    {
        GameEvents.OnPiecePlaced += ActivateDrill;
    }

    public override void Disable(Board board)
    {
        GameEvents.OnPiecePlaced -= ActivateDrill;
    }


    public void ActivateDrill(Piece piece, Board board)
    {
        if (!piece.data.specialPowers.Contains(this))
        {
            return;
        }
        // Adım 1: Parçanın kapladığı benzersiz sütunları bul.
        // HashSet, dikey parçanın aynı sütunu 4 kez eklemesini engeller.
        HashSet<int> columnsToDrill = new HashSet<int>();
        foreach (var cell in piece.cells)
        {
            columnsToDrill.Add((cell + piece.position).x);
        }

        // Adım 2: Bulunan her benzersiz sütun için delme işlemi yap.
        foreach (int columnX in columnsToDrill)
        {
            // Adım 3: Bu sütundaki parçanın en alçak noktasını bul.
            int lowestYInColumn = int.MaxValue;
            foreach (var cell in piece.cells)
            {
                Vector3Int pieceCellPosition = cell + piece.position;
                if (pieceCellPosition.x == columnX && pieceCellPosition.y < lowestYInColumn)
                {
                    lowestYInColumn = pieceCellPosition.y;
                }
            }

            // Adım 4: Parçanın en alt noktasının BİR ALTINDAN başlayarak del.
            // Bu, parçanın kendini yok etmesini önler.
            for (int y = lowestYInColumn - 1; y >= board.Bounds.yMin; y--)
            {
                Vector3Int positionToClear = new Vector3Int(columnX, y, 0);
                board.powerClearTiles.Add(positionToClear);
            }
        }

        Debug.Log($"{powerName} tetiklendi ve sütunlar temizlendi!");
    }

    public override void ApplyToDeck(List<TetrominoData> deck)
    {
        Debug.Log($"{powerName} kartlara uygulandı!");
        var cards = deck.Where(x => x.tetromino == tetromino).ToList();
        cards.ForEach(x => x.specialPowers.Add(this));
    }
}
