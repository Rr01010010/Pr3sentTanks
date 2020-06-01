//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTank : BaseTankAI
{
    IEnumerator enumerator = null;
    void Start()
    {
    }
    private void FixedUpdate()
    {
        if (enumerator == null) 
        {
            enumerator = UpdatePathToPlayer();
            StartCoroutine(enumerator);
        }

        if(!StopTankBehaviors)
        switch (TanksManager.SingleManager.TanksState)
        {
            case TanksManager.PossibleStates.Attack: Attack(); break;
            case TanksManager.PossibleStates.DefendBase: DefendBase(); break;
            case TanksManager.PossibleStates.Surround: Surround(); break;
        }
    }
    private void Attack()
    {
        Surround();
        //if (TanksManager.SingleManager.HeavyTanks.Count > 0) 
        //{
        //    if (TargetPlayer != null || Vector3.Distance(Target,transform.position)<0.08f)
        //    {
        //        TargetPlayer = null;
        //        Transform transform = TanksManager.SingleManager.HeavyTanks[Random.Range(0, TanksManager.SingleManager.HeavyTanks.Count - 1)].transform;
        //        Target = SearchingPositionAroundObject(transform, 0.48f);
        //    }
        //}
        //else 
        //{
        //    if (TargetPlayer == null) TargetPlayer = TanksManager.SingleManager.Player.transform;
        //}
        //if (ShouldShoot()) Shoot();
        //MoveToPosition = PositionForMovingToPosition(GetNextStep(), Time.deltaTime);
    }
    private void DefendBase()
    {
        Surround();
        //if (TargetPlayer != null)
        //{
        //    TargetPlayer = null;
        //    Target = SearchingPositionAroundObject(TanksManager.SingleManager.Factory.transform, 0.8f);
        //}
        //
        //if (Target.Equals(Vector3.zero)) Target = SearchingPositionAroundObject(TanksManager.SingleManager.Factory.transform, 0.8f);
        //
        //if (ShouldShoot()) Shoot();
        //MoveToPosition = PositionForMovingToPosition(GetNextStep(), Time.deltaTime);
    }
    private void Surround()
    {
        //if (TargetPlayer != null)
        //{
        //    Debug.Log("IM there");
        //    TargetPlayer = null;
        //    Target = SearchingPositionAroundObject(TanksManager.SingleManager.Player.transform, 0.64f);
        //}
        //
        //if(Target.Equals(Vector3.zero)) Target = SearchingPositionAroundObject(TanksManager.SingleManager.Player.transform, 0.64f);
        //

        if (TanksManager.SingleManager.Player != null)
        {
            TargetPlayer = TanksManager.SingleManager.Player;

            lastObservedPlayerPosition = TargetPlayer.TowerAxis.position;//Костыль
                                                               //Поворот башни
            Vector3 towerRotatetionTarget = lastObservedPlayerPosition;
            towerRotatetionTarget -= Vector3.up * towerRotatetionTarget.y;

            Debug.DrawLine(towerRotatetionTarget, TowerAxis.position,Color.black);

            AxisRotate(TowerAxis, towerRotatetionTarget, 0, towerRotationSpeed);
            if (AngleBetweenTarget(TowerAxis, towerRotatetionTarget) > 0.05f)
            {
                AxisRotate(TowerAxis, towerRotatetionTarget, 0, towerRotationSpeed);
            }

            BaseBehavior();

            if (ShouldShoot()) Shoot(hitDamage);
        }
    }
    private void BaseBehavior()
    {

        //Поворот базы
        if (AngleBetweenTarget(transform, GetNextStep()) < 0.05f) 
        {
            transform.position = PositionForMovingToPosition(GetNextStep()); // движение в сторону цели
        }
        else if (AngleBetweenTarget(transform, GetNextStep()) > 0.05f) AxisRotate(transform, GetNextStep(), 1, baseRotationSpeed); // поворот на цель
    }


}
