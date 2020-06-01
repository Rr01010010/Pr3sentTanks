using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyTank : BaseTankAI
{
    void Start()
    {
        StartCoroutine(UpdatePathToPlayer());
    }
    private void Update()
    {
        if (!StopTankBehaviors)
        switch (TanksManager.SingleManager.TanksState) 
        {
            case TanksManager.PossibleStates.Attack: Attack();break;
            case TanksManager.PossibleStates.DefendBase: DefendBase();break;
            case TanksManager.PossibleStates.Surround: Surround();break;
        }
    }
    private void Attack() 
    {
        Surround();
        //if(TargetPlayer == null) TargetPlayer = TanksManager.SingleManager.Player.transform;
        //if (ShouldShoot()) Shoot();
        //MoveToPosition = PositionForMovingToPosition(GetNextStep(), Time.deltaTime);
    }
    private void DefendBase() 
    {
        Surround();
        //if (TargetPlayer != null) 
        //{
        //    TargetPlayer = null;
        //    Target = SearchingPositionAroundObject(TanksManager.SingleManager.Factory.transform,0.8f);
        //}
        //if (ShouldShoot()) Shoot();
        //MoveToPosition = PositionForMovingToPosition(GetNextStep(), Time.deltaTime);
    }
    private void Surround()
    {
        if (TargetPlayer != null)
        {
            //TargetPlayer = null;
            //Target = SearchingPositionAroundObject(TanksManager.SingleManager.Player.transform,0.64f);
        }





        if(TanksManager.SingleManager.Player != null) 
        {
            TargetPlayer = TanksManager.SingleManager.Player;
            lastObservedPlayerPosition = TargetPlayer.TowerAxis.position;//Костыль
                                                               //Поворот башни
            Vector3 towerRotatetionTarget=lastObservedPlayerPosition;

            towerRotatetionTarget += Vector3.down * towerRotatetionTarget.y;

            if (AngleBetweenTarget(TowerAxis, towerRotatetionTarget) > 0.05f)
            {
                AxisRotate(TowerAxis, towerRotatetionTarget, 0, towerRotationSpeed);
            }

            //Поворот базы
            if (AngleBetweenTarget(transform, GetNextStep()) < 0.05f)
            {
                transform.position = PositionForMovingToPosition(GetNextStep());
            }
            else
            {
                if (AngleBetweenTarget(transform, GetNextStep()) > 0.05f) AxisRotate(transform, GetNextStep(), 1, baseRotationSpeed);
            }

            if (ShouldShoot()) Shoot(hitDamage);
        }
        
    }
}
