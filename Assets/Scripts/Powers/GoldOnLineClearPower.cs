using UnityEngine;

[CreateAssetMenu(fileName = "Gold On Line Clear", menuName = "Tetris/Powers/Gold On Line Clear")]
public class GoldOnLineClearPower : PowerBase
{
    public int goldPerLine = 5;

    public override void Enable(Board board)
    {
        GameEvents.OnLineCleared += GrantGold;
        Debug.Log($"{powerName} aktif!");
    }

    public override void Disable(Board board)
    {
        GameEvents.OnLineCleared -= GrantGold;
        Debug.Log($"{powerName} devre dışı!");
    }

    private void GrantGold(int linesCleared)
    {
        int totalGold = linesCleared * goldPerLine;
        Debug.Log($"{totalGold} altın kazanıldı!");
        // Burada oyuncunun altınını arttıran kodu çağırırsın.
        // PlayerStats.Instance.AddGold(totalGold);
    }
}
