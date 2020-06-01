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
            }
            _singleManager = value;
        }
    }
    private static TanksManager _singleManager;

    [SerializeField] Transform obstaclesContainer = null;
    [SerializeField] private TankCharacter _player = null;
    public PathFinding PathFindingSystem = null;
    public TanksFactory Factory = null;
    public TankCharacter Player 
    {
        get 
        {
            if (_player != null) return _player;

            foreach (BaseTankAI ai in LightTanks) 
            {
                ai.StopTankBehaviors = true;
            }
            foreach (BaseTankAI ai in HeavyTanks)
            {
                ai.StopTankBehaviors = true;
            }

            return null;
        }
    }


    public List<BaseTankAI> LightTanks { get; set; }
    public List<BaseTankAI> HeavyTanks { get; set; }
    public List<int> takenWayPoints { get; set; }
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
    
    //public int SearchingNearestWayPoint(Vector3 position)
    //{
    //    float dist;
    //    int nighborIndex = int.MinValue; float minDistance = float.MaxValue;
    //    position = new Vector3(position.x, position.y, WayPoints[0].z);
    //    for (int i = 0; i < WayPoints.Count; i++)
    //    {
    //        dist = Vector3.Distance(position, WayPoints[i]);
    //        //Debug.Log($"dist = {dist}");
    //        if (minDistance > dist) { minDistance = dist; nighborIndex = i; }
    //        if (0.079f > dist) break;
    //    }
    //    return nighborIndex;
    //}
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
            //tanksStateStr = _tanksState.ToString();
            yield return new WaitForSeconds(timerCheking);
        }
            
    }
    //[SerializeField] string tanksStateStr;
    private PossibleStates _tanksState;
    public enum PossibleStates
    {
        DefendBase, Attack, Surround
    }
    //private void OnDrawGizmos()
    //{
    //    if(WayPoints!=null)
    //    for (int x = 0; x < WayPoints.Count; x++) 
    //    {
    //        for (int y = 0; y < WayPoints.Count; y++) 
    //        {
    //            if(PathTable!=null)
    //            if (PathTable[x, y].Count== 2) 
    //            {
    //                //Gizmos.DrawLine(WayPoints[x], WayPoints[y]);
    //            }
    //        }
    //    }
    //}
}
