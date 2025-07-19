using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public UpgradeData upgradeData;

    private void OnEnable() {
        GetDataEvents.GetUpgradeData += () => upgradeData;
    }

    private void OnDisable() {
        GetDataEvents.GetUpgradeData -= () => upgradeData;
    }
}
