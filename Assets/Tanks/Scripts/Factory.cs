using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoDestructionObjects
{
    [SerializeField] Vector3 OutPutPosition;
    [SerializeField] List<BaseTankAI.Characteristics> LigthFirstAndHeavySecond = new List<BaseTankAI.Characteristics>();

    [SerializeField] BaseTankAI lightTankPrefab = null;
    [SerializeField] BaseTankAI heavyTankPrefab = null;
    void Start()
    {
        StartCoroutine(FactoryProccesBuilding());
    }

    IEnumerator FactoryProccesBuilding() 
    {
        while (Health > 0) 
        {
            bool buildLightTank = true;
            if (TanksManager.SingleManager.LightTanks.Count > 1) 
            {
                float proportions = (float)TanksManager.SingleManager.HeavyTanks.Count / (float)TanksManager.SingleManager.LightTanks.Count;
                if (proportions < 0.45f) buildLightTank = false;
            }
            yield return new WaitForSeconds(BuildTank(buildLightTank));
        }
    }

    private float BuildTank(bool lightTank) 
    {
        BaseTankAI.Characteristics characteristics;
        if (lightTank) characteristics = LigthFirstAndHeavySecond[0];
        else characteristics = LigthFirstAndHeavySecond[1];

        BaseTankAI tank;
        if (lightTank)
        {
            tank = Instantiate(lightTankPrefab, OutPutPosition, Quaternion.identity, transform.parent);
            tank.SetCharacteristics = characteristics;
            TanksManager.SingleManager.LightTanks.Add(tank);
        }
        else
        {
            tank = Instantiate(heavyTankPrefab, OutPutPosition, Quaternion.identity, transform.parent);
            tank.SetCharacteristics = characteristics;
            TanksManager.SingleManager.HeavyTanks.Add(tank);
        }

        return characteristics.AnotherTankBuildingTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(OutPutPosition, new Vector3(0.16f, 0.16f, 0.16f));
    }
}
