using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour
{
    public SlotTypes slotType;
    public bool isSlotEmpty = true;

    public int index;

    public CardBase card;

    public void Start()
    {
        card = GetComponentInChildren<CardBase>();
    }

    public void OnValidate()
    {
        card = GetComponentInChildren<CardBase>();
        transform.name = slotType.ToString();
    }


}
