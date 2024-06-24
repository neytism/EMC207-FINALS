using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    private Units.Team _teamWinner;

    public bool battleDone = false;
    

    private void OnEnable()
    {
        Units.OnDeathStaticEvent += CheckWinner;
    }
    
    private void OnDisable()
    {
        Units.OnDeathStaticEvent -= CheckWinner;
    }

    private void CheckWinner()
    {
        int teamACount = 0;
        int teamBCount = 0;
        
        foreach (var unit in _allUnits)
        {
            if (unit.team == Units.Team.TeamA && unit.isAlive)
            {
                teamACount++;
            }
            
            if (unit.team == Units.Team.TeamB && unit.isAlive)
            {
                teamBCount++;
            }
        }
        
        Debug.Log("Team A left: " + teamACount + " Team B left: " + teamBCount);

        if (teamACount <= 0)
        {
            battleDone = true;
            _teamWinner = Units.Team.TeamB;
            Debug.Log("Winner: " + _teamWinner);
        }
        
        if (teamBCount <= 0)
        {
            battleDone = true;
            _teamWinner = Units.Team.TeamA;
            Debug.Log("Winner: " + _teamWinner);
        }
    }
}
