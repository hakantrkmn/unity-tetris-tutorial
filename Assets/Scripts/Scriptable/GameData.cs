using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "GameData")]
public class GameData : ScriptableObject
{
    public List<PowerBase> powers;
    public int[] rerollPrices;

    public Vector3Int spawnPosition;

    public List<int> levelScores;

    public float TILE_ANIMATION_SPEED = 0.05f;
}
