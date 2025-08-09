using UnityEngine;

/// <summary>
/// Game-wide constants to avoid magic numbers and improve maintainability
/// </summary>
public static class GameConstants
{
    #region Board Settings
    public const int BOARD_WIDTH = 10;
    public const int BOARD_HEIGHT = 20;
    public const int SPAWN_POSITION_X = -1;
    public const int SPAWN_POSITION_Y = 8;
    public const int SPAWN_POSITION_Z = 0;
    #endregion

    #region Piece Movement
    public const float DEFAULT_STEP_DELAY = 1f;
    public const float DEFAULT_MOVE_DELAY = 0.1f;
    public const float DEFAULT_LOCK_DELAY = 0.5f;
    #endregion

    #region Rotation
    public const float ROTATION_OFFSET = 0.5f;
    public const int ROTATION_STATES = 4;
    #endregion

    #region Multipliers
    public const float DEFAULT_SPEED_MULTIPLIER = 1f;
    public const float DEFAULT_SCORE_MULTIPLIER = 1f;
    public const float MIN_SPEED_MULTIPLIER = 0.1f;
    #endregion

    #region UI & Deck
    public const int DEFAULT_START_CARD_COUNT = 10;
    public const int DEFAULT_DECK_SIZE = 4;
    #endregion

    #region Power System
    public const int DEFAULT_BLOCKS_PER_STACK = 20;
    public const float DEFAULT_SPEED_INCREASE_PER_STACK = 0.02f;
    public const float DEFAULT_SCORE_INCREASE_PER_STACK = 0.1f;
    #endregion
}
