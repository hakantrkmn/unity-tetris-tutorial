using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public List<SpecialPower> piecePowers;
    public List<RoundModifierPower> roundPowers;
}
