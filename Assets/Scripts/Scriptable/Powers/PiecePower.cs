using UnityEngine;

[CreateAssetMenu(fileName = "New Piece Power", menuName = "Tetris/Powers/Piece Power")]
public class PiecePower : PowerBase
{
    // Parça yerleştirildiğinde (düştüğünde) otomatik çağrılır
    public override void Activate(Piece piece, Board board)
    {
        // Örnek: Patlama gibi bir şey yap
        Debug.Log($"{powerName} parçası düştüğünde tetiklendi!");
        // Buraya özel mantık ekle (örn. etraftaki tile'ları sil)
    }
}
