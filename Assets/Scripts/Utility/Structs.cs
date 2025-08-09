using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSession
{
    public GameSession()
    {
        score = 0;
        level = 0;
        rerollValue = 0;
        gold = 0;
        scoreMultiplier = GameConstants.DEFAULT_SCORE_MULTIPLIER;
        speedMultiplier = GameConstants.DEFAULT_SPEED_MULTIPLIER;
        stepDelay = GameConstants.DEFAULT_STEP_DELAY;
        gameState = GameStates.OnPrepare;
    }
    public GameStates gameState = GameStates.OnPrepare;

    public int score;
    public int level;
    public int rerollValue;
    public int gold;
    public float scoreMultiplier;
    public float speedMultiplier;

    public float stepDelay;


}


[System.Serializable]
public class TetrominoColor
{
    public Color color;
    public string colorName;
}
