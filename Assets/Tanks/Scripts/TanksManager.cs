using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class TanksManager : MonoBehaviour
{
    public static TanksManager SingleManager
    {
        get => _singleManager;
        set
        {
            if (_singleManager != null)
            {
                Debug.LogError("Ошибка попытка присвоить одиночному менеджеру ещё одно значение");
                Destroy(value.gameObject);
            }
            _singleManager = value;
        }
    }
    private static TanksManager _singleManager;

    [SerializeField] Transform obstaclesContainer = null;
    [SerializeField] private TankCharacter _player = null;
    public PathFinding PathFindingSystem = null;
    public Factory Factory = null;
    public TankCharacter Player 
    {
        get 
        {
            if (_player != null) return _player;

            foreach (BaseTankAI ai in LightTanks) 
            {
                ai.StopTankBehaviors = true;
                LightTanks.Remove(ai);
            }
            foreach (BaseTankAI ai in HeavyTanks)
            {
                ai.StopTankBehaviors = true;
                HeavyTanks.Remove(ai);
            }

            return null;
        }
    }


    public List<BaseTankAI> LightTanks { get; set; }
    public List<BaseTankAI> HeavyTanks { get; set; }
    public PossibleStates TanksState { get => _tanksState; set => _tanksState = value; }

    private void Awake()
    {
        TanksManager.SingleManager = this;
        LightTanks = new List<BaseTankAI>();
        HeavyTanks = new List<BaseTankAI>();
    }
    void Start()
    {
        StartCoroutine(ChangeStates(5));
    }
    
    private IEnumerator ChangeStates(float timerCheking)
    {
        while( (LightTanks.Count>0 || HeavyTanks.Count>0 || Factory != null) && Player!=null)
        {
            if (Player!=null && Factory !=null && Vector3.Distance(Player.transform.position, Factory.transform.position) < 0.8f)
            {
                _tanksState = PossibleStates.DefendBase;
            }
            else 
            {
                if (LightTanks.Count + HeavyTanks.Count < 6) _tanksState = PossibleStates.Surround;
                else _tanksState = PossibleStates.Attack;
            }
            yield return new WaitForSeconds(timerCheking);
        }
            
    }
    private PossibleStates _tanksState;
    public enum PossibleStates
    {
        DefendBase, Attack, Surround
    }
}
