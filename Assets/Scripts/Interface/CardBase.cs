using UnityEngine;

public class CardBase : MonoBehaviour
{
    public virtual void SetCard(TetrominoData tetromino)
    { }

    public virtual void SetCard(PowerBase power)
    { }

    public Slot slot;
}
