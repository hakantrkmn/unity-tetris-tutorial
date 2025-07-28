using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TooltipController : MonoBehaviour
{
    public TextMeshProUGUI description;
    public CanvasGroup canvasGroup;

    // Update is called once per frame
    void Update()
    {
        var offset = new Vector2(transform.GetComponent<RectTransform>().sizeDelta.x / 2, transform.GetComponent<RectTransform>().sizeDelta.y / 2);
        transform.position = Input.mousePosition + (Vector3)offset;
    }

    public void ShowTooltip(List<PowerBase> powers)
    {
        if (powers.Count == 0)
        {
            HideTooltip();
            return;
        }
        SetTooltip(powers);
        canvasGroup.alpha = 1;
    }

    public void HideTooltip()
    {
        canvasGroup.alpha = 0;
    }

    public void SetTooltip(List<PowerBase> powers)
    {
        description.text = powers.Select(p => p.powerName).Aggregate((a, b) => a + "\n" + b);
    }


    private void OnEnable()
    {
        UIEventManager.ShowTooltip += ShowTooltip;
        UIEventManager.HideTooltip += HideTooltip;
    }

    private void OnDisable()
    {
        UIEventManager.ShowTooltip -= ShowTooltip;
        UIEventManager.HideTooltip -= HideTooltip;
    }
}
