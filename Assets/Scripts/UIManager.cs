using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _hudHolder;
    [SerializeField] private GameObject _vignette;
    
    [SerializeField] private Image _teamAHealthBar;
    [SerializeField] private Image _teamBHealthBar;
    
    private int _totalTeamACount;
    private int _totalTeamBCount;

    #region Instance

    private static UIManager _instance;

    public static UIManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    #endregion
    
    public void SetTotalTeamCount(int teamA, int teamB)
    {
        _totalTeamACount = teamA;
        _totalTeamBCount = teamB;
    }

    public void UpdateTeamHealth(int teamA, int teamB)
    {
        _teamAHealthBar.fillAmount = (float)teamA / _totalTeamACount;
        _teamBHealthBar.fillAmount = (float)teamB / _totalTeamBCount;
    }

    public void HideHud()
    {
        _hudHolder.SetActive(false);
    }

    public void ToggleSlowVignette(bool isSlow)
    {
        _vignette.SetActive(isSlow);
    }


}
