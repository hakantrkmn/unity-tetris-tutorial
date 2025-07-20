using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparePanelManager : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    private void OnValidate() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        UIEventManager.PlayButtonClicked += Hide;
    }

    private void OnDisable()
    {
        UIEventManager.PlayButtonClicked -= Hide;
    }
}
