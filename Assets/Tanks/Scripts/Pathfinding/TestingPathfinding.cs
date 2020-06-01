//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestingPathfinding : MonoBehaviour 
{
    [SerializeField] bool StartSearch;
    [SerializeField] Transform FromObj;
    [SerializeField] Transform TargetObj;
    [SerializeField] Transform CenterSearchingObj;
    [SerializeField] PathFinding finding;
    [SerializeField] List<Vector3> path;
    private void Update()
    {
        if (StartSearch) 
        {
            StartSearch = false;
            
            finding.CenterSearchingZone = CenterSearchingObj;
            path = finding.StartPathFinding(FromObj.position, TargetObj.position);
        }
    }
    private void OnDrawGizmos()
    {
        if (path != null) 
        {
            for (int i = 1; i < path.Count; i++) 
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(path[i - 1], path[i]);
            }
        }
    }
}