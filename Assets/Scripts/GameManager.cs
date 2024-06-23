using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Instance

    private static GameManager _instance;
    public static  GameManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
            _allUnits = FindObjectsOfType<Units>();
        }
        else
        {
            Destroy(this);
        }
    }

    #endregion

    private Units[] _allUnits;

    public Units[] AllUnits => _allUnits;

}
