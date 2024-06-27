using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    #region Instance

    private static GameManager _instance;

    public static GameManager Instance
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

    public static event Action OnStartBattleEvent;

    [SerializeField] private TextMeshProUGUI _winnerText;
    [SerializeField] private RectTransform _topBar;
    [SerializeField] private RectTransform _bottomBar;

    private Units[] _allUnits;

    public Units[] AllUnits => _allUnits;

    private Units.Team _teamWinner;

    public bool battleDone = false;

    private void Start()
    {
        OnStartBattleEvent?.Invoke();
    }


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
            _winnerText.text = "RIVALS WINS";
            Debug.Log("Winner: " + _teamWinner);
        }
        
        if (teamBCount <= 0)
        {
            battleDone = true;
            _teamWinner = Units.Team.TeamA;
            _winnerText.text = "HEROES WINS";
            Debug.Log("Winner: " + _teamWinner);
        }

        if (battleDone)
        {
            foreach (var unit in _allUnits)
            {
                if (unit.isAlive)
                {
                    unit.animator.CrossFade("Idle", 0.2f);
                    unit.agent.speed = 0;
                }
            }

            _topBar.DOScaleY(1, 2);
            _bottomBar.DOScaleY(1, 2);
            _winnerText.DOFade(1, 2);
        }
    }
}
