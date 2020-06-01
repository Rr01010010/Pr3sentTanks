//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTank : BaseTankAI
{
    void Start()
    {
        StartCoroutine(UpdatePathToPlayer());
    }
    private void FixedUpdate()
    {
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
        BaseTactic();
    }
    private void DefendBase()
    {
        BaseTactic();
    }
    private void Surround()
    {
        BaseTactic();
    }
    void BaseTactic() 
    {
        if (TanksManager.SingleManager.Player != null)
        {
            TargetPlayer = TanksManager.SingleManager.Player;

            lastObservedPlayerPosition = TargetPlayer.TowerAxis.position;//Костыль
                                                                         //Поворот башни
            Vector3 towerRotatetionTarget = lastObservedPlayerPosition;
            towerRotatetionTarget -= Vector3.up * towerRotatetionTarget.y;

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
