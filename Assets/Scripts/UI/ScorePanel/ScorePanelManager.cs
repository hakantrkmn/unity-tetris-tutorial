using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class ScorePanelManager : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;

    private void Start()
    {
        UpdateScorePanel();
    }
    [Button]
    public void UpdateScorePanel()
    {
        goldText.text = GameManager.Instance.gameSession.gold.ToString();
        scoreText.text = GameManager.Instance.gameSession.score.ToString();
        levelText.text = "Level: " + GameManager.Instance.gameSession.level.ToString();
    }

    private void OnEnable() {
        UIEventManager.UpdateScorePanel += UpdateScorePanel;
    }

    private void OnDisable() {
        UIEventManager.UpdateScorePanel -= UpdateScorePanel;
    }
}
