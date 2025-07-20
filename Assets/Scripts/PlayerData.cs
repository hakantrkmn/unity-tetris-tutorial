using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    #region Singleton
    private static PlayerData _instance;
    public static PlayerData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerData>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("PlayerData");
                    _instance = go.AddComponent<PlayerData>();
                }
            }
            return _instance;
        }
    }
    #endregion
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

}
