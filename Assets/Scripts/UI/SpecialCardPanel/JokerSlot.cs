using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JokerSlot : MonoBehaviour
{
    public bool isSlotEmpty()
    {
        return transform.childCount == 0;
    }
}
