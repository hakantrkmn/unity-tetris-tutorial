using UnityEngine;

[CreateAssetMenu(fileName = "Ghost On", menuName = "Tetris/Powers/Ghost On")]
public class GhostOn : PowerBase
{
    public override void ApplyGlobal(Board board)
    {
        GameModifierManager.Instance.EnableGhost();
        GameManager.Instance.ChangeDropSpeed(0.5f);
    }
}
