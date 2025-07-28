using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class ScorePanelManager : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;

    public int currentGold;
    public int currentScore;
    public int currentLevel;


    private void Start()
    {
        UpdateScorePanel();
        currentGold = GameManager.Instance.gameSession.gold;
        currentScore = GameManager.Instance.gameSession.score;
        currentLevel = GameManager.Instance.gameSession.level;
    }
    [Button]
    public void UpdateScorePanel()
    {
        DOTween.Kill("ScorePanelAnimation");

        // Mevcut UI değerlerini al

        // Hedef değerleri al
        int targetGold = GameManager.Instance.gameSession.gold;
        int targetScore = GameManager.Instance.gameSession.score;
        int targetLevel = GameManager.Instance.gameSession.level;

        // Animasyonları paralel olarak çalıştır
        DOTween.To(() => currentGold, (x) => goldText.text = x.ToString(), targetGold, 0.5f).SetId("ScorePanelAnimation");
        DOTween.To(() => currentScore, (x) => scoreText.text = x.ToString(), targetScore, 0.5f).SetId("ScorePanelAnimation");
        DOTween.To(() => currentLevel, (x) => levelText.text = "Level: " + x.ToString(), targetLevel, 0.5f).SetId("ScorePanelAnimation");
    }

    private void OnEnable() {
        UIEventManager.UpdateScorePanel += UpdateScorePanel;
    }

    private void OnDisable() {
        UIEventManager.UpdateScorePanel -= UpdateScorePanel;
    }
}
