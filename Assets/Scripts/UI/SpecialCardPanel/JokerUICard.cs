using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class JokerUICard : MonoBehaviour ,IPointerClickHandler,IEndDragHandler,IDragHandler
{
    public Image artwork;
    public SpecialPower power;
    public RoundModifierPower roundPower;


    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("JokerUICard clicked");
    }

}
